#include "main.h"

template <typename T> int sgn(T val) {
    return (T(0) < val) - (val < T(0));
}

int GetNrMutari3f()
{
    return nr_mutari3f;
}
/*
char** DLL_EXPORT GetEvaluationNames(int* count)
{
    *count = 6;
    return EvaluationNames;
}
*/
int DLL_EXPORT GetSfarsit()
{
    return sfarsit;
}
Mutare DLL_EXPORT lm() {
	if (nr_mutari3f != -1) return Mutari3f[nr_mutari3f];
	return Mutare(-1, -1, -1, -1, -1);
}
Mutare DLL_EXPORT lm_alb() {
	if (nr_mutari3f != -1)
	{
		if (t[lm().r2][lm().c2] > 0) return lm();
		else if (nr_mutari3f > 0) return Mutari3f[nr_mutari3f - 1];
		return Mutare(-1, -1, -1, -1, -1);
	}
	return Mutare(-1, -1, -1, -1, -1);
}
Mutare DLL_EXPORT lm_negru() {
	if (nr_mutari3f != -1)
	{
		if (t[lm().r2][lm().c2] < 0) return lm();
		else if (nr_mutari3f > 0) return Mutari3f[nr_mutari3f - 1];
		return Mutare(-1, -1, -1, -1, -1);
	}
	return Mutare(-1, -1, -1, -1, -1);
}
int x(int player){
    if (player == 1) return ra;
    return rn;
}
int y(int player){
    if (player == 1) return ca;
    return cn;
}

//trebuie sa fie vizibile din exterior pt undo/redo, si atunci verificare mutare nu o si executa
void schimbare(Mutare m){
    nr_mutari3f++;
    //regula de 50 mutari
    if (abs(t[m.r1][m.c1]) == 1 || t[m.r2][m.c2] != 0 || nr_mutari3f == 0) mutari50[nr_mutari3f] = 1;
    else mutari50[nr_mutari3f] = mutari50[nr_mutari3f - 1] + 1;
    //mutare normala
    Mutari3f[nr_mutari3f] = m;
    piese[nr_mutari3f] = t[m.r2][m.c2];
    t[m.r2][m.c2] = t[m.r1][m.c1];
    t[m.r1][m.c1] = 0;
    //en passant
    if (m.extra == 1) t[m.r1][m.c2] = 0;
    //transformare
    if (1 < m.extra && m.extra < 6) t[m.r2][m.c2] *= m.extra;
    //rocadele
    if (m.extra == 6) { t[m.r1][3] = t[m.r1][0]; t[m.r1][0] = 0; }
    if (m.extra == 7) { t[m.r1][5] = t[m.r1][7]; t[m.r1][7] = 0; }
    if (m.extra == 8) { t[m.r1][3] = t[m.r1][0]; t[m.r1][0] = 0; }
    if (m.extra == 9) { t[m.r1][5] = t[m.r1][7]; t[m.r1][7] = 0; }
    //mutare pt rocada
    if (m.r1 == 0)
    {
        if (m.c1 == 0) if (turn_alb_mare == -1) turn_alb_mare = nr_mutari3f;
        if (m.c1 == 4) if (rege_alb == -1) rege_alb = nr_mutari3f;
        if (m.c1 == 7) if (turn_alb_mic == -1) turn_alb_mic = nr_mutari3f;
    }
    if (m.r1 == 7)
    {
        if (m.c1 == 0) if (turn_negru_mare == -1) turn_negru_mare = nr_mutari3f;
        if (m.c1 == 4) if (rege_negru == -1) rege_negru = nr_mutari3f;
        if (m.c1 == 7) if (turn_negru_mic == -1) turn_negru_mic = nr_mutari3f;
    }
    //captura pt rocada
    if (m.r2 == 0)
    {
        if (m.c2 == 0) if (turn_alb_mare == -1) turn_alb_mare = nr_mutari3f;
        if (m.c2 == 7) if (turn_alb_mic == -1) turn_alb_mic = nr_mutari3f;
    }
    if (m.r2 == 7)
    {
        if (m.c2 == 0) if (turn_negru_mare == -1) turn_negru_mare = nr_mutari3f;
        if (m.c2 == 7) if (turn_negru_mic == -1) turn_negru_mic = nr_mutari3f;
    }
    //mutare regi
    if (t[m.r2][m.c2] == 6) { ra = m.r2; ca = m.c2; }
    if (t[m.r2][m.c2] == -6) { rn = m.r2; cn = m.c2; }
}
void schimbare_inversa(Mutare m){
    t[m.r1][m.c1] = t[m.r2][m.c2];
    t[m.r2][m.c2] = piese[nr_mutari3f];
    //en passant
    if (m.extra == 1) t[m.r1][m.c2] = -t[m.r1][m.c1];
    //transformare
    if (1 < m.extra && m.extra < 6) t[m.r1][m.c1] = sgn(t[m.r1][m.c1]);
    //rocadele
    if (m.extra == 6) { t[0][0] = 4; t[0][3] = 0; }
    if (m.extra == 7) { t[0][7] = 4; t[0][5] = 0; }
    if (m.extra == 8) { t[7][0] = -4; t[7][3] = 0; }
    if (m.extra == 9) { t[7][7] = -4; t[7][5] = 0; }
    //conditii rocade
    if (rege_alb == nr_mutari3f) rege_alb = -1;
    if (turn_alb_mic == nr_mutari3f) turn_alb_mic = -1;
    if (turn_alb_mare == nr_mutari3f) turn_alb_mare = -1;
    if (rege_negru == nr_mutari3f) rege_negru = -1;
    if (turn_negru_mic == nr_mutari3f) turn_negru_mic = -1;
    if (turn_negru_mare == nr_mutari3f) turn_negru_mare = -1;
    //mutare regi
    if (t[m.r1][m.c1] == 6) { ra = m.r1; ca = m.c1; }
    if (t[m.r1][m.c1] == -6) { rn = m.r1; cn = m.c1; }
    //resetare sfarsit
    nr_mutari3f--;
    sfarsit = 0;
}

