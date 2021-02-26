using System.Windows;
using System.Windows.Input;

namespace ChessAppV5
{
    public partial class Transformare : Window
    {
        private MainWindow m;

        public Transformare(MainWindow mm, bool p)
        {
            InitializeComponent();
            m = mm;
            if (p)
            {
                cal.Source = BoardImages.CalAA;
                nebun.Source = BoardImages.NebunAN;
                turn.Source = BoardImages.TurnAA;
                regina.Source = BoardImages.ReginaAN;
            }
            else
            {
                cal.Source = BoardImages.CalNA;
                nebun.Source = BoardImages.NebunNN;
                turn.Source = BoardImages.TurnNA;
                regina.Source = BoardImages.ReginaNN;
            }
        }
        
        private void cal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m.mm.extra = 2;
            Close();
        }
        private void nebun_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m.mm.extra = 3;
            Close();
        }
        private void turn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m.mm.extra = 4;
            Close();
        }
        private void regina_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m.mm.extra = 5;
            Close();
        }
    }
}
