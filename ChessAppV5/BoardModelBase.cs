using System;
using System.Collections.Generic;

namespace ChessAppV5
{
    public partial class BoardModel
    {
        //trebuie pt en passant si afisarea ultimelor mutari
        public Mutare lm
        {
            get
            {
                if (Mutari3f != null && nr_mutari3f != -1)
                    return Mutari3f[nr_mutari3f];
                return new Mutare(-1, -1, -1, -1, -1);
            }
        }
        public Mutare lm_alb
        {
            get
            {
                if (Mutari3f != null && nr_mutari3f != -1)
                {
                    if (t[lm.r2, lm.c2] > 0) return lm;
                    else if (nr_mutari3f > 0) return Mutari3f[nr_mutari3f - 1];
                }
                return new Mutare(-1, -1, -1, -1, -1);
            }
        }
        public Mutare lm_negru
        {
            get
            {
                if (Mutari3f != null && nr_mutari3f != -1)
                {
                    if (t[lm.r2, lm.c2] < 0) return lm;
                    else if (nr_mutari3f > 0) return Mutari3f[nr_mutari3f - 1];
                }
                return new Mutare(-1, -1, -1, -1, -1);
            }
        }
        public int sfarsit { get; private set; }
        public int nr_mutari3f { get; private set; }

        protected static readonly int nr_max = 321, nr_max3f = 200, depth_max = 20;//parametrii globali
        protected int[,] t = new int[8, 8];//piesele sunt de la -6 la +6
        protected Mutare[] Mutari3f;//lista de mutari efectuate, necesar pt recursiv si undo
        protected List<Mutare[]> m;//lista cu mutarile posibile in functie de depth, prealocate
        protected int[] piese;//piesele luate, necesar pt mutarile inverse
        protected int[] nr;//nr mutari posibile la fiecare depth, prealocate
        protected int[] mutari50;//la fiecare mutare cate mutari au ramas catre regula de 50, necesar pt recursiv si undo
        //coordonatele regelui in functie de culoare
        private int x1, y1, x2, y2;//coordonatele regilor
        private int x(int player)
        {
            if (player == 1) return x1;
            return x2;
        }
        private int y(int player)
        {
            if (player == 1) return y1;
            return y2;
        }
        //pt rocade, indexul primei mutari
        private int rege_alb, rege_negru, turn_alb_mare, turn_alb_mic, turn_negru_mare, turn_negru_mic;//-1 inseamna ca nu a mutat
        //protected short i, j, k;//iteratori impliciti prealocati
        //protected int p0, p1, p2;
        