bool atacat_de_pion(int r, int c, int p){
    if (p * r < 3.5 + p * 3.5)
    {
        if (c < 7) if (t[r + p][c + 1] == -p) return true;
        if (c > 0) if (t[r + p][c - 1] == -p) return true;
    }
    return false;
}
bool atacat_de_cal(int r, int c, int p){
    p = -2 * p;
    if (r < 6)
    {
        if (c < 7) if (t[r + 2][c + 1] == p) return true;
        if (c > 0) if (t[r + 2][c - 1] == p) return true;
    }
    if (r > 1)
    {
        if (c < 7) if (t[r - 2][c + 1] == p) return true;
        if (c > 0) if (t[r - 2][c - 1] == p) return true;
    }
    if (r < 7)
    {
        if (c < 6) if (t[r + 1][c + 2] == p) return true;
        if (c > 1) if (t[r + 1][c - 2] == p) return true;
    }
    if (r > 0)
    {
        if (c < 6) if (t[r - 1][c + 2] == p) return true;
        if (c > 1) if (t[r - 1][c - 2] == p) return true;
    }
    return false;
}
bool atacat_diagonala(int r, int c, int p){
    short k, p1 = -3 * p, p2 = -5 * p;
    for (k = 1; r + k <= 7 && c + k <= 7; k++)
    {
        if (t[r + k][c + k] == p1 || t[r + k][c + k] == p2) return true;
        if (t[r + k][c + k] != 0) break;
    }
    for (k = 1; r + k <= 7 && c - k >= 0; k++)
    {
        if (t[r + k][c - k] == p1 || t[r + k][c - k] == p2) return true;
        if (t[r + k][c - k] != 0) break;
    }
    for (k = 1; r - k >= 0 && c + k <= 7; k++)
    {
        if (t[r - k][c + k] == p1 || t[r - k][c + k] == p2) return true;
        if (t[r - k][c + k] != 0) break;
    }
    for (k = 1; r - k >= 0 && c - k >= 0; k++)
    {
        if (t[r - k][c - k] == p1 || t[r - k][c - k] == p2) return true;
        if (t[r - k][c - k] != 0) break;
    }
    return false;
}
bool atacat_linie(int r, int c, int p){
    short k, p1 = -4 * p, p2 = -5 * p;
    for (k = 1; c + k <= 7; k++)
    {
        if (t[r][c + k] == p1 || t[r][c + k] == p2) return true;
        if (t[r][c + k] != 0) break;
    }
    for (k = 1; c - k >= 0; k++)
    {
        if (t[r][c - k] == p1 || t[r][c - k] == p2) return true;
        if (t[r][c - k] != 0) break;
    }
    for (k = 1; r + k <= 7; k++)
    {
        if (t[r + k][c] == p1 || t[r + k][c] == p2) return true;
        if (t[r + k][c] != 0) break;
    }
    for (k = 1; r - k >= 0; k++)
    {
        if (t[r - k][c] == p1 || t[r - k][c] == p2) return true;
        if (t[r - k][c] != 0) break;
    }
    return false;
}
bool atacat_de_rege(int r, int c, int p){
    //ma folosesc de faptul ca nu imi trebuie decat pentru regi
    if (abs(ra - rn) < 2 && abs(ca - cn) < 2) return true;
    return false;
}
bool atacat(int r, int c, int p){
    if (atacat_de_rege(r, c, p)) return true;
    if (atacat_de_pion(r, c, p)) return true;
    if (atacat_de_cal(r, c, p)) return true;
    if (atacat_diagonala(r, c, p)) return true;
    if (atacat_linie(r, c, p)) return true;
    return false;
}

