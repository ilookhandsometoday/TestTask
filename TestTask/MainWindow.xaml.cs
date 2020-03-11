using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestTask
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public enum Position
        {
            Left,
            Center,
            Right
        }

        public void EmptyTextBoxPrevention(Position position) 
        {
            Button buttonWildCard = new Button();
            TextBox textBoxWildCard = new TextBox();
            switch (position) 
            {
                case Position.Left:
                    textBoxWildCard = textBoxURLLeft;
                    buttonWildCard = buttonStartLeft;
                    break;
                case Position.Center:
                    textBoxWildCard = textBoxURLCenter;
                    buttonWildCard = buttonStartCenter;
                    break;
                case Position.Right:
                    textBoxWildCard = textBoxURLRight;
                    buttonWildCard = buttonStartRight;
                    break;
            }

            if (textBoxWildCard.Text.Length != 0)
            {
                buttonWildCard.IsEnabled = true;
            }
            else
            {
                buttonWildCard.IsEnabled = false;
            }
        }

        private void textBoxURLLeft_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmptyTextBoxPrevention(Position.Left);
        }

        private void textBoxURLCenter_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmptyTextBoxPrevention(Position.Center);
        }

        private void textBoxURLRight_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmptyTextBoxPrevention(Position.Right);
        }
    }
}
