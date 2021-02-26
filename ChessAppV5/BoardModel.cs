using System;

namespace ChessAppV5
{
    public partial class BoardModel
    {
        private Random r = new Random();//necesar pt random

        //functii ajutatoare pentru evaluare
        private int FileStatus(int c)
        {
            int n1 = 0, n2 = 0;
            for (int i = 0; i <= 7; i++)
            {
                if (t[i, c] == 1) n1++;
                if (t[i, c] == -1) n2++;
            }
            if (n1 == 0 && n2 == 0) return 2;//open file
            if (n1 == 0 || n2 == 0) return 1;//semi open file
            return 0;//closed file
        }
        private double PawnRowValue(int r)
        {
            switch (r)
            {
                case 4: return 0.1;
                case 5: return 1;
                case 6: return 3;
                default: return 0;
            }
        }
        private readonly short[,] KnightRelPos = new short[8, 8];
        private readonly short[,] BishopRelPos = new short[8, 8];
        
        //functii de evaluare
        private delegate double delegat_evaluare(int depth);
        delegat_evaluare evaluare_generala;
        public string[] EvaluationNames = { "random", "basic", "position", "moves", "position" };
        private double evaluare_random(int depth)
        {
            //sau nextdouble
            //trebuie %20 ca altfel apare conflict cu maximul admis
            return r.Next() % 20;
        }
        private double evaluare_basic(int depth)
        {
            //suma valorilor pieselor
            int i, j;
            double val = 0;
            for (i = 0; i <= 7; i++)
                for (j = 0; j <= 7; j++)
                    switch (t[i, j])
                    {
                        case 1:
                        case -1: { val += Math.Sign(t[i, j]) * 1; break; }
                        case 2:
                        case -2: { val += Math.Sign(t[i, j]) * 3.5; break; }
                        case 3:
                        case -3: { val += Math.Sign(t[i, j]) * 3.5; break; }
                        case 4:
                        case -4: { val += Math.Sign(t[i, j]) * 5; break; }
                        case 5:
                        case -5: { val += Math.Sign(t[i, j]) * 10; break; }
                        default: break;
                    }
            //evaluare sfarsit
            if (sfarsit != 0)
            {
                if (sfarsit == 1) val = 1000;
                else if (sfarsit == 2) val = -1000;
                else val = -500 * Math.Sign(val);
            }
            return val;
        }
        private double evaluare_basic_position(int depth)
        {
            //valorile si pozitiile pieselor
            int i, j;
            double val = 0;
            for (i = 0; i <= 7; i++)
                for (j = 0; j <= 7; j++)
                    switch (t[i, j])
                    {
                        case 1: { val += Math.Sign(t[i, j]) + PawnRowValue(i); break; }
                        case -1: { val += Math.Sign(t[i, j]) + PawnRowValue(7 - i); break; }
                        case 2:
                        case -2: { val += Math.Sign(t[i, j]) * (3.5 + KnightRelPos[i, j] / 8); break; }// /8
                        case 3:
                        case -3: { val += Math.Sign(t[i, j]) * (3.5 + BishopRelPos[i, j] / 13); break; }// /13
                        case 4:
                        case -4: { val += Math.Sign(t[i, j]) * 5; break; }
                        case 5:
                        case -5: { val += Math.Sign(t[i, j]) * (10 + (14 + BishopRelPos[i, j]) / 21); break; }// /21
                        default: break;
                    }
            //evaluare sfarsit
            if (sfarsit != 0)
            {
                if (sfarsit == 1) val = 1000;
                else if (sfarsit == 2) val = -1000;
                else val = -500 * Math.Sign(val);
            }
            return val;
        }
        private double evaluare_basic_moves(int depth)
        {
            //valorile pieselor + nr mutari
            int i, p = 0, j;
            double val = 0;
            for (i = 0; i <= 7; i++)
                for (j = 0; j <= 7; j++)
                    switch (t[i, j])
                    {
                        case 1:
                        case -1: { val += Math.Sign(t[i, j]) * 0.3; break; }
                        case 2:
                        case -2: { val += Math.Sign(t[i, j]) * 2.5; break; }
                        case 3:
                        case -3: { val += Math.Sign(t[i, j]) * 2.5; break; }
                        case 4:
                        case -4: { val += Math.Sign(t[i, j]) * 4; break; }
                        case 5:
                        case -5: { val += Math.Sign(t[i, j]) * 9; break; }
                        default: break;
                    }
            //nr mutari
            for (i = 0; i <= nr[depth]; i++)
            {
                p = t[m[depth][i].r1, m[depth][i].c1];
                switch (Math.Abs(p))
                {
                    case 1: { val += 0.23 * Math.Sign(p); break; }
                    case 2: { val += 0.13 * Math.Sign(p); break; }
                    case 3: { val += 0.09 * Math.Sign(p); break; }
                    case 4: { val += 0.07 * Math.Sign(p); break; }
                    case 5: { val += 0.04 * Math.Sign(p); break; }
                    case 6: { val += 0.01 * Math.Sign(p); break; }
                }
            }
            //nu se poate si cu mutarile adversarului pt ca se salveaza peste cele bune, plus ca dureaza mult
            //evaluare sfarsit
            if (sfarsit != 0)
            {
                if (sfarsit == 1) val = 1000;
                else if (sfarsit == 2) val = -1000;
                else val = -500 * Math.Sign(val);
            }
            return val;
        }//netestat, ->pot sa pun adversarul peste depth+1
        private double evaluare_move_position(int depth)
        {
            //valorile pieselor + nr mutari
            int i, j, p = 0;
            double val = 0;
            for (i = 0; i <= 7; i++)
                for (j = 0; j <= 7; j++)
                    switch (t[i, j])
                    {
                        case 1:
                        case -1: { val += Math.Sign(t[i, j]) * 0.3; break; }
                        case 2:
                        case -2: { val += Math.Sign(t[i, j]) * 2.5; break; }
                        case 3:
                        case -3: { val += Math.Sign(t[i, j]) * 2.5; break; }
                        case 4:
                        case -4: { val += Math.Sign(t[i, j]) * 4; break; }
                        case 5:
                        case -5: { val += Math.Sign(t[i, j]) * 9; break; }
                        default: break;
                    }
            //nr mutari
            for (i = 0; i <= nr[depth]; i++)
            {
                p = t[m[depth][i].r1, m[depth][i].c1];
                switch (Math.Abs(p))
                {
                    case 1: { val += 0.23 * Math.Sign(p); break; }
                    case 2: { val += 0.13 * Math.Sign(p); break; }
                    case 3: { val += 0.09 * Math.Sign(p); break; }
                    case 4: { val += 0.07 * Math.Sign(p); break; }
                    case 5: { val += 0.04 * Math.Sign(p); break; }
                    case 6: { val += 0.01 * Math.Sign(p); break; }
                }
            }
            //nu se poate si cu mutarile adversarului pt ca se salveaza peste cele bune, plus ca dureaza mult
            //evaluare sfarsit
            if (sfarsit != 0)
            {
                if (sfarsit == 1) val = 1000;
                else if (sfarsit == 2) val = -1000;
                else val = -500 * Math.Sign(val);
            }
            return val;
        }//neterminat, netestat