void transformare(int r1, int c1, int r2, int c2, int depth){
    nr[depth]++;
    if (r2 == 3.5 + 3.5 * t[r1][c1])
    {
        m[depth][nr[depth]] = Mutare(r1, c1, r2, c2, 2);
        m[depth][nr[depth] + 1] = Mutare(r1, c1, r2, c2, 3);
        m[depth][nr[depth] + 2] = Mutare(r1, c1, r2, c2, 4);
        m[depth][nr[depth] + 3] = Mutare(r1, c1, r2, c2, 5);
        nr[depth] = nr[depth] + 3;
    }
    else m[depth][nr[depth]] = Mutare(r1, c1, r2, c2);
}
void det_pion(int r, int c, int depth){
    //mutare in fata
    if (t[r + t[r][c]][c] == 0)
        transformare(r, c, r + t[r][c], c, depth);
    //captura
    if (c > 0)
        if (t[r + t[r][c]][c - 1] * t[r][c] < 0)
            transformare(r, c, r + t[r][c], c - 1, depth);
    if (c < 7)
        if (t[r + t[r][c]][c + 1] * t[r][c] < 0)
            transformare(r, c, r + t[r][c], c + 1, depth);
    //mutare dubla
    int aux = (int)(3.5 - 0.5 * t[r][c]);//util si pt mutarea dubla si pt en passant
    if (r == aux - 2 * t[r][c] && t[aux - t[r][c]][c] == 0 && t[aux][c] == 0)
    {
        nr[depth]++;
        m[depth][nr[depth]] = Mutare(aux - 2 * t[r][c], c, aux, c);
    }
	//en_passant
	if (r == aux + t[r][c] && nr_mutari3f != -1)
	{
		Mutare lmx = lm();
		if (abs(t[lmx.r2][lmx.c2]) == 1 && abs(lmx.r2 - lmx.r1) == 2)
		{
			if (c - 1 == lmx.c2)
			{
				nr[depth]++;
				m[depth][nr[depth]] = Mutare(aux + t[r][c], c, aux + 2 * t[r][c], c - 1, 1);
			}
			if (c + 1 == lmx.c2)
			{
				nr[depth]++;
				m[depth][nr[depth]] = Mutare(aux + t[r][c], c, aux + 2 * t[r][c], c + 1, 1);
			}
		}
	}
}
void det_cal(int r, int c, int depth){
    if (r < 6)
    {
        if (c > 0)
            if (t[r + 2][c - 1] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r + 2, c - 1);
            }
        if (c < 7)
            if (t[r + 2][c + 1] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r + 2, c + 1);
            }
    }
    if (r > 1)
    {
        if (c > 0)
            if (t[r - 2][c - 1] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r - 2, c - 1);
            }
        if (c < 7)
            if (t[r - 2][c + 1] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r - 2, c + 1);
            }
    }
    if (r < 7)
    {
        if (c > 1)
            if (t[r + 1][c - 2] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r + 1, c - 2);
            }
        if (c < 6)
            if (t[r + 1][c + 2] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r + 1, c + 2);
            }
    }
    if (r > 0)
    {
        if (c > 1)
            if (t[r - 1][c - 2] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r - 1, c - 2);
            }
        if (c < 6)
            if (t[r - 1][c + 2] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r - 1, c + 2);
            }
    }
}
void det_nebun(int r, int c, int depth){
    short k;
    for (k = 1; r + k <= 7 && c + k <= 7; k++)
    {
        if (t[r + k][c + k] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r + k, c + k);
        }
        if (t[r + k][c + k] != 0) break;
    }
    for (k = 1; r + k <= 7 && c - k >= 0; k++)
    {
        if (t[r + k][c - k] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r + k, c - k);
        }
        if (t[r + k][c - k] != 0) break;
    }
    for (k = 1; r - k >= 0 && c + k <= 7; k++)
    {
        if (t[r - k][c + k] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r - k, c + k);
        }
        if (t[r - k][c + k] != 0) break;
    }
    for (k = 1; r - k >= 0 && c - k >= 0; k++)
    {
        if (t[r - k][c - k] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r - k, c - k);
        }
        if (t[r - k][c - k] != 0) break;
    }
}
void det_turn(int r, int c, int depth){
    short k;
    for (k = 1; c + k <= 7; k++)
    {
        if (t[r][c + k] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r, c + k);
        }
        if (t[r][c + k] != 0) break;
    }
    for (k = 1; c - k >= 0; k++)
    {
        if (t[r][c - k] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r, c - k);
        }
        if (t[r][c - k] != 0) break;
    }
    for (k = 1; r + k <= 7; k++)
    {
        if (t[r + k][c] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r + k, c);
        }
        if (t[r + k][c] != 0) break;
    }
    for (k = 1; r - k >= 0; k++)
    {
        if (t[r - k][c] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r - k, c);
        }
        if (t[r - k][c] != 0) break;
    }
}
void det_dama(int r, int c, int depth){
    det_nebun(r, c, depth);
    det_turn(r, c, depth);
}
void det_rege(int r, int c, int depth){
    int p = sgn(t[r][c]);
    //mutari normale
    if (r < 7)
    {
        if (t[r + 1][c] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r + 1, c);
        }
        if (c > 0)
            if (t[r + 1][c - 1] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r + 1, c - 1);
            }
        if (c < 7)
            if (t[r + 1][c + 1] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r + 1, c + 1);
            }
    }
    if (r > 0)
    {
        if (t[r - 1][c] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r - 1, c);
        }
        if (c > 0)
            if (t[r - 1][c - 1] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r - 1, c - 1);
            }
        if (c < 7)
            if (t[r - 1][c + 1] * t[r][c] <= 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = Mutare(r, c, r - 1, c + 1);
            }
    }
    if (c > 0)
        if (t[r][c - 1] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r, c - 1);
        }
    if (c < 7)
        if (t[r][c + 1] * t[r][c] <= 0)
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(r, c, r, c + 1);
        }
    //region rocade
    if (t[r][c] > 0 && rege_alb == -1 && !atacat(0, 4, p))//rocade albe
    {
        //rocada mica
        if (turn_alb_mic == -1 && t[0][5] == 0 && t[0][6] == 0 && !atacat(0, 5, p))
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(0, 4, 0, 6, 7);
        }
        //rocada mare
        if (turn_alb_mare == -1 && t[0][1] == 0 && t[0][2] == 0 && t[0][3] == 0 && !atacat(0, 3, p))
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(0, 4, 0, 2, 6);
        }
    }
    if (t[r][c] < 0 && rege_negru == -1 && !atacat(7, 4, p))//rocade negre
    {
        //rocada mica
        if (turn_negru_mic == -1 && t[7][5] == 0 && t[7][6] == 0 && !atacat(7, 5, p))
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(7, 4, 7, 6, 9);
        }
        //rocada mare
        if (turn_negru_mare == -1 && t[7][1] == 0 && t[7][2] == 0 && t[7][3] == 0 && !atacat(7, 3, p))
        {
            nr[depth]++;
            m[depth][nr[depth]] = Mutare(7, 4, 7, 2, 8);
        }
    }
}
void det_generala(int depth, int player, bool quiescence){
    short i, j, other = 0, light = 0;
    nr[depth] = -1;
    for (i = 0; i <= 7; i++)
        for (j = 0; j <= 7; j++)
            if (t[i][j] * player > 0)
                switch (abs(t[i][j]))
                {
                    case 1: { det_pion(i, j, depth); break; }
                    case 2: { det_cal(i, j, depth); break; }
                    case 3: { det_nebun(i, j, depth); break; }
                    case 4: { det_turn(i, j, depth); break; }
                    case 5: { det_dama(i, j, depth); break; }
                    case 6: { det_rege(i, j, depth); break; }
                    default: break;
                }
    //selectia mutarilor corecte
    //evitarea schimbarii de 2 ori ajuta cu max 5%, nu mai conteaza
    i = 0;
    bool atac;
    while (i <= nr[depth])
    {
		//quiescence conditions: capturi, promovari, sahuri
		if (quiescence)
		{
			if (t[m[depth][i].r2][m[depth][i].c2] == 0 && (m[depth][i].extra < 2 && m[depth][i].extra > 5) && !atacat(x(-player), y(-player), -player))
			{
				m[depth][i] = m[depth][nr[depth]];
				nr[depth]--;
			}
			else i++;
		}
		else
		{
			schimbare(m[depth][i]);
			atac = atacat(x(player), y(player), player);
			schimbare_inversa(m[depth][i]);//nu pot sa sterg mutarea fara sa o fac inapoi
			if (atac)
			{
				m[depth][i] = m[depth][nr[depth]];
				nr[depth]--;
			}
			else i++;
		}
    }
    //terminare
    if (nr[depth] == -1)
    {
        if (atacat(x(player), y(player), player))//mat
        {
            if (player == 1) sfarsit = 2;//negru castiga
            else sfarsit = 1;//alb castiga
        }
        else sfarsit = 3;//pat
    }
    if (mutari50[nr_mutari3f] == 100) sfarsit = 4;//regula de 50 mutari
	if (nr_mutari3f >= 7)
	{
		if (Mutari3f[nr_mutari3f] == Mutari3f[nr_mutari3f - 4] && Mutari3f[nr_mutari3f - 1] == Mutari3f[nr_mutari3f - 5] &&
			Mutari3f[nr_mutari3f - 2] == Mutari3f[nr_mutari3f - 6] && Mutari3f[nr_mutari3f - 3] == Mutari3f[nr_mutari3f - 7])
			sfarsit = 5;//3 fold repetition
	}
    //lipsa de material
    for (i = 0; i <= 7; i++)
        for (j = 0; j <= 7; j++)
            if (abs(t[i][j]) == 2 || abs(t[i][j]) == 3) light++;
            else if (t[i][j] != 0) other++;
    if (other == 2 && light <= 1) sfarsit = 6;
    //sfarsitul vectorilor
    if (nr_mutari3f == 2 * nr_max3f - depth_max - 1) sfarsit = 7;
}

