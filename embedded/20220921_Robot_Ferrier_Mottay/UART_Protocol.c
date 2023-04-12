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

int msgDecodedFunction = 0;
int msgDecodedPayloadLength = 0;
unsigned char msgDecodedPayload[128];
int msgDecodedPayloadIndex = 0;

/*void UartDecodeMessage(unsigned char c)
{
//Fonction prenant en entrée un octet et servant à reconstituer les trames

}

void UartProcessDecodedMessage(unsigned char function, unsigned char payloadLength, unsigned char* payload)
{
//Fonction appelée après le décodage pour exécuter l?action
//correspondant au message reçu

}*/