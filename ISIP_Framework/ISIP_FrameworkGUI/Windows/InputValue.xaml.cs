using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ISIP_FrameworkGUI.Windows
{
    /// <summary>
    /// Interaction logic for InputValue.xaml
    /// </summary>
    public partial class InputValue : Window
    {
        private float _value;
        public InputValue()
        {
            InitializeComponent();
            _value = 0;
        }

        public float GetValue()
        {
            return _value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(float.TryParse(textBox.Text, out _value)))
            {
                MessageBox.Show("The input value is not correct! Try again.");
                textBox.Clear();
            }
            else
            {
                this.Close();
            }

        }
    }
}
