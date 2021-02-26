

namespace ChessAppV5
{
    public class BoardModelView
    {
        //trebuie pt en passant si afisarea ultimelor mutari
        public Mutare lm { get { return DllImports.lm(); } }
        public Mutare lm_alb { get { return DllImports.lm_alb(); } }
        public Mutare lm_negru { get { return DllImports.lm_negru(); } }
        public int sfarsit { get { return DllImports.GetSfarsit(); } }//doar pt afisare e public
        public int move_number { get { return DllImports.GetNrMutari3f(); } }

        //private BoardModel bm = new BoardModel();

        public string verificare_initializare(int[,] tt, bool turn)
        {
            int[] tt2 = new int[64];
            int idx = 0;
            foreach (int elem in tt)
            {
                tt2[idx] = elem;
                idx++;
            }

            bool[] b = new bool[5];

            //verifica pozitia si daca e corecta o initializeaza
            //returneaza string cu erorile rezultate
            int ret = DllImports.verificare_initializare(tt2, turn ? 1 : -1);
            
            for (int i = 0; i < 5; i++)
            {
                int mask = (1 << i);
                b[i] = ((ret & mask) == mask); 
            }
           
            string s = "";
            if (b[0] && !b[2] && b[1] && !b[3])
            {
                if (!b[4]) s = "Initializare reusita.";
                else s += "Primul player nu poate muta.";
            }
            if (!b[0]) s = "Trebuie sa existe exact un rege alb\n";
            else if (b[2]) s = "Regele alb e in sah si e tura negrului\n";
            if (!b[1]) s += "Trebuie sa existe exact un rege negru\n";
            else if (b[3]) s += "Regele negru e in sah si e tura albului\n";
            return s + "\n";
        }

        public bool verificare_mutare(Mutare m, bool player)
        {
            if (sfarsit == 0)
            {
                return DllImports.verificare_mutare(m, player ? 1 : -1);
            }
            return false;
        }
        /*metoda veche 
        using System.Collections.Generic;
        using System.Linq;
        using System.Reflection;
        private List<MethodInfo> EvaluationNames;
        //atentie metodele sa fie publice in model
        public string[] GetEvaluationNames()
        {
            List<string> MyMethodNames = new List<string>();
            EvaluationNames = bm.GetType().GetMethods().ToList();
            EvaluationNames = EvaluationNames.Where(a => a.Name.IndexOf("evaluare") >= 0).ToList();
            foreach (MethodInfo mi in EvaluationNames)
                MyMethodNames.Add(mi.Name.Substring(9));
            return MyMethodNames.ToArray();
        }
        //atentie ca nu trebuie evaluare.invoke, inainte era ai.invoke
        public Mutare ai(int EvaluationIndex, bool Player, bool Dynamic, bool RT)
        {
            if (sfarsit == 0)//trebuie aici ca sa nu o scriu in toate din model
                return (Mutare)EvaluationNames[EvaluationIndex].Invoke(bm, new object[] { Player ? 1 : -1 });
            return new Mutare(-1, -1, -1, -1, -1);
        }*/
        public string[] GetEvaluationNames2()
        {
            return new string[] { "Random", "Basic", "Moves[V4.0]", "Position", "Position 2", "Position 3", "Position 4", "Position 5", "Move Position" };
        }

        public Mutare ai(int EvaluationIndex, bool Player, int depth, bool Dynamic, bool RT)
        {
            return DllImports.AIGeneral(Player ? 1 : -1, depth, EvaluationIndex, Dynamic, RT);
        }
    }
}