int DLL_EXPORT verificare_initializare(int tt[][8], int turn) {

	int n1 = 0, n2 = 0, i, j, ret = 0;
	bool b[5] = { false, false, true, true, true };
	//initializare matrice si cautare regi
	for (i = 0; i <= 7; i++)
		for (j = 0; j <= 7; j++)
		{
			t[i][j] = tt[i][j];
			if (t[i][j] == 6)
			{
				n1++;
				ra = i;
				ca = j;
			}
			if (t[i][j] == -6)
			{
				n2++;
				rn = i;
				cn = j;
			}
		}
	if (n1 == 1)
	{
		b[0] = true;
		if (!atacat(ra, ca, 1) || turn == 1) b[2] = false;
	}
	if (n2 == 1)
	{
		b[1] = true;
		if (!atacat(rn, cn, -1) || turn == -1) b[3] = false;
	}
	if (b[0] && b[1] && !b[2] && !b[3])
	{
		//initializare
		nr_mutari3f = -1;
		sfarsit = 0;
		//flaguri rocade
		if (t[0][0] == 4) turn_alb_mare = -1;
		else turn_alb_mare = -2;
		if (t[0][7] == 4) turn_alb_mic = -1;
		else turn_alb_mic = -2;
		if (t[0][4] == 6) rege_alb = -1;
		else rege_alb = -2;
		if (t[7][0] == -4) turn_negru_mare = -1;
		else turn_negru_mare = -2;
		if (t[7][7] == -4) turn_negru_mic = -1;
		else turn_negru_mic = -2;
		if (t[7][4] == -6) rege_negru = -1;
		else rege_negru = -2;
		//verific daca primul player are mutari posibile
		det_generala(0, turn, false);
		if (sfarsit == 0) b[4] = false;
	}

	for (int i = 0; i < 5; i++)
	{
		if (b[i])
			ret += (1 << i);
	}

	return ret;
}
bool DLL_EXPORT verificare_mutare(Mutare mm, int player) {
	//testeaza existenta mutarii introduse de om, daca exista o si executa
	if (sfarsit == 0)
	{
		//mutarile sunt deja determinate, inclusiv pentru prima
		for (short k = 0; k <= nr[0]; k++)
			if (m[0][k].r1 == mm.r1 && m[0][k].c1 == mm.c1 &&
				m[0][k].r2 == mm.r2 && m[0][k].c2 == mm.c2)
			{
				if (mm.extra > 1 && mm.extra < 6) schimbare(mm);//caz special pt transformare
				else schimbare(m[0][k]);//trebuie asa ca sa pastreze campul extra
				//verificare terminare meci
				det_generala(0, -player, false);
				return true;
			}
	}
	return false;
}

