/* 
 * File:   PWM.h
 * Author: TP-EO-5
 *
 * Created on 27 septembre 2022, 15:13
 */

#ifndef PWM_H
#define	PWM_H
#define MOTEUR_DROIT 1
#define MOTEUR_GAUCHE 0
#define BI_MOTEUR 2

void InitPWM(void);
//void PWMSetSpeed(float vitesseEnPourcents, unsigned char motorNumber );
void PWMUpdateSpeed();
void PWMSetSpeedConsigne (float vitesseEnpourcents, unsigned char Moteur);

#endif	/* PWM_H */

