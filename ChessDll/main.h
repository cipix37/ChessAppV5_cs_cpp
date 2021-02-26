#ifndef __MAIN_H__
#define __MAIN_H__

#include <windows.h>

/*  To use this exported function of dll, include this header
 *  in your project.
 */

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C"
{
#endif

#include <math.h>
#include <time.h>
//#include <stdlib.h>
#define nr_max 240
#define nr_max3f 200
#define depth_max 20

struct Mutare{
    int r1=-1, r2=-1, c1=-1, c2=-1, extra=0;
    double val=0;

	Mutare(){}
    Mutare(int x1,int y1,int x2,int y2){
        r1=x1;
        r2=x2;
        c1=y1;
        c2=y2;
    }
    Mutare(int x1,int y1,int x2,int y2,int e){
        r1=x1;
        r2=x2;
        c1=y1;
        c2=y2;
        extra=e;
    }
    Mutare(double v){
        val=v;
    }

    bool operator ==(Mutare m){
        if(r1==m.r1 && r2==m.r2 && c1==m.c1 && c2==m.c2 && extra==m.extra)return true;
        return false;
    }
};

int sfarsit,nr_mutari3f,ra,ca,rn,cn,rege_alb,rege_negru,turn_alb_mic,turn_alb_mare,turn_negru_mic,turn_negru_mare;
int t[8][8], piese[2 * nr_max3f], nr[depth_max], mutari50[2 * nr_max3f], n[depth_max];
Mutare Mutari3f[2 * nr_max3f], m[depth_max][nr_max];
double (* evaluare_generala)(int depth);
bool dinamic, RTable;

char* EvaluationNames[] = { (char*)"Random", (char*)"Basic", (char*)"Position", (char*)"Position2", (char*)"Moves" , (char*)"Move Position" };

int DLL_EXPORT GetNrMutari3f();
//char** DLL_EXPORT GetEvaluationNames(int* count);
int DLL_EXPORT GetSfarsit();
Mutare DLL_EXPORT lm();
Mutare DLL_EXPORT lm_alb();
Mutare DLL_EXPORT lm_negru();
Mutare DLL_EXPORT AIGeneral(int player, int leaf_depth, int evaluare, bool d, bool RT);
int DLL_EXPORT verificare_initializare(int tt[][8], int turn);
bool DLL_EXPORT verificare_mutare(Mutare mm, int player);

#ifdef __cplusplus
}
#endif

#endif // __MAIN_H__