        protected void det_generala(int depth, int player)
        {
            int i, j;
            nr[depth] = -1;
            for (i = 0; i <= 7; i++)
                for (j = 0; j <= 7; j++)
                    if (t[i, j] * player > 0)
                        switch (Math.Abs(t[i, j]))
                        {
                            case 0: break;
                            case 1: { det_pion(i, j, depth); break; }
                            case 2: { det_cal(i, j, depth); break; }
                            case 3: { det_nebun(i, j, depth); break; }
                            case 4: { det_turn(i, j, depth); break; }
                            case 5: { det_dama(i, j, depth); break; }
                            case 6: { det_rege(i, j, depth); break; }
                            default: { throw new Exception("Eroare! Piesa nu exista!"); }
                        }
            //selectia mutarilor corecte
            //evitarea schimbarii de 2 ori ajuta cu max 5%, nu mai conteaza
            i = 0;
            bool atac;
            while (i <= nr[depth])
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
            if (mutari50[depth] == 100) sfarsit = 4;//regula de 50 mutari
            if (nr_mutari3f >= 7)
            {
                if (Mutari3f[nr_mutari3f] == Mutari3f[nr_mutari3f - 4] && Mutari3f[nr_mutari3f - 1] == Mutari3f[nr_mutari3f - 5] &&
                        Mutari3f[nr_mutari3f - 2] == Mutari3f[nr_mutari3f - 6] && Mutari3f[nr_mutari3f - 3] == Mutari3f[nr_mutari3f - 7])
                    sfarsit = 5;//3 fold repetition
            }
            //lipsa de material
            int other = 0, light = 0;
            for (i = 0; i <= 7; i++)
                for (j = 0; j <= 7; j++)
                    if (Math.Abs(t[i, j]) == 2 || Math.Abs(t[i, j]) == 3) light++;
                    else if (t[i, j] != 0) other++;
            if (other == 2 && light <= 1) sfarsit = 6;
            //sfarsitul vectorilor
            if (nr_mutari3f == 2 * nr_max3f - depth_max - 1) sfarsit = 7;
        }
        private void det_pion(int r, int c, int depth)
        {
            //mutare in fata
            if (t[r + t[r, c], c] == 0)
                transformare(r, c, r + t[r, c], c, depth);
            //captura
            if (c > 0)
                if (t[r + t[r, c], c - 1] * t[r, c] < 0)
                    transformare(r, c, r + t[r, c], c - 1, depth);
            if (c < 7)
                if (t[r + t[r, c], c + 1] * t[r, c] < 0)
                    transformare(r, c, r + t[r, c], c + 1, depth);
            //mutare dubla
            int p0 = (int)(3.5 - 0.5 * t[r, c]);//util si pt mutarea dubla si pt en passant
            if (r == p0 - 2 * t[r, c] && t[p0 - t[r, c], c] == 0 && t[p0, c] == 0)
            {
                nr[depth]++;
                m[depth][nr[depth]] = new Mutare(p0 - 2 * t[r, c], c, p0, c);
            }
            //en_passant
            if (r == p0 + t[r, c] && lm.extra != -1)
            {
                if (Math.Abs(t[lm.r2, lm.c2]) == 1 && Math.Abs(lm.r2 - lm.r1) == 2)
                {
                    if (c - 1 == lm.c2)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(p0 + t[r, c], c, p0 + 2 * t[r, c], c - 1, 1);
                    }
                    if (c + 1 == lm.c2)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(p0 + t[r, c], c, p0 + 2 * t[r, c], c + 1, 1);
                    }
                }
            }
        }
        private void transformare(int r1, int c1, int r2, int c2, int depth)
        {
            nr[depth]++;
            if (r2 == 3.5 + 3.5 * t[r1, c1])
            {
                m[depth][nr[depth]] = new Mutare(r1, c1, r2, c2, 2);
                m[depth][nr[depth] + 1] = new Mutare(r1, c1, r2, c2, 3);
                m[depth][nr[depth] + 2] = new Mutare(r1, c1, r2, c2, 4);
                m[depth][nr[depth] + 3] = new Mutare(r1, c1, r2, c2, 5);
                nr[depth] = nr[depth] + 3;
            }
            else m[depth][nr[depth]] = new Mutare(r1, c1, r2, c2);
        }
        private void det_cal(int r, int c, int depth)
        {
            if (r < 6)
            {
                if (c > 0)
                    if (t[r + 2, c - 1] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r + 2, c - 1);
                    }
                if (c < 7)
                    if (t[r + 2, c + 1] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r + 2, c + 1);
                    }
            }
            if (r > 1)
            {
                if (c > 0)
                    if (t[r - 2, c - 1] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r - 2, c - 1);
                    }
                if (c < 7)
                    if (t[r - 2, c + 1] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r - 2, c + 1);
                    }
            }
            if (r < 7)
            {
                if (c > 1)
                    if (t[r + 1, c - 2] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r + 1, c - 2);
                    }
                if (c < 6)
                    if (t[r + 1, c + 2] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r + 1, c + 2);
                    }
            }
            if (r > 0)
            {
                if (c > 1)
                    if (t[r - 1, c - 2] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r - 1, c - 2);
                    }
                if (c < 6)
                    if (t[r - 1, c + 2] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r - 1, c + 2);
                    }
            }
        }
        private void det_nebun(int r, int c, int depth)
        {
            int k;
            for (k = 1; r + k <= 7 && c + k <= 7; k++)
            {
                if (t[r + k, c + k] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r + k, c + k);
                }
                if (t[r + k, c + k] != 0) break;
            }
            for (k = 1; r + k <= 7 && c - k >= 0; k++)
            {
                if (t[r + k, c - k] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r + k, c - k);
                }
                if (t[r + k, c - k] != 0) break;
            }
            for (k = 1; r - k >= 0 && c + k <= 7; k++)
            {
                if (t[r - k, c + k] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r - k, c + k);
                }
                if (t[r - k, c + k] != 0) break;
            }
            for (k = 1; r - k >= 0 && c - k >= 0; k++)
            {
                if (t[r - k, c - k] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r - k, c - k);
                }
                if (t[r - k, c - k] != 0) break;
            }
        }
        private void det_turn(int r, int c, int depth)
        {
            int k;
            for (k = 1; c + k <= 7; k++)
            {
                if (t[r, c + k] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r, c + k);
                }
                if (t[r, c + k] != 0) break;
            }
            for (k = 1; c - k >= 0; k++)
            {
                if (t[r, c - k] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r, c - k);
                }
                if (t[r, c - k] != 0) break;
            }
            for (k = 1; r + k <= 7; k++)
            {
                if (t[r + k, c] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r + k, c);
                }
                if (t[r + k, c] != 0) break;
            }
            for (k = 1; r - k >= 0; k++)
            {
                if (t[r - k, c] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r - k, c);
                }
                if (t[r - k, c] != 0) break;
            }
        }
        private void det_dama(int r, int c, int depth)
        {
            det_nebun(r, c, depth);
            det_turn(r, c, depth);
        }
        private void det_rege(int r, int c, int depth)
        {
            int p0 = Math.Sign(t[r, c]);
            #region mutari normale
            if (r < 7)
            {
                if (t[r + 1, c] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r + 1, c);
                }
                if (c > 0)
                    if (t[r + 1, c - 1] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r + 1, c - 1);
                    }
                if (c < 7)
                    if (t[r + 1, c + 1] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r + 1, c + 1);
                    }
            }
            if (r > 0)
            {
                if (t[r - 1, c] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r - 1, c);
                }
                if (c > 0)
                    if (t[r - 1, c - 1] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r - 1, c - 1);
                    }
                if (c < 7)
                    if (t[r - 1, c + 1] * t[r, c] <= 0)
                    {
                        nr[depth]++;
                        m[depth][nr[depth]] = new Mutare(r, c, r - 1, c + 1);
                    }
            }
            if (c > 0)
                if (t[r, c - 1] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r, c - 1);
                }
            if (c < 7)
                if (t[r, c + 1] * t[r, c] <= 0)
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(r, c, r, c + 1);
                }
            #endregion
            #region rocade
            if (t[r, c] > 0 && rege_alb == -1 && !atacat(0, 4, p0))//rocade albe
            {
                //rocada mica
                if (turn_alb_mic == -1 && t[0, 5] == 0 && t[0, 6] == 0 && !atacat(0, 5, p0))
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(0, 4, 0, 6, 7);
                }
                //rocada mare
                if (turn_alb_mare == -1 && t[0, 1] == 0 && t[0, 2] == 0 && t[0, 3] == 0 && !atacat(0, 3, p0))
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(0, 4, 0, 2, 6);
                }
            }
            if (t[r, c] < 0 && rege_negru == -1 && !atacat(7, 4, p0))//rocade negre
            {
                //rocada mica
                if (turn_negru_mic == -1 && t[7, 5] == 0 && t[7, 6] == 0 && !atacat(7, 5, p0))
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(7, 4, 7, 6, 9);
                }
                //rocada mare
                if (turn_negru_mare == -1 && t[7, 1] == 0 && t[7, 2] == 0 && t[7, 3] == 0 && !atacat(7, 3, p0))
                {
                    nr[depth]++;
                    m[depth][nr[depth]] = new Mutare(7, 4, 7, 2, 8);
                }
            }
            #endregion
        }
        
        protected bool atacat(int r, int c, int p)
        {
            /* e folosita pt verificarea validitatii mutarilor umane, deci nu e nevoie de en passant
             * si pt rocada deci nu exista mereu piesa la (r,c) si e nevoie de culoarea opusa*/
            if (atacat_de_rege(r, c, p)) return true;
            if (atacat_de_pion(r, c, p)) return true;
            if (atacat_de_cal(r, c, p)) return true;
            if (atacat_diagonala(r, c, p)) return true;
            if (atacat_linie(r, c, p)) return true;
            return false;
        }
        private bool atacat_de_pion(int r, int c, int p)
        {
            if (p * r < 3.5 + p * 3.5)
            {
                if (c < 7) if (t[r + p, c + 1] == -p) return true;
                if (c > 0) if (t[r + p, c - 1] == -p) return true;
            }
            return false;
        }
        private bool atacat_de_cal(int r, int c, int p)
        {
            p = -2 * p;
            if (r < 6)
            {
                if (c < 7) if (t[r + 2, c + 1] == p) return true;
                if (c > 0) if (t[r + 2, c - 1] == p) return true;
            }
            if (r > 1)
            {
                if (c < 7) if (t[r - 2, c + 1] == p) return true;
                if (c > 0) if (t[r - 2, c - 1] == p) return true;
            }
            if (r < 7)
            {
                if (c < 6) if (t[r + 1, c + 2] == p) return true;
                if (c > 1) if (t[r + 1, c - 2] == p) return true;
            }
            if (r > 0)
            {
                if (c < 6) if (t[r - 1, c + 2] == p) return true;
                if (c > 1) if (t[r - 1, c - 2] == p) return true;
            }
            return false;
        }
        private bool atacat_diagonala(int r, int c, int p)
        {
            int p1 = -3 * p, p2 = -5 * p, k;
            for (k = 1; r + k <= 7 && c + k <= 7; k++)
            {
                if (t[r + k, c + k] == p1 || t[r + k, c + k] == p2) return true;
                if (t[r + k, c + k] != 0) break;
            }
            for (k = 1; r + k <= 7 && c - k >= 0; k++)
            {
                if (t[r + k, c - k] == p1 || t[r + k, c - k] == p2) return true;
                if (t[r + k, c - k] != 0) break;
            }
            for (k = 1; r - k >= 0 && c + k <= 7; k++)
            {
                if (t[r - k, c + k] == p1 || t[r - k, c + k] == p2) return true;
                if (t[r - k, c + k] != 0) break;
            }
            for (k = 1; r - k >= 0 && c - k >= 0; k++)
            {
                if (t[r - k, c - k] == p1 || t[r - k, c - k] == p2) return true;
                if (t[r - k, c - k] != 0) break;
            }
            return false;
        }
        private bool atacat_linie(int r, int c, int p)
        {
            int p1 = -4 * p, p2 = -5 * p, k;
            for (k = 1; c + k <= 7; k++)
            {
                if (t[r, c + k] == p1 || t[r, c + k] == p2) return true;
                if (t[r, c + k] != 0) break;
            }
            for (k = 1; c - k >= 0; k++)
            {
                if (t[r, c - k] == p1 || t[r, c - k] == p2) return true;
                if (t[r, c - k] != 0) break;
            }
            for (k = 1; r + k <= 7; k++)
            {
                if (t[r + k, c] == p1 || t[r + k, c] == p2) return true;
                if (t[r + k, c] != 0) break;
            }
            for (k = 1; r - k >= 0; k++)
            {
                if (t[r - k, c] == p1 || t[r - k, c] == p2) return true;
                if (t[r - k, c] != 0) break;
            }
            return false;
        }
        private bool atacat_de_rege(int r, int c, int p)
        {
            if (Math.Abs(x1 - x2) < 2 && Math.Abs(y1 - y2) < 2) return true;
            return false;
        }

        public bool[] verificare_initializare(int[,] tt, int turn)//verificare si initializare
        {
            bool[] b = { false, false, true, true, true };
            /* bitul 0 indica daca exista exact un rege alg, bitul 1 analog pt negru
             * bitul 2 indica daca regele alb poate fi capturat in prima tura, bitul 3 analog pt negru
             bitul 4 indica daca primul player nu are mutari posibile*/

            int n1 = 0, n2 = 0, i, j;//nr de regi

            //initializare matrice si cautare regi
            for (i = 0; i <= 7; i++)
                for (j = 0; j <= 7; j++)
                {
                    t[i, j] = tt[i, j];
                    if (t[i, j] == 6)
                    {
                        n1++;
                        x1 = i;
                        y1 = j;
                    }
                    if (t[i, j] == -6)
                    {
                        n2++;
                        x2 = i;
                        y2 = j;
                    }
                }
            if (n1 == 1)
            {
                b[0] = true;
                if (!atacat(x1, y1, Math.Sign(t[x1, y1])) || turn == 1) b[2] = false;
            }
            if (n2 == 1)
            {
                b[1] = true;
                if (!atacat(x2, y2, Math.Sign(t[x2, y2])) || turn == -1) b[3] = false;
            }
            if (b[0] && b[1] && !b[2] && !b[3])
            {
                //initializare
                Mutari3f = new Mutare[2 * nr_max3f];//dublu ca pastrez pt ambii jucatori => max 200 mutari/meci
                m = new List<Mutare[]>();
                for (i = 0; i < depth_max; i++)
                    m.Add(new Mutare[nr_max]);
                nr = new int[depth_max];
                piese = new int[2 * nr_max3f];
                mutari50 = new int[2 * nr_max3f];//nr maxim mutari / meci
                mutari50[0] = 1;
                nr_mutari3f = -1;
                sfarsit = 0;
                //flaguri rocade
                if (t[0, 0] == 4) turn_alb_mare = -1;
                else turn_alb_mare = -2;
                if (t[0, 7] == 4) turn_alb_mic = -1;
                else turn_alb_mic = -2;
                if (t[0, 4] == 6) rege_alb = -1;
                else rege_alb = -2;
                if (t[7, 0] == -4) turn_negru_mare = -1;
                else turn_negru_mare = -2;
                if (t[7, 7] == -4) turn_negru_mic = -1;
                else turn_negru_mic = -2;
                if (t[7, 4] == -6) rege_negru = -1;
                else rege_negru = -2;
                //verific daca primul player are mutari posibile
                det_generala(0, turn);
                if (sfarsit == 0) b[4] = false;
            }
            return b;
        }
        public bool verificare_mutare(Mutare mm, int player)
        {
            //testeaza existenta mutarii introduse de om, daca exista o si executa
            if (sfarsit == 0)
            {
                //mutarile sunt deja determinate, inclusiv pentru prima
                for (int k = 0; k <= nr[0]; k++)
                    if (m[0][k].r1 == mm.r1 && m[0][k].c1 == mm.c1 &&
                            m[0][k].r2 == mm.r2 && m[0][k].c2 == mm.c2)
                    {
                        //alegere piesa in care sa transforme pionul
                        if (1 < m[0][k].extra && m[0][k].extra < 6)
                        {
                            //Transformare t = new Transformare();
                            //etc
                        }
                        schimbare(m[0][k]);
                        //verificare terminare meci
                        det_generala(0, -player);
                        return true;
                    }
            }
            return false;
        }
        
        protected void schimbare(Mutare m)
        {
            nr_mutari3f++;
            //regula de 50 mutari
            if (Math.Abs(t[m.r1, m.c1]) == 1 || t[m.r2, m.c2] != 0) mutari50[nr_mutari3f] = 1;
            else if (nr_mutari3f > 0) mutari50[nr_mutari3f - 1] += 1;
            //mutare normala
            Mutari3f[nr_mutari3f] = m;
            piese[nr_mutari3f] = t[m.r2, m.c2];
            t[m.r2, m.c2] = t[m.r1, m.c1];
            t[m.r1, m.c1] = 0;
            //en passant
            if (m.extra == 1) t[m.r1, m.c2] = 0;
            //transformare
            if (1 < m.extra && m.extra < 6) t[m.r2, m.c2] *= m.extra;
            //rocadele
            if (m.extra == 6) { t[m.r1, 3] = t[m.r1, 0]; t[m.r1, 0] = 0; }
            if (m.extra == 7) { t[m.r1, 5] = t[m.r1, 7]; t[m.r1, 7] = 0; }
            if (m.extra == 8) { t[m.r1, 3] = t[m.r1, 0]; t[m.r1, 0] = 0; }
            if (m.extra == 9) { t[m.r1, 5] = t[m.r1, 7]; t[m.r1, 7] = 0; }
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
            if (t[m.r2, m.c2] == 6) { x1 = m.r2; y1 = m.c2; }
            if (t[m.r2, m.c2] == -6) { x2 = m.r2; y2 = m.c2; }
        }
        protected void schimbare_inversa(Mutare m)
        {
            //pt regula de 50 de mutari se intoarce automat la valoarea anterioara
            //mutare normala
            t[m.r1, m.c1] = t[m.r2, m.c2];
            t[m.r2, m.c2] = piese[nr_mutari3f];
            //en passant
            if (m.extra == 1) t[m.r1, m.c2] = -t[m.r1, m.c1];
            //transformare
            if (1 < m.extra && m.extra < 6) t[m.r1, m.c1] = Math.Sign(t[m.r1, m.c1]);
            //rocadele
            if (m.extra == 6) { t[0, 0] = 4; t[0, 3] = 0; }
            if (m.extra == 7) { t[0, 7] = 4; t[0, 5] = 0; }
            if (m.extra == 8) { t[7, 0] = -4; t[7, 3] = 0; }
            if (m.extra == 9) { t[7, 7] = -4; t[7, 5] = 0; }
            //conditii rocade
            if (rege_alb == nr_mutari3f) rege_alb = -1;
            if (turn_alb_mic == nr_mutari3f) turn_alb_mic = -1;
            if (turn_alb_mare == nr_mutari3f) turn_alb_mare = -1;
            if (rege_negru == nr_mutari3f) rege_negru = -1;
            if (turn_negru_mic == nr_mutari3f) turn_negru_mic = -1;
            if (turn_negru_mare == nr_mutari3f) turn_negru_mare = -1;
            //mutare regi
            if (t[m.r1, m.c1] == 6) { x1 = m.r1; y1 = m.c1; }
            if (t[m.r1, m.c1] == -6) { x2 = m.r1; y2 = m.c1; }
            //resetare sfarsit
            nr_mutari3f--;
            sfarsit = 0;
        }
    }
}
