/* 
 * File:   timer.h
 * Author: TP-EO-5
 *
 * Created on 21 septembre 2022, 16:22
 */

#ifndef TIMER_H
#define	TIMER_H
void InitTimer23( void ) ;
void InitTimer1( void ) ;
void __attribute__((interrupt,no_auto_psv )) _T3Interrupt(void) ;
#endif	/* TIMER_H */