const short KnightRelPos[8][8]={//max=8
    { 2, 3, 4, 4, 4, 4, 3, 2},
    { 3, 4, 6, 6, 6, 6, 4, 3},
    { 4, 6, 8, 8, 8, 8, 6, 4},
    { 4, 6, 8, 8, 8, 8, 6, 4},
    { 4, 6, 8, 8, 8, 8, 6, 4},
    { 4, 6, 8, 8, 8, 8, 6, 4},
    { 3, 4, 6, 6, 6, 6, 4, 3},
    { 2, 3, 4, 4, 4, 4, 3, 2}
};
const short BishopRelPos[8][8]={//max=13
    { 7, 7, 7, 7, 7, 7, 7, 7},
    { 7, 9, 9, 9, 9, 9, 9, 7},
    { 7, 9,11,11,11,11, 9, 7},
    { 7, 9,11,13,13,11, 9, 7},
    { 7, 9,11,13,13,11, 9, 7},
    { 7, 9,11,11,11,11, 9, 7},
    { 7, 9, 9, 9, 9, 9, 9, 7},
    { 7, 7, 7, 7, 7, 7, 7, 7}
};
const short KnightRelPos2[8][8]={//max=56
    { 12,18,23,26,26,23,18,12},
    { 18,24,32,37,37,32,24,18},
    { 23,32,42,48,48,42,32,23},
    { 26,37,48,56,56,48,37,26},
    { 26,37,48,56,56,48,37,26},
    { 23,32,42,48,48,42,32,23},
    { 18,24,32,37,37,32,24,18},
    { 12,18,23,26,26,23,18,12}
};
const short BishopRelPos2[8][8]={//max=121
    { 73,67,63,61,61,63,67,73},
    { 67,85,81,79,79,81,85,67},
    { 63,81,101,99,99,101,81,63},
    { 61,79,99,121,121,99,79,61},
    { 61,79,99,121,121,99,79,61},
    { 63,81,101,99,99,101,81,63},
    { 67,85,81,79,79,81,85,67},
    { 73,67,63,61,61,63,67,73}
};
const short KingRelPos2[8][8]={//max=512
    { 105,183,220,233,233,220,183,105},
    { 183,318,382,404,404,382,318,183},
    { 220,382,459,485,485,459,382,220},
    { 233,404,485,512,512,485,404,233},
    { 233,404,485,512,512,485,404,233},
    { 220,382,459,485,485,459,382,220},
    { 183,318,382,404,404,382,318,183},
    { 105,183,220,233,233,220,183,105}
};
const short QweenRelPos2[8][8]={//max=317
    { 269,263,259,257,257,259,263,269},
    { 263,281,277,275,275,277,281,263},
    { 259,277,297,295,295,297,277,259},
    { 257,275,295,317,317,295,275,257},
    { 257,275,295,317,317,295,275,257},
    { 259,277,297,295,295,297,277,259},
    { 263,281,277,275,275,277,281,263},
    { 269,263,259,257,257,259,263,269}
};
const double PawnRelPos[8][8] = {//inlocuieste valoarea initiala, doar pozitie nu si structura
	{  9.0, 9.0, 9.0, 9.0, 9.0, 9.0, 9.0, 9.0},
	{ 3.6, 3.6, 3.6, 3.6, 3.6, 3.6, 3.6, 3.6},
	{ 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0},
	{ 1.2, 1.2, 1.2, 1.4, 1.4, 1.2, 1.2, 1.2},
	{ 1.0, 1.0, 1.2, 1.4, 1.4, 0.4, 0.8, 0.8},
	{ 1.0, 1.0, 0.8, 1.2, 1.2, 0.4, 0.8, 0.8},
	{ 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0},
	{ 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0}
};

double FileStatus(int c){
	short i, n1 = 0, n2 = 0;
    for (i = 0; i <= 7; i++)
    {
        if (t[i][c] == 1) n1++;
        if (t[i][c] == -1) n2++;
    }
    if (n1 == 0 && n2 == 0) return 1;//open file
    if (n1 == 0 || n2 == 0) return 0.9;//semi open file
    return 0.8;//closed file
}
double PawnRowValue(int r){
    switch (r)
    {
        case 4: return 0.1;
        case 5: return 1;
        case 6: return 3;
        default: return 0;
    }
}
bool IsBackward(int r, int c){
    //backward pawn inseamna ca toti pionii aliati alaturati sunt mai avansati
	for (int p = t[r][c]; 0 <= r && r <= 7; r += p)
	{
		if (c > 0) if (t[r][c - 1] == p) return false;
		if (c < 7) if (t[r][c + 1] == p) return false;
	}
    return true;
}

//foloseste deja pawn rel pos
double PawnStructureValue(int r, int c){
    //obs: turnul in spatele passedp are prea multe exceptii, nu are rost implementarea
    int i, p = t[r][c], nr1 = 0, nr2 = 0, nr3 = 0;
	double val = 1;//default value - valoare absoluta
	if (p > 0) val = PawnRelPos[7 - r][c];
	else val = PawnRelPos[r][c];
    //nr1 = nr pioni inamici care impiedica promovarea
	for(i = r + p; i >= 0 && i <= 7; i += p)
	{
		if (t[i][c] == -p) nr1++;
		if (c < 7) if(t[i][c + 1] == -p) nr1++;
		if (c > 0) if(t[i][c - 1] == -p) nr1++;
	}
	//nr2 = nr pioni aliati pe coloanele adiacente
	for(i = 0; i <= 7; i++)
	{
		if (c < 7) if(t[i][c + 1] == p) nr2++;
		if (c > 0) if(t[i][c - 1] == p) nr2++;
	}
	//nr3 = nr pioni aliati pe aceeasi coloana
	for(i = 0; i <= 7; i++)
		if(t[i][c] == p) nr3++;
    //evaluare valoare absoluta
    if(nr1 == 0)
    {
        val += 1;//passed pawn e avantajos
		if(c == 0 || c == 7) val += 0.2;//passed in margine e mai bun decat doar passed
    }
	if(nr3 != 1) val -= 0.6; //multiplu, doar o data ca oricum se aplica pe fiecare dintre ei, evident nu e bun
	if(IsBackward(r, c)) val -= 0.2; //backward pawn nu poate fi avansat in siguranta si poate fi blocat
	if (nr2 < 2) val -= 0.2 * (2 - nr2); //izolat nu e bun
	return val;
}

