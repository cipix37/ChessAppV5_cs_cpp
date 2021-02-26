using System.Runtime.InteropServices;

namespace ChessAppV5
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Mutare
    {
        public readonly int r1, r2, c1, c2;
        public int extra; //plm nu e atribuit decat in constructor si in transformare
        //0=default, -1=null
        //1=en passant, 2-5 transformare in cal/nebun/turn/regina
        //rocade: 6=mare alb 7=mica alb 8=mare negru 9=mica negru
        public double val;

        public Mutare(int x1, int y1, int x2, int y2)
        {
            r1 = x1;
            c1 = y1;
            r2 = x2;
            c2 = y2;
            extra = 0;
            val = 0;
        }
        public Mutare(int x1, int y1, int x2, int y2, int e)
        {
            r1 = x1;
            c1 = y1;
            r2 = x2;
            c2 = y2;
            extra = e;
            val = 0;
        }
        public Mutare(double v)
        {
            r1 = -1;
            c1 = -1;
            r2 = -1;
            c2 = -1;
            extra = 0;
            val = v;
        }

        public static bool operator ==(Mutare a, Mutare b)
        {
            if (a.r1 == b.r1 && a.c1 == b.c1 && a.r2 == b.r2 && a.c2 == b.c2 && a.extra == b.extra) return true;
            return false;
        }
        public static bool operator !=(Mutare a, Mutare b)
        {
            if (a.r1 != b.r1 || a.c1 != b.c1 || a.r2 != b.r2 || a.c2 != b.c2 || a.extra != b.extra) return true;
            return false;
        }
    }
}
