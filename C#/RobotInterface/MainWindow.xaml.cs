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
            serialPort1 = new ReliableSerialPort("COM11", 115200, Parity.None, 8, StopBits.One);
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
                textBoxReception.Text+="0x" + robot.byteListReceived.Dequeue().ToString("X2") + " ";  
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
        private void buttonTest_Click(object sender, RoutedEventArgs e)
            {
            for (int i = 0; i<20; i++){
                byteList[i] = (byte)(2*i);
            }
            serialPort1.Write(byteList, 0, byteList.Length);
        }
    }
}