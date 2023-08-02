using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using CodeChecker.Models;
using System.Linq.Expressions;

namespace CodeChecker
{

    public delegate void INPUT_WERTE(List<Rules> rulses, string b);

    /// <summary>
    /// Interaction logic for AddHW.xaml
    /// </summary>
    public partial class AddHW : Window
    {
        public event INPUT_WERTE onClick;

        private List<Tuple<string, int>> exists;
        private List<Tuple<string, int>> expression;
        

        public AddHW()
        {
            InitializeComponent();
            exists = new List<Tuple<string, int>>();
            expression = new List<Tuple<string, int>>();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Rules> rulses = new List<Rules>();
            int p1 = 0;
            
            try
            {
                p1 = Int32.Parse(points1.Text);
            }
            catch { }


            rulses.Add(new Rules("notSubmitted", "true", p1));
            foreach (Tuple<string, int> t in exists)
                rulses.Add(new Rules("fileExists", t.Item1, t.Item2));
            foreach (Tuple<string, int> t in expression)
                rulses.Add(new Rules("expression", t.Item1, t.Item2));
            if (onClick != null)
            {
                onClick(rulses, textbox.Text);
            }
            this.Close();

        }

        private void addbut_Click(object sender, RoutedEventArgs e)
        {
            if (filename.Text == "")
            {
                MessageBox.Show("The file name is incorrect");
            }
            else
            {
                string s = filename.Text;
                int numVal = Int32.Parse(points2.Text);
                exists.Add(new Tuple<string, int>(s, numVal));
                edit.Items.Add(filename.Text);
                filename.Text = "";
                points2.Text = "0";
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void points1_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*int num, sum = 0;
            int num1 = 0;
            try
            {
                num = Int32.Parse(((TextBox)sender).Text);
            }
            catch
            {
                num = 0;
            }
            try
            {
                num1 = Int32.Parse(points3.Text);
            }
            catch { }
            if (points3.Text != "")
                sum += num1;
            foreach (Tuple<string, int> s in exists)
            {
                sum += s.Item2;
            }
            if (num < 0 || num > 100 - sum)
            {
                MessageBox.Show("number is incorrect (sum of points is 100)");
                ((TextBox)sender).Text = "0";
            }*/
        }

        private void points2_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num, sum = 0;
            int num1 = 0, num2 = 0;

            try
            {
                num = Int32.Parse(((TextBox)sender).Text);
            }
            catch
            {
                num = 0;
            }
            try
            {
                //num1 = Int32.Parse(points1.Text);
                num2 = Int32.Parse(points3.Text);
            }
            catch { }
            /*if (points1.Text != "")
                sum += num1;*/
            if (points3.Text != "")
                sum += num2;
            foreach (Tuple<string, int> s in exists)
            {
                sum += s.Item2;
            }

            if (num < 0 || num > 100 - sum)
            {
                MessageBox.Show("number is incorrect (sum of points is 100)");
                ((TextBox)sender).Text = "0";
            }
        }

        private void points3_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num, sum = 0;
            int num1 = 0;
            try
            {
                num = Int32.Parse(((TextBox)sender).Text);
            }
            catch
            {
                num = 0;
            }
            /*try
            {
                num1 = Int32.Parse(points1.Text);
            }
            catch { }
            if (points1.Text != "")
                sum += num1;*/
            foreach (Tuple<string, int> s in exists)
            {
                sum += s.Item2;
            }
            if (num < 0 || num > 100 - sum)
            {
                MessageBox.Show("number is incorrect (sum of points is 100)");
                ((TextBox)sender).Text = "0";
            }
        }

        private void edit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (edit.SelectedItem != null)
            {
                string choice = edit.SelectedItem.ToString();
                edit.Items.RemoveAt(edit.SelectedIndex);

                for (int i = 0; i < exists.Count; i++)
                {
                    Tuple<string, int> t = exists[i];
                    if (t.Item1 == choice)
                    {
                        filename.Text = choice;
                        string s = t.Item2.ToString();
                        exists.RemoveAt(i);
                        points2.Text = s;
                    }
                }
            }

        }

       /* private void getOutputFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".txt";
            ofd.Filter = "Text Document(.txt)|*.txt";
            if (ofd.ShowDialog() == true)
            {
                string filename = ofd.FileName;
                string[] parts = filename.Split('\\');
                //fileNameOutput.Content = "name the file: " + parts[parts.Length - 1];
                path = filename;
            }
        }*/

        private void edit2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (edit2.SelectedItem != null)
            {
                string choice = edit2.SelectedItem.ToString();
                edit2.Items.RemoveAt(edit2.SelectedIndex);

                for (int i = 0; i < expression.Count; i++)
                {
                    Tuple<string, int> t = expression[i];
                    if (t.Item1 == choice)
                    {
                        stringinput.Text = choice;
                        string s = t.Item2.ToString();
                        expression.RemoveAt(i);
                        points3.Text = s;
                    }
                }
            }
        }


        private void addbut2_Click(object sender, RoutedEventArgs e)
        {
            if (stringinput.Text == "")
            {
                MessageBox.Show("The file name is incorrect");
            }
            else
            {
                string s = stringinput.Text;
                int numVal = Int32.Parse(points3.Text);
                expression.Add(new Tuple<string, int>(s, numVal));
                edit2.Items.Add(stringinput.Text);
                stringinput.Text = "";
                points3.Text = "0";
            }
        }
    }
}