        //algoritmul propriu zis
        private bool IsDynamic, UseRT;
        public Mutare Alphabeta(double alfa, double beta, int player, double nrmax, int depth, int depth_m)
        {
            int best_index = -1, n;
            double best = -2000 * player;
            //determinare mutari si sfarsit
            det_generala(depth, player);
            //daca am ajuns la o frunza returnez evaluarea
            //if (depth == depth_m || sfarsit != 0) return new Mutare(evaluare_generala(depth));
            //if (nrmax < 1 || sfarsit != 0) return new Mutare(evaluare_generala(ref m, ref nr));
            //if ((depth >= depth_m && nrmax < 1) || sfarsit != 0) return new Mutare(evaluare_generala(depth));
            if ((depth >= depth_m && (nrmax < 1 || !IsDynamic)) || sfarsit != 0) return new Mutare(evaluare_generala(depth));

            for (n = 0; n <= nr[depth]; n++)
            {
                schimbare(m[depth][n]);
                m[depth][n].val = Alphabeta(alfa, beta, -player, nrmax / (nr[depth] + 2), depth + 1, depth_m).val;
                if (m[depth][n].val == player * 1000) m[depth][n].val -= depth * player;//trebuie pentru cel mai scurt mat
                if (player == 1)
                {
                    if (m[depth][n].val > best || (m[depth][n].val == best && r.Next(100) < 25))
                    {
                        best_index = n;
                        best = m[depth][n].val;
                    }
                    alfa = Math.Max(alfa, best);
                }
                else
                {
                    if (m[depth][n].val < best || (m[depth][n].val == best && r.Next(100) < 25))
                    {
                        best_index = n;
                        best = m[depth][n].val;
                    }
                    beta = Math.Min(beta, best);
                }
                schimbare_inversa(m[depth][n]);
                if (alfa > beta) break;
            }
            return m[depth][best_index];
        }

        public Mutare AI_General(int EvaluationIndex, int Player, int Depth, bool Dynamic, bool RT)
        {
            if (sfarsit == 0)
            {
                //initializare
                switch (EvaluationIndex)
                {
                    case 1: { evaluare_generala = evaluare_random; break; }
                    case 2: { evaluare_generala = evaluare_basic; break; }
                    case 3: { evaluare_generala = evaluare_basic_position; break; }
                    case 4: { evaluare_generala = evaluare_basic_moves; break; }
                    case 5: { evaluare_generala = evaluare_move_position; break; }
                    default: throw new Exception("Undefined evaluation function!");
                }
                IsDynamic = Dynamic;
                UseRT = RT;
                //algoritmul efectiv
                Mutare mm = Alphabeta(-2000, 2000, Player, Math.Pow(25, Depth), 0, Depth);
                schimbare(mm);
                //verificare terminare meci
                det_generala(0, -Player);
                return mm;
            }
            return new Mutare(-1, -1, -1, -1, -1);
        }
    }
}
