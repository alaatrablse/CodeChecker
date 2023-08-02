using CodeChecker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeChecker
{
    public delegate void INPUT_WERTE1(string name, string id, int point);

    /// <summary>
    /// Interaction logic for edit.xaml
    /// </summary>
    public partial class edit : Window
    {
        public event INPUT_WERTE1 onClick;
        string[] parts;
        public edit()
        {
            InitializeComponent();
            string s = ((MainWindow)Application.Current.MainWindow).listboxname.SelectedItem.ToString();
            parts = s.Split(' ');
            name.Content += " " + parts[0];
            txtId.Content += " "+ parts[1];
            averageinput.Text = parts[2];
        }

        private void cancelbut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void savebut_Click(object sender, RoutedEventArgs e)
        {
            onClick(parts[0], parts[1], Int32.Parse(averageinput.Text));
            this.Close();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void averageinput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num;
            try
            {
                num = Int32.Parse(((TextBox)sender).Text);
            }
            catch
            {
                num = 0;
            }

            if (num < 0 || num > 100)
            {
                MessageBox.Show("number is incorrect (sum of points is 100)");
                ((TextBox)sender).Text = "0";
            }
        }
    }
}
