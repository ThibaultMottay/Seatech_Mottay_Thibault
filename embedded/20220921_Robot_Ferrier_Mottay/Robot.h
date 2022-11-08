/* 
 * File:   Robot.h
 * Author: TP-EO-5
 *
 * Created on 27 septembre 2022, 14:48
 */
#ifndef ROBOT_H
#define ROBOT_H

typedef struct robotStateBITS {
    union {
        struct {
            unsigned char taskEnCours;
            float vitesseGaucheConsigne;
            float vitesseGaucheCommandeCourante;
            float vitesseDroiteConsigne;
            float vitesseDroiteCommandeCourante;
            float distanceTelemetreUltraDroit;
            float distanceTelemetreDroit;
            float distanceTelemetreCentre;
            float distanceTelemetreGauche;
            float distanceTelemetreUltraGauche;
        };
    };
}ROBOT_STATE_BITS;
extern volatile ROBOT_STATE_BITS robotState;

#endif 
