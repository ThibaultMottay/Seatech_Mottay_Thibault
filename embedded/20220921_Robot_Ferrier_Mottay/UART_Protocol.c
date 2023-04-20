#include <xc.h>
#include "UART_Protocol.h"

unsigned char UartCalculateChecksum(int msgFunction, int msgPayloadLength, unsigned char* msgPayload)
{
//Fonction prenant entrée la trame et sa longueur pour calculer le checksum
        unsigned char checksum = 0;
        checksum ^= 0xFE;
        checksum ^= (unsigned char)(msgFunction >> 8);
        checksum ^= (unsigned char)(msgFunction >> 0);

        checksum ^= (unsigned char)(msgPayloadLength >> 8);
        checksum ^= (unsigned char)(msgPayloadLength >> 0);
        for  (int i = 0;i< msgPayloadLength; i++)
        {
            checksum ^= msgPayload[i];
        }
        return checksum;
}

void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, unsigned char* msgPayload)
{
//Fonction d?encodage et d?envoi d?un message
            unsigned char byteList[6 + msgPayloadLength];
            int pos = 0;
            byteList[pos++]= 0xFE;
            byteList[pos++] = (unsigned char)(msgFunction >> 8);
            byteList[pos++] = (unsigned char)(msgFunction >> 0);
            byteList[pos++] = (unsigned char)(msgPayloadLength >> 8);
            byteList[pos++] = (unsigned char)(msgPayloadLength >> 0);
            for (int i = 0; i < msgPayloadLength; i++)
            {
                byteList[pos++] = msgPayload[i];
            }
            unsigned char checksum = UartCalculateChecksum(msgFunction, msgPayloadLength, msgPayload);
            byteList[pos++] = checksum;
            
            SendMessage(byteList, pos);
}
enum StateReception
{
        Waiting,
        FunctionMSB,
        FunctionLSB,
        PayloadLengthMSB,
        PayloadLengthLSB,
        Payload,
        CheckSum
};
int msgDecodedFunction = 0;
int msgDecodedPayloadLength = 0;
unsigned char msgDecodedPayload[128];
int msgDecodedPayloadIndex = 0;
enum StateReception rcvState = Waiting;



void UartDecodeMessage(unsigned char c)
{
            switch (rcvState)
            {
                case Waiting:
                    if (c == 0xFE)
                        rcvState = FunctionMSB;
                    break;
                case FunctionMSB:
                    msgDecodedFunction = c << 8;
                    rcvState = FunctionLSB;
                    break;
                case FunctionLSB:
                    msgDecodedFunction += c << 0;
                    rcvState = PayloadLengthMSB;
                    break;
                case PayloadLengthMSB:
                    msgDecodedPayloadLength = c << 8;
                    rcvState = PayloadLengthLSB;
                    break;
                case PayloadLengthLSB:
                    msgDecodedPayloadLength += c << 0;
                    if (msgDecodedPayloadLength == 0)
                        rcvState = CheckSum;
                    else
                    {
                        rcvState = Payload;
                        msgDecodedPayloadIndex = 0;
                    }
                    break;
                case Payload:                    
                        msgDecodedPayload[msgDecodedPayloadIndex] = c;
                        msgDecodedPayloadIndex++;
                        if (msgDecodedPayloadIndex >= msgDecodedPayloadLength)
                            rcvState = CheckSum;
                    break;
                case CheckSum:
                {
                    unsigned char receivedChecksum = c;
                    unsigned char calculatedChecksum = UartCalculateChecksum( msgDecodedFunction,  msgDecodedPayloadLength,   msgDecodedPayload);
                    if (calculatedChecksum == receivedChecksum)
                    {
                        //Success, on a un message valide
                    }
                    else 
                    {
                    
                    }
                    rcvState = Waiting;
                }
                    break;
                default:
                    rcvState = Waiting;
                    break;
            }
//Fonction prenant en entrée un octet et servant à reconstituer les trames

}
/*
void UartProcessDecodedMessage(unsigned char function, unsigned char payloadLength, unsigned char* payload)
{
//Fonction appelée après le décodage pour exécuter l?action
//correspondant au message reçu

}*/