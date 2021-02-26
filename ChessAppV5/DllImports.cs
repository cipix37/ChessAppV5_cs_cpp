using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessAppV5
{
    public static class DllImports
    {
        [DllImport("ChessDLLMunca1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNrMutari3f();

        [DllImport("ChessDLLMunca1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetEvaluationNames(out int count);

        [DllImport("ChessDLLMunca1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSfarsit();

        [DllImport("ChessDLLMunca1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Mutare lm();

        [DllImport("ChessDLLMunca1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Mutare lm_alb();

        [DllImport("ChessDLLMunca1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Mutare lm_negru();

        [DllImport("ChessDLLMunca1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Mutare AIGeneral(int player, int leaf_depth, int evaluare, bool d, bool RT);

        [DllImport("ChessDLLMunca1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int verificare_initializare(int [] tt, int turn);

        [DllImport("ChessDLLMunca1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool verificare_mutare([MarshalAs(UnmanagedType.Struct)]Mutare mm, int player);
    }
}
