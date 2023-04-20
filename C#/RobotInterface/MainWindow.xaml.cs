using ExtendedSerialPort;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RobotInterface
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReliableSerialPort serialPort1;
        // Instanciation classe robot
        Robot robot = new Robot();

        public MainWindow()
        {
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();
            /// Mise en place du timer
            DispatcherTimer timerAffichage;
            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();

            textBoxEmission.KeyUp += TextBoxEmission_KeyUp;
        }

        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            /*if (robot.receivedText != "")
            {
                textBoxReception.Text += robot.receivedText;
                robot.receivedText = "";
            }*/

            while(robot.byteListReceived.Count > 0)
            {
                //textBoxReception.Text+="0x" + robot.byteListReceived.Dequeue().ToString("X2") + " ";
                var c = robot.byteListReceived.Dequeue();
                DecodeMessage(c);

            }
        }


        public void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            //robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            for (int i = 0; i < e.Data.Length; i++)
            {
                robot.byteListReceived.Enqueue(e.Data[i]);
            }
        }

        private void SendMessage()
        {
            serialPort1.WriteLine(textBoxEmission.Text);
            //textBoxReception.Text += "Reçu : " + textBoxEmission.Text + "\r";
            textBoxEmission.Text = "";
        }

        private void TextBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }

        }

        bool toggle = false;
        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            //if(toggle)
            //    buttonEnvoyer.Background = Brushes.RoyalBlue;
            //else
            //    buttonEnvoyer.Background = Brushes.Beige;
            //toggle = !toggle;

            SendMessage();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxReception.Text ="";
        }
        byte[] byteList=new byte[20];
        private void buttonTest_Click(object sender, RoutedEventArgs e) {

            /*for (int i = 0; i<20; i++){
                byteList[i] = (byte)(2*i);
            }
            serialPort1.Write(byteList, 0, byteList.Length);*/
            int msgfunction = 0x0080;
            string s = "Bonjour";
            byte[] Payload = Encoding.ASCII.GetBytes(s);
            UartEncodeAndSendMessage( msgfunction, Payload.Length, Payload);
            UartEncodeAndSendMessage(0x0020, 2, new byte[] { 1, 1 });
            UartEncodeAndSendMessage(0x0030, 3, new byte[] { 25, 64, 75 });


        }
        // Implantation d'un message
        // Calcul de la checksum
        byte CalculateChecksum(int msgFunction,int msgPayloadLength, byte[] msgPayload)
        {
            byte checksum = 0;
            checksum ^= 0xFE;
            checksum ^= (byte)(msgFunction >> 8);// on est sur des entiers en int 16, on récupère le bit de poids fort puis le bit de poids faible
            checksum ^= (byte)(msgFunction >> 0);

            checksum ^= (byte)(msgPayloadLength >> 8);
            checksum ^= (byte)(msgPayloadLength >> 0);
            for  (int i = 0;i< msgPayloadLength; i++)
            {
                checksum ^= msgPayload[i];
            }
            return checksum;
        }
        void UartEncodeAndSendMessage(int msgFunction,int msgPayloadLength, byte[] msgPayload)
        {
            byte[] byteList = new byte[6 + msgPayloadLength];
            int pos = 0;
            byteList[pos++]= 0xFE;
            byteList[pos++] = (byte)(msgFunction >> 8);
            byteList[pos++] = (byte)(msgFunction >> 0);
            byteList[pos++] = (byte)(msgPayloadLength >> 8);
            byteList[pos++] = (byte)(msgPayloadLength >> 0);
            for (int i = 0; i < msgPayloadLength; i++)
            {
                byteList[pos++] = msgPayload[i];
            }
            byte checksum = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload);
            byteList[pos++] = checksum;
            serialPort1.Write(byteList, 0, pos);
        }
        public enum StateReception
        {
            Waiting,
            FunctionMSB,
            FunctionLSB,
            PayloadLengthMSB,
            PayloadLengthLSB,
            Payload,
            CheckSum
        }
        StateReception rcvState = StateReception.Waiting;
        int msgDecodedFunction = 0;
        int msgDecodedPayloadLength = 0;
        byte[] msgDecodedPayload;
        int msgDecodedPayloadIndex = 0;
        private void DecodeMessage(byte c)
        {
            switch (rcvState)
            {
                case StateReception.Waiting:
                    if (c == 0xFE)
                        rcvState = StateReception.FunctionMSB;
                    break;
                case StateReception.FunctionMSB:
                    msgDecodedFunction = c << 8;
                    rcvState = StateReception.FunctionLSB;
                    break;
                case StateReception.FunctionLSB:
                    msgDecodedFunction += c << 0;
                    rcvState = StateReception.PayloadLengthMSB;
                    break;
                case StateReception.PayloadLengthMSB:
                    msgDecodedPayloadLength = c << 8;
                    rcvState = StateReception.PayloadLengthLSB;
                    break;
                case StateReception.PayloadLengthLSB:
                    msgDecodedPayloadLength += c << 0;
                    if (msgDecodedPayloadLength == 0)
                        rcvState = StateReception.CheckSum;
                    else
                    {
                        rcvState = StateReception.Payload;
                        msgDecodedPayload = new byte[msgDecodedPayloadLength];
                        msgDecodedPayloadIndex = 0;
                    }
                    break;
                case StateReception.Payload:                    
                        msgDecodedPayload[msgDecodedPayloadIndex] = c;
                        msgDecodedPayloadIndex++;
                        if (msgDecodedPayloadIndex >= msgDecodedPayloadLength)
                            rcvState = StateReception.CheckSum;
                    break;
                case StateReception.CheckSum:
                    byte receivedChecksum = c;
                    if (CalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload) == receivedChecksum)
                    {
                        //Success, on a un message valide
                        textBoxReception.Text += "message valide";
                        ProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                    }
                    else textBoxReception.Text += "message invalide";
                    rcvState = StateReception.Waiting;
                    break;
                default:
                    rcvState = StateReception.Waiting;
                    break;
            }
        }
        public enum StateRobot
        {
            STATE_ATTENTE = 0,
            STATE_ATTENTE_EN_COURS = 1,
            STATE_AVANCE = 2,
            STATE_AVANCE_EN_COURS = 3,
            STATE_TOURNE_GAUCHE = 4,
            STATE_TOURNE_GAUCHE_EN_COURS = 5,
            STATE_TOURNE_DROITE = 6,
            STATE_TOURNE_DROITE_EN_COURS = 7,
            STATE_TOURNE_SUR_PLACE_GAUCHE = 8,
            STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS = 9,
            STATE_TOURNE_SUR_PLACE_DROITE = 10,
            STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS = 11,
            STATE_ARRET = 12,
            STATE_ARRET_EN_COURS = 13,
            STATE_RECULE = 14,
            STATE_RECULE_EN_COURS = 15,
            STATE_RETOUR = 16
        }

        void ProcessDecodedMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            switch (msgFunction)
            {
                case 0x0080:
                    LabelTransmissionDeTexte.Content = System.Text.Encoding.ASCII.GetString(msgPayload);
                    break;
                case 0x0020:
                    switch(msgPayload[0])
                    {
                        case 0:
                            if (msgPayload[1] == 1) { Led1.IsChecked = true; }
                            else { Led1.IsChecked = false; };
                            break;
                        case 1:
                            if (msgPayload[1] == 1) { Led2.IsChecked = true; }
                            else { Led2.IsChecked = false; };
                            break;
                        case 2:
                            if (msgPayload[1] == 1) { Led3.IsChecked = true; }
                            else { Led3.IsChecked = false; };
                            break;
                    }
                    break;
                case 0x0030:
                    LabelIRGauche.Content = "IR Gauche = " + msgPayload[0].ToString() + " cm";
                    LabelIRCentre.Content = "IR Centre = " + msgPayload[1].ToString() + " cm";
                    LabelIRDroit.Content = "IR Droit = " + msgPayload[2].ToString() + " cm";
                    break;
                case 0x0040:
                    LabelVitesseGauche.Content = "Vitesse Gauche = " + msgPayload[0].ToString() + " %";
                    LabelVitesseDroit.Content = "Vitesse Droit = " + msgPayload[1].ToString() + " %";
                    break;
                case 0x0050:
                    LabelEtape.Content = "Etape = " + ((StateRobot)(msgPayload[0])).ToString();
                    int instant = (((int)msgPayload[1]) << 24) + (((int)msgPayload[2]) << 16) + (((int)msgPayload[3]) << 8) + ((int)msgPayload[4]);

                    LabelTemps.Content ="instant courant = " + instant.ToString() + " ms";
                    break;
            }
        }

    }
}