//trebuie incercat si move position
double evaluare_random(int depth){
	//evaluarea se face random
    return rand() % 20;
}
double evaluare_basic(int depth){
	//doar suma valorilor pieselor
    double val = 0;
    for (int i = 0; i <= 7; i++)
        for (int j = 0; j <= 7; j++)
            switch (t[i][j])
            {
                case 1:
                case -1: { val += sgn(t[i][j]) * 1; break; }
                case 2:
                case -2: { val += sgn(t[i][j]) * 3.5; break; }
                case 3:
                case -3: { val += sgn(t[i][j]) * 3.5; break; }
                case 4:
                case -4: { val += sgn(t[i][j]) * 5; break; }
                case 5:
                case -5: { val += sgn(t[i][j]) * 10; break; }
                default: break;
            }
    //evaluare sfarsit, trebuie facut la final ca sa aibe sens remiza
    if (sfarsit != 0)
    {
        if (sfarsit == 1) val = 1000;
        else if (sfarsit == 2) val = -1000;
        else val = -500 * sgn(val);
    }
    return val;
}
double evaluare_moves(int depth) {
	//suma dintre valorile pieselor si mutarile posibile (pt o culoare) - ar trebui pt amandoua
	//vag mai slaba decat position1
	int aux, i, j;
	double val = 0;
	//valoarea de baza
	for (i = 0; i <= 7; i++)
		for (j = 0; j <= 7; j++)
			switch (t[i][j])
			{
			case 1:
			case -1: { val += sgn(t[i][j]) * 0.53; break; }
			case 2:
			case -2: { val += sgn(t[i][j]) * 2.63; break; }
			case 3:
			case -3: { val += sgn(t[i][j]) * 2.59; break; }
			case 4:
			case -4: { val += sgn(t[i][j]) * 4.07; break; }
			case 5:
			case -5: { val += sgn(t[i][j]) * 9.04; break; }
			case 6:
			case -6:
			default: break;
			}
	//valorile numar mutari
	for (i = 0;i <= nr[depth];i++)
	{
		aux = t[m[depth][i].r1][m[depth][i].c1];
		switch (aux)
		{
		case 1:
		case -1: { val += sgn(aux) * 0.23; break; }
		case 2:
		case -2: { val += sgn(aux) * 0.13; break; }
		case 3:
		case -3: { val += sgn(aux) * 0.09; break; }
		case 4:
		case -4: { val += sgn(aux) * 0.07; break; }
		case 5:
		case -5: { val += sgn(aux) * 0.04; break; }
		case 6:
		case -6: { val += sgn(aux) * 0.01; break; }
		default: break;
		}

	}
	//evaluare sfarsit, trebuie facut la final ca sa aibe sens remiza
	if (sfarsit != 0)
	{
		if (sfarsit == 1) val = 1000;
		else if (sfarsit == 2) val = -1000;
		else val = -500 * sgn(val);
	}
	return val;
}
double evaluare_position(int depth){
    //suma dintre valorile pieselor si pozitiile lor
	//vag mai buna decat moves
    double val = 0;
    for (int i = 0; i <= 7; i++)
        for (int j = 0; j <= 7; j++)
            switch (t[i][j])
            {
                case 1: { val += sgn(t[i][j]) + PawnRowValue(i); break; }
                case -1: { val += sgn(t[i][j]) + PawnRowValue(7 - i); break; }
                case 2:
                case -2: { val += sgn(t[i][j]) * (3.5 + KnightRelPos[i][j] / 8); break; }// /8
                case 3:
                case -3: { val += sgn(t[i][j]) * (3.5 + BishopRelPos[i][j] / 13); break; }// /13
                case 4:
                case -4: { val += sgn(t[i][j]) * 5; break; }
                case 5:
                case -5: { val += sgn(t[i][j]) * (10 + (14 + BishopRelPos[i][j]) / 27); break; }// /27
                default: break;
            }
    //evaluare sfarsit, trebuie facut la final ca sa aibe sens remiza
    if (sfarsit != 0)
    {
        if (sfarsit == 1) val = 1000;
        else if (sfarsit == 2) val = -1000;
        else val = -500 * sgn(val);
    }
    return val;
}
double evaluare_position2(int depth){
	//suma dintre valorile pieselor si pozitiile lor, considerand pozitiile cu numar maxim de nivele
    double val = 0;
    for (int i = 0; i <= 7; i++)
        for (int j = 0; j <= 7; j++)
            switch (t[i][j])
            {
                case 1: { val += sgn(t[i][j]) + PawnRowValue(i); break; }
                case -1: { val += sgn(t[i][j]) + PawnRowValue(7 - i); break; }
                case 2:
                case -2: { val += sgn(t[i][j]) * (3.5 + KnightRelPos2[i][j] / 56); break; }// /56
                case 3:
                case -3: { val += sgn(t[i][j]) * (3.5 + BishopRelPos2[i][j] / 121); break; }// /121
                case 4:
                case -4: { val += sgn(t[i][j]) * 5; break; }// /196
                case 5:
                case -5: { val += sgn(t[i][j]) * (10 + QweenRelPos2[i][j] / 317); break; }// /317
                case 6:
                case -6: { val += sgn(t[i][j]) * KingRelPos2[i][j] / 512; break; } //512
                default: break;
            }
    //evaluare sfarsit, trebuie facut la final ca sa aibe sens remiza
    if (sfarsit != 0)
    {
        if (sfarsit == 1) val = 1000;
        else if (sfarsit == 2) val = -1000;
        else val = -500 * sgn(val);
    }
    return val;
}
double evaluare_position3(int depth) {
	//suma dintre valorile pieselor si pozitiile lor, considerand pozitiile cu numar maxim de nivele
	//se foloseste in plus si pozitia pionilor, mai ales pentru deschidere
	double val = 0;
	for (int i = 0; i <= 7; i++)
		for (int j = 0; j <= 7; j++)
			switch (t[i][j])
			{
			case 1: { val += sgn(t[i][j]) * PawnRelPos[7 - i][j]; break; }
			case -1: { val += sgn(t[i][j]) * PawnRelPos[i][j]; break; }
			case 2:
			case -2: { val += sgn(t[i][j]) * (3.5 + KnightRelPos2[i][j] / 56); break; }// /56
			case 3:
			case -3: { val += sgn(t[i][j]) * (3.5 + BishopRelPos2[i][j] / 121); break; }// /121
			case 4:
			case -4: { val += sgn(t[i][j]) * 5 * FileStatus(j); break; }// /196
			case 5:
			case -5: { val += sgn(t[i][j]) * (10 + QweenRelPos2[i][j] / 317); break; }// /317
			case 6:
			case -6: { val += sgn(t[i][j]) * KingRelPos2[i][j] / 512; break; } //512
			default: break;
			}
	//evaluare sfarsit, trebuie facut la final ca sa aibe sens remiza
	if (sfarsit != 0)
	{
		if (sfarsit == 1) val = 1000;
		else if (sfarsit == 2) val = -1000;
		else val = -500 * sgn(val);
	}
	return val;
}
double evaluare_position4(int depth) {
	//suma dintre valorile pieselor si pozitiile lor, considerand pozitiile cu numar maxim de nivele
	//se foloseste in plus si pozitia si structura, mai ales pentru deschidere
	double val = 0;
	for (int i = 0; i <= 7; i++)
		for (int j = 0; j <= 7; j++)
			switch (t[i][j])
			{
			case 1: 
			case -1: { val += sgn(t[i][j]) * PawnStructureValue(i, j); break; } //asta incorporeaza pawn rel value 
			case 2:
			case -2: { val += sgn(t[i][j]) * (3.5 + KnightRelPos2[i][j] / 56); break; }// /56
			case 3:
			case -3: { val += sgn(t[i][j]) * (3.5 + BishopRelPos2[i][j] / 121); break; }// /121
			case 4:
			case -4: { val += sgn(t[i][j]) * 5; break; }// /196  // * FileStatus(j)
			case 5:
			case -5: { val += sgn(t[i][j]) * (10 + QweenRelPos2[i][j] / 317); break; }// /317
			case 6:
			case -6: { val += sgn(t[i][j]) * KingRelPos2[i][j] / 512; break; } //512
			default: break;
			}
	//evaluare sfarsit, trebuie facut la final ca sa aibe sens remiza
	if (sfarsit != 0)
	{
		if (sfarsit == 1) 
			val = 1000;
		else if (sfarsit == 2) 
			val = -1000;
		else 
			val = -500 * sgn(val);
	}
	val = -val;
	val = -val;
	return val;
}
double evaluare_position5(int depth) {
	//suma dintre valorile pieselor si pozitiile lor, considerand pozitiile cu numar maxim de nivele
	//se foloseste in plus si pozitia si structura, mai ales pentru deschidere
	double val = 0;
	for (int i = 0; i <= 7; i++)
		for (int j = 0; j <= 7; j++)
			switch (t[i][j])
			{
			case 1:
			case -1: { val += sgn(t[i][j]) * PawnStructureValue(i, j); break; } //asta incorporeaza pawn rel value 
			case 2:
			case -2: { val += sgn(t[i][j]) * (3.5 + KnightRelPos2[i][j] / 56.0); break; }// /56
			case 3:
			case -3: { val += sgn(t[i][j]) * (3.5 + BishopRelPos2[i][j] / 121.0); break; }// /121
			case 4:
			case -4: { val += sgn(t[i][j]) * 5; break; }// /196  // * FileStatus(j)
			case 5:
			case -5: { val += sgn(t[i][j]) * (10 + QweenRelPos2[i][j] / 317.0); break; }// /317
			case 6:
			case -6: { val += sgn(t[i][j]) * KingRelPos2[i][j] / 512.0; break; } //512
			default: break;
			}
	//evaluare sfarsit, trebuie facut la final ca sa aibe sens remiza
	if (sfarsit != 0)
	{
		if (sfarsit == 1) val = 1000;
		else if (sfarsit == 2) val = -1000;
		else val = -500 * sgn(val);
	}
	return val;
}
double evaluare_move_position(int depth) {
	//se incearca extinderea partiala pornind de la position 4
	double val = 0, val2 = 0, val3;
	int i, j, k;
	for (i = 0; i <= 7; i++)
		for (j = 0; j <= 7; j++)
			switch (t[i][j])
			{
			case 1:
			case -1: { val += sgn(t[i][j]) * PawnStructureValue(i, j); break; } //asta incorporeaza pawn rel value 
			case 2:
			case -2: { val += sgn(t[i][j]) * (3.5 + KnightRelPos2[i][j] / 56); break; }// /56
			case 3:
			case -3: { val += sgn(t[i][j]) * (3.5 + BishopRelPos2[i][j] / 121); break; }// /121
			case 4:
			case -4: { val += sgn(t[i][j]) * 5 * FileStatus(j); break; }// /196
			case 5:
			case -5: { val += sgn(t[i][j]) * (10 + QweenRelPos2[i][j] / 317); break; }// /317
			case 6:
			case -6: { val += sgn(t[i][j]) * KingRelPos2[i][j] / 512; break; } //512
			default: break;
			}
	//extindere partiala
	for (k = 0; k <= nr[depth]; k++)
	{
		//val3 e auxiliar si calculez maximul in val2
		i = m[depth][k].r2;
		j = m[depth][k].c2;
		//initializare cu captura camp final nu merge, deci incep cu mutarea pe campul final
		//mutare piesa pe campul final
		switch (t[m[depth][k].r1][m[depth][k].c1])
		{
		case 1: { val3 = PawnRelPos[7 - i][j]; break; } //ma complic daca fac cu struct
		case -1: { val3 = PawnRelPos[i][j]; break; } //ma complic daca fac cu struct
		case 2:
		case -2: { val3 = KnightRelPos2[i][j] / 56; break; }// /56
		case 3:
		case -3: { val3 = BishopRelPos2[i][j] / 121; break; }// /121
		case 4:
		case -4: { val3 = 5 * FileStatus(j); break; }// /196
		case 5:
		case -5: { val3 = QweenRelPos2[i][j] / 317; break; }// /317
		case 6:
		case -6: { val3 = KingRelPos2[i][j] / 512; break; } //512
		default:  break;
		}
		//mutare piesa de pe campul initial
		i = m[depth][k].r1;
		j = m[depth][k].c1;
		switch (t[i][j])
		{
		case 1: { val3 -= PawnRelPos[7 - i][j]; break; } //ma complic daca fac cu struct
		case -1: { val3 -= PawnRelPos[i][j]; break; } //ma complic daca fac cu struct
		case 2:
		case -2: { val3 -= KnightRelPos2[i][j] / 56; break; }// /56
		case 3:
		case -3: { val3 -= BishopRelPos2[i][j] / 121; break; }// /121
		case 4:
		case -4: { val3 -= 5 * FileStatus(j); break; }// /196
		case 5:
		case -5: { val3 -= QweenRelPos2[i][j] / 317; break; }// /317
		case 6:
		case -6: { val3 -= KingRelPos2[i][j] / 512; break; } //512
		default: break;
		}
		//determinare maxim
		if (val3 > val2) val2 = val3;
	}
	val -= sgn(t[i][j]) * val2;
	//evaluare sfarsit, trebuie facut la final ca sa aibe sens remiza
	if (sfarsit != 0)
	{
		if (sfarsit == 1) val = 1000;
		else if (sfarsit == 2) val = -1000;
		else val = -500 * sgn(val);
	}
	return val;
}

