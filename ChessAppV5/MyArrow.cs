using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChessAppV5
{
    public class MyArrow
    {
        private Line l, ls, ld;
        private const double Thickness = 3, Angle = Math.PI / 12, Size = 20;

        public MyArrow(ref Grid g)
        {
            l = new Line();
            ls = new Line();
            ld = new Line();
            l.StrokeThickness = Thickness;
            ls.StrokeThickness = Thickness;
            ld.StrokeThickness = Thickness;
            /*l.Visibility = Visibility.Hidden;
            ls.Visibility = Visibility.Hidden;
            ld.Visibility = Visibility.Hidden;*/
            g.Children.Add(l);
            g.Children.Add(ls);
            g.Children.Add(ld);
        }
        public void Show()
        {
            l.Visibility = Visibility.Visible;
            ls.Visibility = Visibility.Visible;
            ld.Visibility = Visibility.Visible;
        }
        public void Hide()
        {
            l.Visibility = Visibility.Hidden;
            ls.Visibility = Visibility.Hidden;
            ld.Visibility = Visibility.Hidden;
        }
        public void Transform(Point p1, Point p2, Brush b)
        {
            //1 e inceputul mutarii si 2 e finalul
            
            //linia centrala
            l.X1 = p1.X;
            l.Y1 = p1.Y;
            l.X2 = p2.X;
            l.Y2 = p2.Y;
            l.Stroke = b;
            //transform in coordonate polare
            Point ps, pd, paux;
            paux= new Point(p2.X - p1.X, p2.Y - p1.Y);
            pd = new Point(Math.Sqrt(Math.Pow(paux.X, 2) + Math.Pow(paux.Y, 2)), Math.Atan(paux.Y / paux.X));
            ps = new Point(pd.X, pd.Y);
            //setez lungimea
            ps.X = Size;
            pd.X = Size;
            //rotesc punctele
            ps.Y += Angle;
            pd.Y -= Angle;
            //compensare 180 grade
            if (paux.X < 0)
            {
                ps.Y = ps.Y + Math.PI;
                pd.Y = pd.Y + Math.PI;
            }
            //transform inapoi in coordonate carteziene
            //linia din stanga
            ls.X1 = -ps.X * Math.Cos(ps.Y) + p2.X;
            ls.Y1 = -ps.X * Math.Sin(ps.Y) + p2.Y;
            ls.X2 = p2.X;
            ls.Y2 = p2.Y;
            ls.Stroke = b;
            //linia din dreapta
            ld.X1 = -pd.X * Math.Cos(pd.Y) + p2.X;
            ld.Y1 = -pd.X * Math.Sin(pd.Y) + p2.Y;
            ld.X2 = p2.X;
            ld.Y2 = p2.Y;
            ld.Stroke = b;
        }
    }
}
