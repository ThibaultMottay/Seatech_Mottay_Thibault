/* 
 * File:   main.c
 * Author: TP-EO-5
 *
 * Created on 21 septembre 2022, 15:01
 */

#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include "ADC.h"
unsigned int ADCValue0;
unsigned int ADCValue1;
unsigned int ADCValue2;

int main(void) {
    /***************************************************************************************************/
    //Initialisation de l?oscillateur
    /****************************************************************************************************/
    InitOscillator();

    /****************************************************************************************************/
    // Configuration des entrées sorties
    /****************************************************************************************************/
    InitIO();

    LED_BLANCHE = 1;
    LED_BLEUE = 1;
    LED_ORANGE = 1;

    InitTimer23();
    InitTimer1();
    InitPWM();
    InitADC1();
    //PWMSetSpeed(0,2);

    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
        if (ADCIsConversionFinished()==1){
            ADCClearConversionFinishedFlag();
            /*unsigned int * result = ADCGetResult();
            ADCValue0 = result[0];
            ADCValue1 = result[1];
            ADCValue2 = result[2];*/
            unsigned int * result = ADCGetResult();
            float volts = ((float) result [2]) * 3.3 / 4096 * 3.2 ;
            robotState.distanceTelemetreDroit = 34 / volts - 5 ;
            volts = ((float) result [1]) * 3.3 / 4096 * 3.2 ;
            robotState.distanceTelemetreCentre = 34 / volts - 5 ;
            volts = ((float) result[0]) * 3. 3 / 4096 * 3 . 2 ;
            robotState.distanceTelemetreGauche = 34 / volts - 5 ;
        }
    } // fin main
}

