using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;

namespace ChessAppV5
{
    public partial class MainWindow : Window
    {
        private BoardModelView bmv = new BoardModelView();
        private Image2[,] i = new Image2[8, 8];
        private Image2 i1, i2;//trebuie pt mutare input uman
        private int SelectedPiece = 0;
        private MyArrow ArrowWhite, ArrowBlack;
        private List<MethodInfo> MyPositions;
        private bool WhiteFirst;

        private Task UITask, MutareTask;
        private TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private CancellationTokenSource cts;
        private CancellationToken ct;

        public Mutare mm;//improvizatie pt transformare

        public MainWindow()
        {
            InitializeComponent();
            int ImageSize = 60, ImageHalfSize = 30, GapSize = 5;
            #region Edit Area
            CheckBoxMove.Checked += new RoutedEventHandler(TurnChanged);
            CheckBoxMove.Unchecked += new RoutedEventHandler(TurnChanged);
            //initializare setare pozitii predefinite
            MyPositions = GetType().GetMethods().ToList();
            MyPositions = MyPositions.Where(a => a.Name.IndexOf("SetPosition") >= 0).ToList();
            foreach (MethodInfo mi in MyPositions)
                ComboBoxPosition.Items.Add(mi.Name.Substring(11));
            //initializare dificultati
            string[] s = bmv.GetEvaluationNames2();
            foreach (string ss in s)
            {
                ComboBoxWhiteEvaluation.Items.Add(ss);
                ComboBoxBlackEvaluation.Items.Add(ss);
            }
            for (int i = 1; i <= 10; i++)
            {
                Depth_alb.Items.Add(i);
                Depth_negru.Items.Add(i);
            }
            //initializare grid auxiliar
            int aux = 0, LeftMarginEdit1 = 340, LeftMarginEdit2 = 405, UpMarginEdit = 115;
            foreach (var a in GridEdit.Children)
                if (a is Image)
                {
                    Image b = a as Image;
                    b.VerticalAlignment = VerticalAlignment.Top;
                    b.HorizontalAlignment = HorizontalAlignment.Left;
                    b.Height = 60;
                    b.Width = 60;
                    b.MouseDown += new MouseButtonEventHandler(SelectNewPiece);
                    if (aux % 2 == 0)
                    {
                        b.Margin = new Thickness(LeftMarginEdit1, UpMarginEdit, 0, 0);
                    }
                    else
                    {
                        b.Margin = new Thickness(LeftMarginEdit2, UpMarginEdit, 0, 0);
                        UpMarginEdit += 65;
                    }
                    aux++;
                }
            #endregion
            #region Play Area
            //initializare labeluri linii
            for (int a = 7; a >= 0; a--)//linii
            {
                Label l = new Label();
                l.HorizontalAlignment = HorizontalAlignment.Left;
                l.VerticalAlignment = VerticalAlignment.Top;
                l.FontSize = 14;
                l.FontWeight = FontWeights.Bold;
                //l.Foreground = Brushes.Red;
                l.HorizontalContentAlignment = HorizontalAlignment.Center;
                l.VerticalContentAlignment = VerticalAlignment.Center;
                l.Height = ImageSize;
                l.Width = ImageHalfSize;
                l.Margin = new Thickness(0, (ImageSize + GapSize) * (a + 1) + GapSize, 0, 0);
                l.Content = (8 - a).ToString();
                GridPlay.Children.Add(l);
            }
            //initializare labeluri coloane
            for (int a = 7; a >= 0; a--)
            {
                Label l = new Label();
                l.HorizontalAlignment = HorizontalAlignment.Left;
                l.VerticalAlignment = VerticalAlignment.Top;
                l.FontSize = 14;
                l.FontWeight = FontWeights.Bold;
                //l.Foreground = Brushes.Red;
                l.HorizontalContentAlignment = HorizontalAlignment.Center;
                l.VerticalContentAlignment = VerticalAlignment.Center;
                l.Height = ImageHalfSize;
                l.Width = ImageSize;
                l.Margin = new Thickness(ImageHalfSize + GapSize + (ImageSize + GapSize) * a, 
                    9 * (GapSize + ImageSize) + GapSize, 0, 0);
                l.Content = ((char)(a + 97)).ToString();
                GridPlay.Children.Add(l);
            }
            //initializare matrice
            for (int a = 0; a <= 7; a++)
                for (int b = 0; b <= 7; b++)
                {
                    i[a, b] = new Image2(a, b);
                    i[a, b].Margin = new Thickness(GapSize + ImageHalfSize + b * (GapSize + ImageSize), 
                        (GapSize + ImageSize) * (8 - a) + GapSize, 0, 0);
                    i[a, b].MouseDown += new MouseButtonEventHandler(SelectBoardField);
                    GridPlay.Children.Add(i[a, b]);
                }
            //initializare sageti pentru ultimele mutari
            ArrowWhite = new MyArrow(ref GridPlay);
            ArrowBlack = new MyArrow(ref GridPlay);
            #endregion
            CheckBoxEditMode.IsChecked = true;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxWhiteEvaluation.SelectionChanged += new SelectionChangedEventHandler(ParametersChanged);
            ComboBoxBlackEvaluation.SelectionChanged += new SelectionChangedEventHandler(ParametersChanged);
            CheckBoxRTWhite.Checked += new RoutedEventHandler(ParametersChanged);
            CheckBoxRTBlack.Checked += new RoutedEventHandler(ParametersChanged);
            CheckBoxRTWhite.Unchecked += new RoutedEventHandler(ParametersChanged);
            CheckBoxRTBlack.Unchecked += new RoutedEventHandler(ParametersChanged);
            CheckBoxDynamicWhite.Checked += new RoutedEventHandler(ParametersChanged);
            CheckBoxDynamicBlack.Checked += new RoutedEventHandler(ParametersChanged);
            CheckBoxDynamicWhite.Unchecked += new RoutedEventHandler(ParametersChanged);
            CheckBoxDynamicBlack.Unchecked += new RoutedEventHandler(ParametersChanged);
            CheckBoxEditMode.Checked += new RoutedEventHandler(EnterEditMode);
            CheckBoxEditMode.Unchecked += new RoutedEventHandler(EnterPlayMode);

            //SetPositionClasic();
            //SetPositionSpeedTest();
            SetPositionBug();
            //SetPositionQueenEnding();
            
            ComboBoxWhiteEvaluation.SelectedIndex = 7;
            ComboBoxBlackEvaluation.SelectedIndex = 0;
            
            Depth_alb.SelectedIndex = 1;
            Depth_negru.SelectedIndex = 1;
        }