int comparare(const void *a, const void *b){
	if (((Mutare*)a)->val < ((Mutare*)b)->val)return -1;
	if (((Mutare*)a)->val > ((Mutare*)b)->val)return 1;
	return 0;
}
Mutare alphabeta(double alfa, double beta, int player, double nrmax, int depth, int depth_m)
{
	int best_index = -1, i;
	double best = -2000 * player;

	//determinare mutari si sfarsit, aici RTable e folosit pentru quiescence search
	//if(RTable && depth>1) det_generala(depth, player, true);
	//else 
		det_generala(depth, player, false);

	//daca am ajuns la o frunza returnez evaluarea
	//if (depth == depth_m || sfarsit != 0) return new Mutare(evaluare_generala(depth));
	//if (nrmax < 1 || sfarsit != 0) return new Mutare(evaluare_generala(ref m, ref nr));
	//if ((depth >= depth_m && nrmax < 1) || sfarsit != 0) return new Mutare(evaluare_generala(depth));
	if ((depth >= depth_m && (nrmax < 1 || !dinamic)) || sfarsit != 0) return Mutare(evaluare_generala(depth));

	//evaluare partiala si sortare mutari -----> numai pt prima adancime
	/*if (depth == 0)
	{
		//determinarea principiului de sortare (evaluare partiala)
		for (i = 0;i <= nr[depth];i++)
		{
			schimbare(m[depth][i]);
			//m[depth][i].val = evaluare_generala(depth);
			m[depth][i].val = alphabeta(alfa, beta, -player, 30, depth + 1, depth + 2).val;
			schimbare_inversa(m[depth][i]);
		}
		//sortare
		qsort(m[depth], nr[depth], sizeof(Mutare), comparare);
	}*/

	//alphabeta normal
	for (i = 0; i <= nr[depth]; i++)
	{
		schimbare(m[depth][i]);
		m[depth][i].val = alphabeta(alfa, beta, -player, nrmax / (nr[depth] + 2), depth + 1, depth_m).val;
		if (m[depth][i].val == player * 1000) m[depth][i].val -= depth * player;//trebuie pentru cel mai scurt mat
		if (player == 1)
		{
			if (m[depth][i].val > best || (m[depth][i].val == best))// && rand() % 100 < 25))
			{
				best_index = i;
				best = m[depth][i].val;
			}
			alfa = max(alfa, best);
		}
		else
		{
			if (m[depth][i].val < best || (m[depth][i].val == best))// && rand() % 100 < 25))
			{
				best_index = i;
				best = m[depth][i].val;
			}
			beta = min(beta, best);
		}
		schimbare_inversa(m[depth][i]);
		//if (alfa > beta) break;
	}
	return m[depth][best_index];
}

Mutare DLL_EXPORT AIGeneral(int player, int leaf_depth, int evaluare, bool d, bool RT) {
	if (sfarsit == 0)
	{
		int breadth = 28;
		srand(time(NULL));
		dinamic = d;
		RTable = RT;
		switch (evaluare)
		{
		default: { throw "Error! Undefined evaluation function!";}
		case 1: {evaluare_generala = evaluare_random; break;}
		case 2: { evaluare_generala = evaluare_basic; break;}
		case 3: { evaluare_generala = evaluare_moves; break;}
		case 4: { evaluare_generala = evaluare_position; break;}
		case 5: { evaluare_generala = evaluare_position2; break;}
		case 6: { evaluare_generala = evaluare_position3; break;}
		case 7: { evaluare_generala = evaluare_position4; break;}
		case 8: { evaluare_generala = evaluare_position5; break;}
		case 9: { evaluare_generala = evaluare_move_position; break;}
		}
		Mutare m1 = alphabeta(-2000, 2000, player, pow(breadth, leaf_depth), 0, leaf_depth);
		schimbare(m1);
		//verificare terminare meci
		det_generala(0, -player, false);
		return m1;
	}
	return Mutare(-1, -1, -1, -1, -1);
}
