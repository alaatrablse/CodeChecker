using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using CodeChecker.Models;
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using CodeChecker.Models.Server;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Shapes;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Policy;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Collections;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;

namespace CodeChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ContactDb contactDb = new ContactDb();

        private string datafolder;
        private string mainfolder;

        public MainWindow()
        {
            InitializeComponent();
            string newfolder = @$"{AppContext.BaseDirectory}" + "data";
            mainfolder = newfolder;
            datafolder = newfolder;
            AddnewHW.IsEnabled = false;
            submitting.IsEnabled = false;
            check.IsEnabled = false;
            try
            {
                if (!Directory.Exists(newfolder))
                {
                    DirectoryInfo di = Directory.CreateDirectory(newfolder);
                }
                else
                {
                    string[] dirs = Directory.GetDirectories(newfolder);
                    foreach (string dir in dirs)
                    {
                        string[] parts = dir.Split('\\');
                        listboxname.Items.Add(parts[parts.Length - 1]);
                        Choose_year.Items.Add(parts[parts.Length - 1]);
                        st_year.Items.Add(parts[parts.Length - 1]);
                    }
                }
            }
            catch (IOException ioex)
            {
                // Console.WriteLine(ioex.Message);
                MessageBox.Show(ioex.ToString());
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (listboxname.SelectedIndex > -1)
            {
                string s = listboxname.SelectedItem.ToString();

                string newfolder = mainfolder + '\\' + s;
                mainfolder = newfolder;
                getfolders(newfolder);
                //listboxname.Items.Add(newfolder);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string newfolder = mainfolder;
            string s = newfolder.Substring(0, newfolder.LastIndexOf("\\"));

            if (s.Contains("data") == true)
            {
                mainfolder = s;
                getfolders(s);
            }
        }


        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (listboxname.SelectedIndex > -1)
            {
                st_year.SelectedIndex = 0;
                st_course.SelectedIndex = 0;
                string s = listboxname.SelectedItem.ToString();
                string[] studentdata = s.Split("  ");
                string[] parts = mainfolder.Split('\\');
                string temp = parts[parts.Length - 1];

                if (temp == "course_info.json")
                {

                    DialogResult dr = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete stodent?",
                               "Mood Test", MessageBoxButtons.YesNo);
                    if (dr.ToString() == "Yes")
                    {

                        StudentDto st = new StudentDto();
                        st.year = parts[parts.Length - 3];
                        st.Namecourse = parts[parts.Length - 2];
                        st.StudemtId = studentdata[1];
                        st.Name = studentdata[0];

                        var boolean = await contactDb.Delet(st);

                        if (boolean == true)
                        {
                            var jsonData = System.IO.File.ReadAllText(mainfolder);
                            CourseInfo courseInfoJson = JsonConvert.DeserializeObject<CourseInfo>(jsonData)
                                      ?? new CourseInfo();
                            if (courseInfoJson.students != null)
                            {
                                courseInfoJson.students.RemoveAll(x => x.id == studentdata[1] && x.name == studentdata[0]);
                                var JsonSting = JsonConvert.SerializeObject(courseInfoJson);
                                File.WriteAllText(mainfolder, JsonSting);
                            }
                        }
                    }
                }
                else
                {
                    string str = mainfolder.Substring(mainfolder.IndexOf("data"));
                    int size = str.Split('\\').Length;
                    if (s != "course_info.json" && s != "rules.json")
                    {
                        DialogResult dr = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete this folder? " +
                            "(Delete all students)", "Mood Test", MessageBoxButtons.YesNo);
                        if (dr.ToString() == "Yes")
                        {
                            bool boolean = false;
                            if (size == 1)
                            {
                                boolean = await contactDb.DeletYear(s);
                            }
                            if (size == 2)
                            {
                                boolean = await contactDb.DeletCours(s);
                            }
                            if (size == 3)
                            {


                            }


                            string deletfolder = mainfolder + '\\' + s;
                            if (Directory.Exists(deletfolder))
                            {
                                Directory.Delete(deletfolder, true);
                                Choose_year.Items.Remove(s);
                                st_year.Items.Remove(s);
                            }
                        }

                    }
                }


                getfolders(mainfolder);
            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            st_year.SelectedIndex = 0;
            st_course.SelectedIndex = 0;
            string year = addYear.Text;
            Choose_year.Items.Add(year);
            st_year.Items.Add(year);
            string newfolder = datafolder + '\\' + year;
            try
            {
                if (!Directory.Exists(newfolder))
                {
                    DirectoryInfo di = Directory.CreateDirectory(newfolder);
                }
                getfolders(mainfolder);
                addYear.Text = "";
            }
            catch (IOException ioex)
            {
                Console.WriteLine(ioex.Message);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            st_year.SelectedIndex = 0;
            st_course.SelectedIndex = 0;
            CourseInfo ci = new CourseInfo();
            if (Choose_year.SelectedIndex > 0)
            {
                string s = Choose_year.Text;
                string newfolder = datafolder + '\\' + s;
                if (Directory.Exists(newfolder) && text_newcourse.Text != "")
                {
                    string ss = newfolder + '\\' + text_newcourse.Text;
                    if (!Directory.Exists(ss))
                    {
                        ci.nameCourse = text_newcourse.Text;
                        ci.language = languageProg.Text;
                        ci.yearofcourse = Choose_year.Text;
                        var JsonSting = JsonConvert.SerializeObject(ci);
                        DirectoryInfo di = Directory.CreateDirectory(ss);
                        creatfile(ss + '\\' + "course_info.json", JsonSting);
                        var filepath = ss + '\\' + "course_grades.csv";
                        using (StreamWriter writer = new StreamWriter(new FileStream(filepath,
                        FileMode.Create, FileAccess.Write)))
                        {
                            writer.WriteLine("student name,student id,avrage");
                        }
                        getfolders(mainfolder);
                        text_newcourse.Text = "";
                    }
                }
            }

        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (st_name.Text != "" && st_id.Text != "" && st_year.SelectedIndex > 0 && st_course.SelectedIndex > 0)
            {
                StudentDto studentDto = new StudentDto();
                studentDto.Name = st_name.Text;
                studentDto.year = st_year.Text;
                studentDto.StudemtId = st_id.Text;
                studentDto.Namecourse = st_course.Text;
                studentDto.CourseAverage = -1;
                var s = await contactDb.CreateStudent(studentDto);
                if (s.Normalize() != "")
                {
                    Student student = new Student();
                    var filePath = datafolder + "\\" + st_year.Text + "\\" + st_course.Text + "\\course_info.json";
                    if (File.Exists(filePath))
                    {
                        var jsonData = System.IO.File.ReadAllText(filePath);

                        CourseInfo courseInfoJson = JsonConvert.DeserializeObject<CourseInfo>(jsonData)
                                  ?? new CourseInfo();
                        student.name = st_name.Text;
                        student.id = st_id.Text;
                        if (courseInfoJson.students == null)
                            courseInfoJson.students = new List<Student>();
                        courseInfoJson.students.Add(student);
                        var JsonSting = JsonConvert.SerializeObject(courseInfoJson);
                        File.WriteAllText(filePath, JsonSting);
                        st_year.SelectedIndex = 0;
                        st_course.SelectedIndex = 0;
                        st_name.Text = "";
                        st_id.Text = "";
                        getfolders(mainfolder);
                    }
                    else
                    {
                        MessageBox.Show("Error: File \"course_info.json\" not exists.");
                    }
                }
                else
                {
                    MessageBox.Show("Error: Student exists");
                }


            }
            else
            {
                MessageBox.Show("Error: Not all fields are filled out.");
            }
        }

        private void AddnewHW_Click(object sender, RoutedEventArgs e)
        {
            AddHW wen = new AddHW();
            wen.onClick += AddHW;
            wen.ShowDialog();

        }

        private void Submitting_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Zip Files|*.zip;*.rar";
            if (ofd.ShowDialog() == true)
            {
                string filename = ofd.FileName;
                homework(filename);
                getfolders(mainfolder);
            }
        }


        private void st_year_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string[] list = st_year.SelectedItem.ToString().Split(": ");
                string chooes = list[list.Length - 1];
                string link = datafolder + '\\' + chooes;

                if (st_year.SelectedIndex > 0 && Directory.Exists(link))
                {
                    string[] dirs = Directory.GetDirectories(link);
                    foreach (string dir in dirs)
                    {
                        string[] parts = dir.Split('\\');
                        st_course.Items.Add(parts[parts.Length - 1]);
                    }
                }
            }
            catch { }


        }

        private async void check_Click(object sender, RoutedEventArgs e)
        {
            string path = mainfolder + "\\rules.json";
            var jsonData = System.IO.File.ReadAllText(path);
            List<Rules> rulseJson = JsonConvert.DeserializeObject<List<Rules>>(jsonData)
                      ?? new List<Rules>();



            if (listboxname.SelectedIndex > -1)
            {
                string str = listboxname.SelectedItem.ToString();
                string url = mainfolder + '\\' + str;
                string[] parts = url.Split('\\');
                DirectoryInfo d = new DirectoryInfo(url); //Assuming Test is your Folder

                FileInfo[] Files = d.GetFiles(); //Getting Text files

                int point = help_fun(rulseJson, Files, url);
                string[] temp = parts[parts.Length - 1].Split('_');
                StudentDto student = new StudentDto();
                StudentDto student2 = new StudentDto();
                if (temp.Length == 1)
                {
                    student.StudemtId = parts[parts.Length - 1];
                    student.year = parts[parts.Length - 4];
                    student.Namecourse = parts[parts.Length - 3];
                    student.CourseAverage = point;
                    StudentDto studentDto = await contactDb.updet(student);
                    if (studentDto == null)
                    {
                        MessageBox.Show("Server error, try again");
                        return;
                    }
                    else
                    {
                        lineChanger($"{student.StudemtId}", $"{studentDto.Name},{student.StudemtId},{point}", mainfolder + "\\grades.csv");
                        MessageBox.Show($"point of student is: {point}");
                    }
                }
                if (temp.Length == 2)
                {
                    student.StudemtId = temp[0];
                    student.year = parts[parts.Length - 4];
                    student.Namecourse = parts[parts.Length - 3];
                    student.CourseAverage = point;

                    student2.StudemtId = temp[1];
                    student2.year = parts[parts.Length - 4];
                    student2.Namecourse = parts[parts.Length - 3];
                    student2.CourseAverage = point;
                    StudentDto studentDto = await contactDb.updet(student);
                    StudentDto studentDto2 = await contactDb.updet(student2);
                    if (studentDto == null || studentDto2 == null)
                    {
                        MessageBox.Show("Server error, try again");
                        return;
                    }
                    else
                    {
                        lineChanger($"{student.StudemtId}", $"{studentDto.Name},{student.StudemtId},{point}", mainfolder + "\\grades.csv");
                        lineChanger($"{student2.StudemtId}", $"{studentDto2.Name},{student2.StudemtId},{point}", mainfolder + "\\grades.csv");
                        MessageBox.Show($"point of students is: {point}");
                    }
                }



            }
            else
            {
                string[] dirs = Directory.GetDirectories(mainfolder, "*", SearchOption.TopDirectoryOnly);
                foreach (string url in dirs)
                {
                    string[] parts = url.Split('\\');
                    DirectoryInfo d = new DirectoryInfo(url); //Assuming Test is your Folder

                    FileInfo[] Files = d.GetFiles(); //Getting Text files

                    int point = help_fun(rulseJson, Files, url);

                    StudentDto student = new StudentDto();
                    student.StudemtId = parts[parts.Length - 1];
                    student.year = parts[parts.Length - 4];
                    student.Namecourse = parts[parts.Length - 3];
                    student.CourseAverage = point;
                    StudentDto studentDto = await contactDb.updet(student);
                    if (studentDto == null)
                    {
                        MessageBox.Show("Server error, try again");
                        return;
                    }
                    else
                    {
                        lineChanger($"{student.StudemtId}", $"{studentDto.Name},{student.StudemtId},{point}", mainfolder + "\\grades.csv");
                    }

                }
                MessageBox.Show($"All assignments have been checked");

            }

        }


        /////////////////helps fun///////////////////



        private void getfolders(string path)
        {
            string[] par = path.Split('\\');

            if (Directory.Exists(path))
            {
                listboxname.Items.Clear();
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                string s = path.Substring(path.IndexOf("data"));
                int size = s.Split('\\').Length;

                if (size == 3)
                    AddnewHW.IsEnabled = true;
                else
                    AddnewHW.IsEnabled = false;
                if (size == 4)
                {
                    submitting.IsEnabled = true;
                    check.IsEnabled = true;
                }
                else
                {
                    submitting.IsEnabled = false;
                    check.IsEnabled = false;
                }
                foreach (string file in files)
                {
                    string[] parts = file.Split('\\');
                    listboxname.Items.Add(parts[parts.Length - 1]);
                }
                foreach (string dir in dirs)
                {
                    string[] parts = dir.Split('\\');
                    listboxname.Items.Add(parts[parts.Length - 1]);
                }
            }
            else if (File.Exists(path))
            {

                if (path.Contains("course_info.json") == true)
                {
                    AddnewHW.IsEnabled = false;
                    listboxname.Items.Clear();
                    var jsonData = System.IO.File.ReadAllText(path);
                    CourseInfo courseInfoJson = JsonConvert.DeserializeObject<CourseInfo>(jsonData)
                              ?? new CourseInfo();
                    List<Student> s = courseInfoJson.students ?? new List<Student>();
                    listboxname.Items.Add(new ComboBoxItem { Content = $"programming language: {courseInfoJson.language}", IsEnabled = false });
                    listboxname.Items.Add(new ComboBoxItem { Content = "LIST OF STUDENT(name,Id):\n ", IsEnabled = false });
                    foreach (Student student in s)
                    {
                        listboxname.Items.Add($"{student.name}  {student.id}");
                    }
                }
                else if (path.Contains("course_grades.csv") == true)
                {
                    listboxname.Items.Clear();
                    string rootPath = path.Replace("\\course_grades.csv", "");
                    string[] dirs = Directory.GetDirectories(rootPath, "*", SearchOption.TopDirectoryOnly);
                    int sizedirs = dirs.Length;
                    string url = path.Replace("course_grades.csv", "course_info.json");
                    var jsonData = System.IO.File.ReadAllText(url);
                    CourseInfo courseInfoJson = JsonConvert.DeserializeObject<CourseInfo>(jsonData)
                              ?? new CourseInfo();

                    Dictionary<string, int> lis = new Dictionary<string, int>();
                    File.WriteAllText(path, "student name,student id,avrage");
                    foreach (string dir in dirs)
                    {
                        foreach (Student student in courseInfoJson.students)
                        {
                            var jsonData2 = System.IO.File.ReadAllLines(dir + "\\grades.csv");
                            for (int i = 1; i < jsonData2.Length; i++)
                            {
                                var temp = jsonData2[i].Split(',');
                                if (temp[1] == student.id)
                                {
                                    string recordstag = $"{student.name},{student.id}";
                                    int value;
                                    if (!lis.TryGetValue(recordstag, out value))
                                        lis.Add(recordstag, Int32.Parse(temp[2]));
                                    else
                                        lis[recordstag] = value + Int32.Parse(temp[2]);
                                    i = jsonData2.Length + 1;
                                }
                            }
                        }
                    }

                    listboxname.Items.Add(new ComboBoxItem { Content = "LIST OF COURSE GRADES (name,Id,average):\n ", IsEnabled = false });
                    string[] strings = new string[lis.Count + 1];
                    strings[0] = "student name,id,average";
                    int x = 1;
                    foreach (var d in lis)
                    {
                        strings[x] = d.Key + $",{(d.Value / sizedirs).ToString()}";
                        string[] fff = d.Key.Split(',');
                        listboxname.Items.Add($"{fff[0]} {fff[1]} {d.Value / sizedirs}");
                        x++;
                    }
                    File.WriteAllLines(path, strings);
                    //listboxname.Items.Add("yes");

                }
                else if (path.Contains("grades.csv") == true)
                {
                    listboxname.Items.Clear();
                    listboxname.Items.Add(new ComboBoxItem { Content = "LIST OF GRADES(name,Id,point):\n ", IsEnabled = false });
                    var lines = File.ReadAllLines(path);
                    List<string> lis = new List<string>();
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var values = lines[i].Split(',');
                        lis.Add(values[1]);
                        listboxname.Items.Add($"{values[0]} {values[1]} {values[2]}");
                    }
                    string temp = path.Replace("\\grades.csv", "");
                    string[] parts2 = temp.Split('\\');
                    string url = temp.Replace(parts2[parts2.Length - 1], "course_info.json");
                    var jsonData = System.IO.File.ReadAllText(url);
                    CourseInfo courseInfoJson = JsonConvert.DeserializeObject<CourseInfo>(jsonData)
                              ?? new CourseInfo();
                    foreach (Student s in courseInfoJson.students)
                    {
                        bool b = true;
                        foreach (string v in lis)
                        {
                            if (v == s.id)
                                b = false;
                        }
                        if (b) listboxname.Items.Add($"{s.name} {s.id} not-submitted");
                    }

                }
                else
                {
                    listboxname.Items.Clear();
                }
            }
            else if (par[par.Length - 2] == "course_grades.csv")
            {
                string[] tempLis = par[par.Length - 1].Split(' ');
                edit wen = new edit();
                wen.onClick += editfun;
                wen.ShowDialog();

            }
        }

        private void creatfile(string namefile, string input)
        {
            if (!File.Exists(namefile))
            {
                using (StreamWriter sw = File.AppendText(namefile))
                {
                    sw.Write(input);
                }
            }

        }

        private void AddHW(List<Rules> rulses, string b)
        {

            string newfolder = mainfolder + '\\' + b;
            Directory.CreateDirectory(newfolder);

            var JsonSting = JsonConvert.SerializeObject(rulses);
            creatfile(newfolder + '\\' + "rules.json", JsonSting);

            var filepath = newfolder + '\\' + "grades.csv";
            using (StreamWriter writer = new StreamWriter(new FileStream(filepath,
            FileMode.Create, FileAccess.Write)))
            {
                writer.WriteLine("student name,student id,avrage");
            }

            getfolders(mainfolder);

        }

        private async void homework(string path)
        {
            string ss = mainfolder;
            string[] parts = path.Split('\\');
            string last = parts[parts.Length - 1];
            string[] parts2 = last.Split('_');
            List<string> studetsid = new List<string>();
            foreach (string part in parts2)
            {

                if (int.TryParse(part, out int n) || int.TryParse(part.Replace(".zip", ""), out int n2))
                {
                    studetsid.Add(part);
                }
                else
                {
                    MessageBox.Show("name of ZIP file not correct \n(012345678_901234567.Zip / 012345678.zip)");
                    return;
                }
            }

            if (studetsid.Count == 2 || studetsid.Count == 1)
            {
                for (int i = 0; i < studetsid.Count; i++)
                {

                    string s = studetsid[i].Replace(".zip", "");
                    try
                    {
                        await contactDb.Getbyid(s);
                    }
                    catch
                    {
                        MessageBox.Show("Error: Not registered for this course");
                        return;
                    }

                }
                ExtractFile(path, mainfolder + '\\' + last.Replace(".zip", ""));
            }

            getfolders(mainfolder);
        }

       

        private void ExtractFile(string zipFilePath, string extractPath)
        {
            if (!Directory.Exists(extractPath))
            {
                // If directory already exist, CreateDirectory does nothing
                Directory.CreateDirectory(extractPath);

                // Extract current zip file
                ZipFile.ExtractToDirectory(zipFilePath, extractPath);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private List<string[]> getmatrex()
        {
            var s = new List<string[]>();
            return s;
        }




        private int help_fun(List<Rules> rulseJson, FileInfo[] Files, string url)
        {
            int point = 100;
            foreach (Rules r in rulseJson)
            {
                if (r.nameRules == "fileExists")
                {
                    bool b = false;
                    foreach (FileInfo file in Files)
                    {
                        if (file.Name == r.data)
                        {
                            b = true;
                        }
                    }
                    if (b == false) point = point - r.point;
                }
                if (r.nameRules == "expression")
                {
                    bool b = false;
                    foreach (FileInfo file in Files)
                    {
                        if (File.ReadAllText(url + '\\' + file.Name).Contains(r.data))
                        {
                            b = true;
                        }
                    }
                    if (b == false) point = point - r.point;
                }
            }
            return point;
        }


        static void lineChanger(string oldText, string newText, string fileName)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            int line_to_edit = 1;
            bool b = false;
            for (int i = 1; i < arrLine.Length; i++)
            {
                if (arrLine[i].Contains(oldText))
                {
                    line_to_edit = i;
                    b = true;
                    i = arrLine.Length + 1;
                }
            }
            if(b)
            {
                arrLine[line_to_edit] = newText;
                File.WriteAllLines(fileName, arrLine);
            }
            else
            {
                string[] temp = new string[arrLine.Length + 1];
                for (int i = 0; i < arrLine.Length; i++) temp[i] = arrLine[i];
                temp[arrLine.Length] = newText;
                File.WriteAllLines(fileName, temp);
            }

        }

        private void editfun(string name, string id, int point)
        {
            string[] parts = mainfolder.Split('\\');
            StudentDto studentDto = new StudentDto();
            studentDto.Name = name;
            studentDto.StudemtId = id;
            studentDto.Namecourse = parts[parts.Length - 3];
            studentDto.year = parts[parts.Length - 4];
            studentDto.CourseAverage = point;
            contactDb.updet(studentDto);
            lineChanger(id, $"{name},{id},{point}", mainfolder.Replace('\\' + parts[parts.Length - 1], ""));


        }

    }
}