        #region set predefined positions
        private void SetPredefinedPosition(object sender, SelectionChangedEventArgs e)
        {
            CheckBoxEditMode.IsChecked = true;
            if ((sender as ComboBox).SelectedItem != null)
                MyPositions[(sender as ComboBox).SelectedIndex].Invoke(this, null);
            (sender as ComboBox).SelectedItem = null;
        }
        public void SetPositionEmpty()
        {
            int[,] t = new int[8, 8];
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 56 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 48 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 40 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 32 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 24 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 16 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 8 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 0 * sizeof(int), 8 * sizeof(int));
            for (short a = 0; a <= 7; a++)
                for (short b = 0; b <= 7; b++)
                    if ((bool)CheckBoxPerspective.IsChecked) i[a, b].SetarePiesa(t[a, b]);
                    else i[a, b].SetarePiesa(t[7 - a, 7 - b]);
        }
        public void SetPositionClasic()
        {
            int[,] t = new int[8, 8];
            Buffer.BlockCopy(new int[] { -4, -2, -3, -5, -6, -3, -2, -4 }, 0, t, 56 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { -1, -1, -1, -1, -1, -1, -1, -1 }, 0, t, 48 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 40 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 32 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 24 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 16 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }, 0, t, 8 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 4, 2, 3, 5, 6, 3, 2, 4 }, 0, t, 0 * sizeof(int), 8 * sizeof(int));
            for (short a = 0; a <= 7; a++)
                for (short b = 0; b <= 7; b++)
                    if ((bool)CheckBoxPerspective.IsChecked) i[a, b].SetarePiesa(t[a, b]);
                    else i[a, b].SetarePiesa(t[7 - a, 7 - b]);
        }
        public void SetPositionQueenEnding()
        {
            int[,] t = new int[8, 8];
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 56 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 48 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 40 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 32 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, -6, 0, 0, 0 }, 0, t, 24 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 16 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 8 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 5, 0, 0, 0, 0, 0, 0, 6 }, 0, t, 0 * sizeof(int), 8 * sizeof(int));
            for (short a = 0; a <= 7; a++)
                for (short b = 0; b <= 7; b++)
                    if ((bool)CheckBoxPerspective.IsChecked) i[a, b].SetarePiesa(t[a, b]);
                    else i[a, b].SetarePiesa(t[7 - a, 7 - b]);
        }
        public void SetPositionRookEnding()
        {
            int[,] t = new int[8, 8];
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 56 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 48 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 40 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 32 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, -6, 0, 0, 0 }, 0, t, 24 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 16 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 8 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 4, 0, 0, 0, 0, 0, 0, 6 }, 0, t, 0 * sizeof(int), 8 * sizeof(int));
            for (short a = 0; a <= 7; a++)
                for (short b = 0; b <= 7; b++)
                    if ((bool)CheckBoxPerspective.IsChecked) i[a, b].SetarePiesa(t[a, b]);
                    else i[a, b].SetarePiesa(t[7 - a, 7 - b]);
        }
        public void SetPositionBug()
        {
            int[,] t = new int[8, 8];
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 56 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 48 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 40 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 32 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, -6, 0, 0, 0 }, 0, t, 24 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 16 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 8 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 5, 6, 0, 0, 0, 0, 0, 0 }, 0, t, 0 * sizeof(int), 8 * sizeof(int));
            for (short a = 0; a <= 7; a++)
                for (short b = 0; b <= 7; b++)
                    if ((bool)CheckBoxPerspective.IsChecked) i[a, b].SetarePiesa(t[a, b]);
                    else i[a, b].SetarePiesa(t[7 - a, 7 - b]);
        }
        public void SetPositionSpeedTest()
        {
            //old
            int[,] t = new int[8, 8];
            Buffer.BlockCopy(new int[] { -4, 0, -3, -5, -6, -3, 0, -4 }, 0, t, 56 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { -1, -1, -1, -1, -1, -1, 0, 0 }, 0, t, 48 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, -2, 0, 0, -2, -1, -1 }, 0, t, 40 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 32 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 1, 1, 0, 0, 0 }, 0, t, 24 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 2, 0, 0, 2, 0, 0 }, 0, t, 16 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 1, 1, 1, 0, 0, 1, 1, 1 }, 0, t, 8 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 4, 0, 3, 5, 6, 3, 0, 4 }, 0, t, 0 * sizeof(int), 8 * sizeof(int));
            for (short a = 0; a <= 7; a++)
                for (short b = 0; b <= 7; b++)
                    if ((bool)CheckBoxPerspective.IsChecked) i[a, b].SetarePiesa(t[a, b]);
                    else i[a, b].SetarePiesa(t[7 - a, 7 - b]);
            //new
            /*int[,] t = new int[8, 8];
            Buffer.BlockCopy(new int[] { -4, 0, -3, -5, -6, 0, -2, -4 }, 0, t, 56 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { -1, -1, -1, -1, 0, -1, -3, -1 }, 0, t, 48 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, -2, 0, -1, 0, -1, 0 }, 0, t, 40 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 32 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, t, 24 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 0, 0, 2, 0, 1, 0, 1, 0 }, 0, t, 16 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 1, 1, 1, 1, 0, 1, 3, 1 }, 0, t, 8 * sizeof(int), 8 * sizeof(int));
            Buffer.BlockCopy(new int[] { 4, 0, 3, 5, 6, 0, 2, 4 }, 0, t, 0 * sizeof(int), 8 * sizeof(int));
            for (short a = 0; a <= 7; a++)
                for (short b = 0; b <= 7; b++)
                    if ((bool)CheckBoxPerspective.IsChecked) i[a, b].SetarePiesa(t[a, b]);
                    else i[a, b].SetarePiesa(t[7 - a, 7 - b]);*/
        }
        #endregion

        private void SelectNewPiece(object sender, MouseButtonEventArgs e)
        {
            //CheckBoxEditMode.IsChecked e atribuit doar la SelectNewPiece, Play si StrengthChanged
            CheckBoxEditMode.IsChecked = true;
            SelectedPiece = Convert.ToInt32((sender as Image).Tag);
        }
        private void SelectBoardField(object sender, MouseButtonEventArgs e)
        {
            if (CheckBoxEditMode.IsChecked == true)
            {
                Image2 a = sender as Image2;
                a.SetarePiesa(SelectedPiece);
                ComboBoxPosition.SelectedItem = null;
            }
            else if (((bool)(CheckBoxMove.IsChecked) && ComboBoxWhiteEvaluation.SelectedIndex == 0) ||
                (!(bool)(CheckBoxMove.IsChecked) && ComboBoxBlackEvaluation.SelectedIndex == 0))
            {
                if (i1 == null)
                {
                    i1 = sender as Image2;
                }
                else
                {
                    i2 = sender as Image2;
                    if ((bool)CheckBoxPerspective.IsChecked)
                        mm = new Mutare(i1.Row, i1.Column, i2.Row, i2.Column);
                    else mm = new Mutare(7 - i1.Row, 7 - i1.Column, 7 - i2.Row, 7 - i2.Column);

                    if (Math.Abs(i1.Piece) == 1 && (i2.Row == 7 || i2.Row == 0))//improvizatie pt transformare
                    {
                        Transformare t = new Transformare(this, (bool)CheckBoxMove.IsChecked);
                        t.ShowDialog();
                    }

                    if (bmv.verificare_mutare(mm, (bool)CheckBoxMove.IsChecked))
                    {
                        schimbare(bmv.lm);//trebuie asa ca sa pastreze campul extra
                        i1 = null;
                        i2 = null;
                    }
                    else
                    {
                        i1 = i2;
                    }
                }
            }
        }

        private void EnterEditMode(object sender, RoutedEventArgs e)
        {
            //CheckBoxEditMode.IsChecked e atribuit doar la SelectNewPiece, setare pozitie si StrengthChanged
            cts?.Cancel();
            CheckBoxMove.IsEnabled = true;
            CheckBoxMove.IsChecked = true;
            ArrowWhite.Hide();
            ArrowBlack.Hide();
            //opresc timpul
        }
        private void EnterPlayMode(object sender, RoutedEventArgs e)
        {
            //CheckBoxEditMode.IsChecked e atribuit doar la SelectNewPiece, setare pozitie si StrengthChanged
            CheckBoxMove.IsEnabled = false;
            WhiteFirst = (bool)CheckBoxMove.IsChecked;
            int[,] t = new int[8, 8];
            for (short a = 0; a <= 7; a++)
                for (short b = 0; b <= 7; b++)
                    if ((bool)(CheckBoxPerspective.IsChecked)) t[a, b] = i[a, b].Piece;
                    else t[a, b] = i[7 - a, 7 - b].Piece;
            string s = bmv.verificare_initializare(t, (bool)CheckBoxMove.IsChecked);
            TextBoxAllText.Text += "\n" + s;
            if (s == "Initializare reusita.\n")
            {
                if ((WhiteFirst && ComboBoxWhiteEvaluation.SelectedIndex != 0) || (!WhiteFirst && ComboBoxBlackEvaluation.SelectedIndex != 0))
                    TurnChanged(sender, e);
            }
            else CheckBoxEditMode.IsChecked = true;
        }

        private void ParametersChanged(object sender, RoutedEventArgs e)
        {
            //CheckBoxEditMode.IsChecked e atribuit doar la SelectNewPiece, setare pozitie si StrengthChanged
            CheckBoxEditMode.IsChecked = true;
        }
        private async void TurnChanged(object sender, RoutedEventArgs e)
        {
            //daca modul e play si urmatorul la mutare e bot, atunci muta
            if (!(bool)CheckBoxEditMode.IsChecked)
            {
                cts = new CancellationTokenSource();
                ct = cts.Token;
                Mutare m = new Mutare(-1, -1, -1, -1, -1);
                int index, depth;
                DateTime dt;
                double t = 0;
                bool d, rt;
                if ((bool)CheckBoxMove.IsChecked)
                {
                    if (ComboBoxWhiteEvaluation.SelectedIndex != 0)
                    {
                        if (ComboBoxBlackEvaluation.SelectedIndex != 0) await Task.Delay(400);//delay daca amandoi sunt boti

                        index = ComboBoxWhiteEvaluation.SelectedIndex;
                        depth = (int)(Depth_alb.SelectedItem);
                        d = (bool)(CheckBoxDynamicWhite.IsChecked);
                        rt = (bool)(CheckBoxRTWhite.IsChecked);
                        MutareTask = new Task(() =>
                        {
                            dt = DateTime.Now;
                            m = bmv.ai(index, true, depth, d, rt);
                            t = (DateTime.Now - dt).TotalSeconds;
                        }, ct);
                        UITask = MutareTask.ContinueWith(a =>
                        {
                            if (!cts.IsCancellationRequested)
                            {
                                TextBoxAllText.Text += Math.Round(t, 2).ToString() + " ";
                                schimbare(m);
                            }
                        }, UIScheduler);
                        MutareTask.Start();
                    }
                }
                else
                {
                    if (ComboBoxBlackEvaluation.SelectedIndex != 0)
                    {
                        if (ComboBoxWhiteEvaluation.SelectedIndex != 0) await Task.Delay(400);//delay daca amandoi sunt boti

                        index = ComboBoxBlackEvaluation.SelectedIndex;
                        depth = (int)(Depth_negru.SelectedItem);
                        d = (bool)(CheckBoxDynamicBlack.IsChecked);
                        rt = (bool)(CheckBoxRTBlack.IsChecked);
                        MutareTask = new Task(() =>
                        {
                            dt = DateTime.Now;
                            m = bmv.ai(index, false, depth, d, rt);
                            t = (DateTime.Now - dt).TotalSeconds;
                        }, ct);
                        UITask = MutareTask.ContinueWith(a =>
                        {
                            if (!cts.IsCancellationRequested)
                            {
                                TextBoxAllText.Text += Math.Round(t, 2).ToString() + " ";
                                schimbare(m);
                            }
                        }, UIScheduler);
                        MutareTask.Start();
                    }
                }
            }
            if ((bool)CheckBoxEditMode.IsChecked) EnterEditMode(sender, e);
            //daca amandoi sunt oameni schimba perspectiva
            if (ComboBoxWhiteEvaluation.SelectedIndex == 0 && ComboBoxBlackEvaluation.SelectedIndex == 0)
            {
                CheckBoxPerspective.IsChecked = !CheckBoxPerspective.IsChecked;
                PerspectiveChanged(sender, e);
            }
        }
        private void PerspectiveChanged(object sender, RoutedEventArgs e)
        {
            if (!(bool)CheckBoxEditMode.IsChecked)
            {
                if ((bool)CheckBoxPerspective.IsChecked)
                {
                    if (bmv.lm_alb.extra != -1)
                    {
                        ArrowWhite.Transform(i[bmv.lm_alb.r1, bmv.lm_alb.c1].GetCenter(),
                            i[bmv.lm_alb.r2, bmv.lm_alb.c2].GetCenter(), Brushes.Red);
                    }
                    if (bmv.lm_negru.extra != -1)
                    {
                        ArrowBlack.Transform(i[bmv.lm_negru.r1, bmv.lm_negru.c1].GetCenter(),
                        i[bmv.lm_negru.r2, bmv.lm_negru.c2].GetCenter(), Brushes.Red);
                    }
                }
                else
                {
                    if (bmv.lm_alb.extra != -1)
                    {
                        ArrowWhite.Transform(i[7 - bmv.lm_alb.r1, 7 - bmv.lm_alb.c1].GetCenter(),
                            i[7 - bmv.lm_alb.r2, 7 - bmv.lm_alb.c2].GetCenter(), Brushes.Red);
                    }
                    if (bmv.lm_negru.extra != -1)
                    {
                        ArrowBlack.Transform(i[7 - bmv.lm_negru.r1, 7 - bmv.lm_negru.c1].GetCenter(),
                        i[7 - bmv.lm_negru.r2, 7 - bmv.lm_negru.c2].GetCenter(), Brushes.Red);
                    }
                }
            }
            //schimbare imagini
            int p;
            for (int r = 0; r <= 3; r++)
                for (int c = 0; c <= 7; c++)
                {
                    p = i[r, c].Piece;
                    i[r, c].SetarePiesa(i[7 - r, 7 - c].Piece);
                    i[7 - r, 7 - c].SetarePiesa(p);
                }
            //schimbare etichete linii si coloane
            Label b;
            foreach (var a in GridPlay.Children)
                if (a is Label)
                {
                    b = a as Label;
                    char c = Convert.ToChar(b.Content);
                    if (c > 60) b.Content = ((char)(201 - c)).ToString();
                    else b.Content = ((char)(105 - c)).ToString();
                }
        }

        private void WhiteLastMove(object sender, RoutedEventArgs e)
        {
            if (!(bool)CheckBoxEditMode.IsChecked)
                if ((bool)CheckBoxWhiteMove.IsChecked && bmv.lm_alb.extra != -1) ArrowWhite.Show();
                else ArrowWhite.Hide();
        }
        private void BlackLastMove(object sender, RoutedEventArgs e)
        {
            if (!(bool)CheckBoxEditMode.IsChecked)
                if ((bool)CheckBoxBlackMove.IsChecked && bmv.lm_negru.extra != -1) ArrowBlack.Show();
                else ArrowBlack.Hide();
        }
        private void Undo(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature not available");
        }
        private void Redo(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature not available");
        }
        private void OfferDraw(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature not available");
        }
        private void Resign(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature not available");
        }

        private void schimbare(Mutare m)
        {
            if (m.extra != -1)//poate fi null la ai cand se termina meciul
            {
                //se aplica mereu pe mutarea furnizata de model deci din perspectiva albului
                //trebuie afisata in functie de perspectiva curenta
                if (!(bool)(CheckBoxPerspective.IsChecked))
                    m = new Mutare(7 - m.r1, 7 - m.c1, 7 - m.r2, 7 - m.c2, m.extra);
                //mutare normala
                i[m.r2, m.c2].SetarePiesa(i[m.r1, m.c1].Piece);
                i[m.r1, m.c1].SetarePiesa(0);
                //en passant
                if (m.extra == 1) i[m.r1, m.c2].SetarePiesa(0);
                //transformare
                if (2 <= m.extra && m.extra <= 5)
                    i[m.r2, m.c2].SetarePiesa(i[m.r2, m.c2].Piece * m.extra);
                //rocadele unificate
                int s = Math.Sign(i[m.r2, m.c2].Piece), s2 = (bool)(CheckBoxPerspective.IsChecked) ? 1 : -1;
                if (m.extra == 6)
                { i[m.r1, m.c1 - 4 * s2].SetarePiesa(0); i[m.r1, m.c1 - s2].SetarePiesa(4 * s); }
                if (m.extra == 7)
                { i[m.r1, m.c1 + 3 * s2].SetarePiesa(0); i[m.r1, m.c1 + s2].SetarePiesa(4 * s); }
                if (m.extra == 8)
                { i[m.r1, m.c1 - 4 * s2].SetarePiesa(0); i[m.r1, m.c1 - s2].SetarePiesa(4 * s); }
                if (m.extra == 9)
                { i[m.r1, m.c1 + 3 * s2].SetarePiesa(0); i[m.r1, m.c1 + s2].SetarePiesa(4 * s); }
                afisare(bmv.lm);//afisarea mereu din perspectiva albului
                CheckBoxMove.IsChecked = !CheckBoxMove.IsChecked;
                //await Task.Run(() => CheckBoxMove.IsChecked = !CheckBoxMove.IsChecked);
            }
        }
        private void schimbare_inversa(Mutare m)
        {
            //mutare normala
            i1.SetarePiesa(i2.Piece);
            i2.SetarePiesa(0);//cum pun ce trebuie, necesar doar pt undo
            if ((bool)CheckBoxPerspective.IsChecked)
            {
                //en passant
                if (m.extra == 1) i[m.r1, m.c2].SetarePiesa(-i[m.r1, m.c1].Piece);
                //transformare
                if (2 <= m.extra && m.extra <= 5)
                    i[m.r1, m.c1].SetarePiesa(Math.Sign(i[m.r1, m.c1].Piece));
                //rocadele
                if (m.extra == 6) { i[0, 0].SetarePiesa(4); i[0, 3].SetarePiesa(0); }
                if (m.extra == 7) { i[0, 7].SetarePiesa(4); i[0, 5].SetarePiesa(0); }
                if (m.extra == 8) { i[7, 0].SetarePiesa(-4); i[7, 3].SetarePiesa(0); }
                if (m.extra == 9) { i[7, 7].SetarePiesa(-4); i[7, 5].SetarePiesa(0); }
            }
            else
            {
                //en passant
                if (m.extra == 1) i[7 - m.r1, 7 - m.c2].SetarePiesa(-i[7 - m.r1, 7 - m.c1].Piece);
                //transformare
                if (2 <= m.extra && m.extra <= 5)
                    i[7 - m.r1, 7 - m.c1].SetarePiesa(Math.Sign(i[7 - m.r1, 7 - m.c1].Piece));
                //rocadele
                if (m.extra == 6) { i[7, 7].SetarePiesa(4); i[7, 4].SetarePiesa(0); }
                if (m.extra == 7) { i[7, 0].SetarePiesa(4); i[7, 2].SetarePiesa(0); }
                if (m.extra == 8) { i[0, 7].SetarePiesa(-4); i[0, 4].SetarePiesa(0); }
                if (m.extra == 9) { i[0, 0].SetarePiesa(-4); i[0, 2].SetarePiesa(0); }
            }
            CheckBoxMove.IsChecked = !CheckBoxMove.IsChecked;
        }
        private void afisare(Mutare m)
        {
            //afisarea mereu din perspectiva albului
            //mofificare sageti - asa trebuie mutarea ca sa fie compatibila cu undo/redo
            if (bmv.lm_alb.extra != -1)
            {
                if ((bool)CheckBoxPerspective.IsChecked)
                    ArrowWhite.Transform(i[bmv.lm_alb.r1, bmv.lm_alb.c1].GetCenter(),
                        i[bmv.lm_alb.r2, bmv.lm_alb.c2].GetCenter(), Brushes.Red);
                else
                    ArrowWhite.Transform(i[7 - bmv.lm_alb.r1, 7 - bmv.lm_alb.c1].GetCenter(),
                        i[7 - bmv.lm_alb.r2, 7 - bmv.lm_alb.c2].GetCenter(), Brushes.Red);
                if ((bool)CheckBoxWhiteMove.IsChecked) ArrowWhite.Show();
            }
            else ArrowWhite.Hide();
            if (bmv.lm_negru.extra != -1)
            {
                if ((bool)CheckBoxPerspective.IsChecked)
                    ArrowBlack.Transform(i[bmv.lm_negru.r1, bmv.lm_negru.c1].GetCenter(),
                    i[bmv.lm_negru.r2, bmv.lm_negru.c2].GetCenter(), Brushes.Red);
                else
                    ArrowBlack.Transform(i[7 - bmv.lm_negru.r1, 7 - bmv.lm_negru.c1].GetCenter(),
                    i[7 - bmv.lm_negru.r2, 7 - bmv.lm_negru.c2].GetCenter(), Brushes.Red);
                if ((bool)CheckBoxBlackMove.IsChecked) ArrowBlack.Show();
            }
            else ArrowBlack.Hide();
            //afisare mutare
            if ((bool)CheckBoxMove.IsChecked)
            {
                if (WhiteFirst) TextBoxAllText.Text += (bmv.move_number / 2 + 1).ToString() + ". ";
                else TextBoxAllText.Text += ((bmv.move_number + 1) / 2 + 1).ToString() + ". ";
            }
            else if (bmv.move_number == 0) TextBoxAllText.Text += "1. ... ";
            TextBoxAllText.Text += ((char)(m.c1 + 97)).ToString() + (m.r1 + 1).ToString() + "-";
            TextBoxAllText.Text += ((char)(m.c2 + 97)).ToString() + (m.r2 + 1).ToString();
            if ((bool)CheckBoxMove.IsChecked) TextBoxAllText.Text += "    ";
            else TextBoxAllText.Text += "\n";
            switch (bmv.sfarsit)
            {
                default: break;//adica 0
                case 1: { TextBoxAllText.Text += "#\n1 - 0\n"; break; }
                case 2: { TextBoxAllText.Text += "#\n0 - 1\n"; break; }
                case 3: { TextBoxAllText.Text += "\n1/2 - 1/2 Stalemate\n"; break; }
                case 4: { TextBoxAllText.Text += "\n1/2 - 1/2 by 50 moves rule\n"; break; }
                case 5: { TextBoxAllText.Text += "\n1/2 - 1/2 by 3 fold repetition\n"; break; }
                case 6: { TextBoxAllText.Text += "\n1/2 - 1/2 insufficient material\n"; break; }
                case 7: { TextBoxAllText.Text += "\n1/2 - 1/2 end of vector reached\n"; break; }
            }
            TextBoxAllText.ScrollToEnd();
        }

        /*private void verificare()
        {
            string s = "ok";
            for (int r = 0; r <= 7; r++)
                for (int c = 0; c <= 7; c++)
                {
                    if ((bool)CheckBoxPerspective.IsChecked)
                    {
                        if (i[r, c].Piece != bmv.bm.t[r, c]) s = "not ok";
                    }
                    else
                    {
                        if (i[7 - r, 7 - c].Piece != bmv.bm.t[r, c]) s = "not ok";
                    }
                }
            TextBoxAllText.Text += s;
            if (s == "not ok")
                CheckBoxEditMode.IsChecked = true;
        }*/
    }
}
