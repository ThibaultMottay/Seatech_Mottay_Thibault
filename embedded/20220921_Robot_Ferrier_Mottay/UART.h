/* 
 * File:   UART.h
 * Author: TP-EO-5
 *
 * Created on 1 février 2023, 11:08
 */

#ifndef UART_H
#define	UART_H

void InitUART(void);
void SendMessageDirect(unsigned char* message, int length);

#endif	/* UART_H */

