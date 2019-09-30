using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            engine = new CC.Engine();
        }
        #region TextBlock
        bool shift = false;
        CC.Engine engine;
        private void Code_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (shift)
            {
                try
                {
                    if (e.Delta > 0) (sender as TextBox).FontSize++;
                    if (e.Delta < 0) (sender as TextBox).FontSize--;
                }
                catch { }
            }
        }

        private void Code_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift) shift = true;
            if (e.Key == Key.Right) shift = true;
        }

        private void Code_KeyUp(object sender, KeyEventArgs e)
        {
            shift = false;
        }
        bool turn = false;
        string s = string.Empty;
        private void Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (turn)
            //{
            //    turn = false;
            //    return;
            //}
            if (!Keyboard.IsKeyDown(Key.Back))
            {
                if (!flag) try
                    {
                        s += ((sender as TextBox).Text.Last());
                        //LastValue += ((sender as TextBox).Text.Last());
                    }
                    catch { }
                else flag = false;
            }
            else
            {
                try
                {
                    s = s.Remove(s.Length - 1, 1);
                }
                catch { }
            }
            string v = string.Empty;
            int stack = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ';' && i < s.Length - 1 && s[i + 1] != '\r')
                {
                    v += ";\r";
                    for (int j = 0; j < stack; j++)
                    {
                        v += "    ";
                    }
                }
                else if (s[i] == '{')
                {
                    v += "\r";
                    for (int j = 0; j < stack; j++)
                    {
                        v += "    ";
                    }
                    v += "{";
                    stack++;
                    v += "\r";
                    for (int j = 0; j < stack; j++)
                    {
                        v += "    ";
                    }
                }
                else if (s[i] == '}')
                {
                    stack--;
                    v += "\r";
                    for (int j = 0; j < stack; j++)
                    {
                        v += "    ";
                    }
                    v += "}";
                    v += "\r";
                    for (int j = 0; j < stack; j++)
                    {
                        v += "    ";
                    }
                }
                else if (s[i] == '\r')
                {
                    if (i < s.Length - 1 && s[i + 1] != '{') continue;
                    if (i < s.Length - 1 && s[i + 1] != '}') continue;
                    if (i > 0 && s[i - 1] != '{') continue;
                    if (i > 0 && s[i - 1] != '}') continue;
                    v += "\r";
                    for (int j = 0; j < stack; j++)
                    {
                        v += "    ";
                    }
                }
                else
                {
                    v += s[i].ToString();
                }
            }
            Code.TextChanged -= Code_TextChanged;
            (sender as TextBox).Text = v;
            (sender as TextBox).SelectionStart = (sender as TextBox).Text.Length;
            Code.TextChanged += Code_TextChanged;
            Token.ItemsSource = engine.Tokenizer(s);

            //Console.WriteLine(TokenModel.Tokens.Count);
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            s = string.Empty;
            Code.Text = s;
        }
        bool flag = false;
        private void Load_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                s = File.ReadAllText(openFileDialog.FileName);
            Code.Text = s;
            flag = true;
        }
        #endregion
        #region Config
        private void Choose_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var v = File.ReadAllLines(openFileDialog.FileName);
                foreach (var item in v)
                {
                    try
                    {
                        if (item[0] == '#') continue;
                        if (item.Trim()[0] == '#') continue;
                    }
                    catch { }
                    if (item == string.Empty) continue;
                    engine.Create(item);
                }
                ViewModel.MyProperty.Clear();
                foreach (var item in engine.FSMlist)
                {
                    ViewModel.MyProperty.Add(new CC.View
                    {
                        Name = item.Name,
                        RegEx = item.RegEx,
                        States = item.ToString()
                    });
                }
            }

        }


        private void FSMs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var v = (CC.View)FSMs.SelectedItem;
            new ToolTipWindow(v.States).Show();
        }

        #endregion

        private void Path_Loaded(object sender, RoutedEventArgs e)
        {
            var da = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(5)));
            var rt = new RotateTransform();
            (sender as System.Windows.Shapes.Path).RenderTransform = rt;
            (sender as System.Windows.Shapes.Path).RenderTransformOrigin = new Point(0.5, 0.5);
            da.RepeatBehavior = RepeatBehavior.Forever;
            rt.BeginAnimation(RotateTransform.AngleProperty, da);
        }
    }
    class ViewModel
    {
        static ViewModel()
        {
            MyProperty = new ObservableCollection<CC.View>();
        }
        public static ObservableCollection<CC.View> MyProperty { get; set; }
    }
    class TokenModel
    {
        static TokenModel()
        {
            Tokens = new ObservableCollection<CC.CompilerToken>();
        }
        public static ObservableCollection<CC.CompilerToken> Tokens { get; set; }
    }
}
