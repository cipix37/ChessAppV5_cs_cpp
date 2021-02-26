using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessAppV5
{
    public class Image2 : Image
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public int Piece { get; private set; }

        public Image2(int r, int c)
        {
            Row = r;
            Column = c;
            Piece = 0;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Height = 60;
            Width = 60;
            if ((r + c) % 2 == 1) Source = BoardImages.Alb;
            else Source = BoardImages.Negru;
        }
        public void SetarePiesa(int p)
        {
            Piece = p;
            switch (p)
            {
                case 0:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.Alb;
                        else Source = BoardImages.Negru;
                        break;
                    }
                case 1:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.PionAA;
                        else Source = BoardImages.PionAN;
                        break;
                    }
                case 2:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.CalAA;
                        else Source = BoardImages.CalAN;
                        break;
                    }
                case 3:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.NebunAA;
                        else Source = BoardImages.NebunAN;
                        break;
                    }
                case 4:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.TurnAA;
                        else Source = BoardImages.TurnAN;
                        break;
                    }
                case 5:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.ReginaAA;
                        else Source = BoardImages.ReginaAN;
                        break;
                    }
                case 6:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.RegeAA;
                        else Source = BoardImages.RegeAN;
                        break;
                    }
                case -1:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.PionNA;
                        else Source = BoardImages.PionNN;
                        break;
                    }
                case -2:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.CalNA;
                        else Source = BoardImages.CalNN;
                        break;
                    }
                case -3:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.NebunNA;
                        else Source = BoardImages.NebunNN;
                        break;
                    }
                case -4:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.TurnNA;
                        else Source = BoardImages.TurnNN;
                        break;
                    }
                case -5:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.ReginaNA;
                        else Source = BoardImages.ReginaNN;
                        break;
                    }
                case -6:
                    {
                        if ((Row + Column) % 2 == 1) Source = BoardImages.RegeNA;
                        else Source = BoardImages.RegeNN;
                        break;
                    }
                default: { throw new Exception("Error! Unknown piece!"); }
            }
        }
        public Point GetCenter()
        {
            return new Point(Margin.Left + Width / 2, Margin.Top + Height / 2);
        }
    }
}
