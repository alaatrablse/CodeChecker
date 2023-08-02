using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using static Google.Apis.Requests.BatchRequest;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CodeChecker.Models.Server
{
    public class ContactDb
    {
        private HttpClient client = new HttpClient();
        public ContactDb()
        {
            client.BaseAddress = new Uri("https://localhost:7165");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<List<StudentDto>> Get()
        {
            var response = await client.GetAsync("api/student");
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                var students = await response.Content.ReadFromJsonAsync<IEnumerable<StudentDto>>();
                List<StudentDto> result = new List<StudentDto>();
                foreach (var student in students)
                {
                    result.Add(student);
                }
                return result;
            }
            else
            {
                throw new Exception("error");
            }

        }

        public async Task<StudentDto> Getbyid(string studentid)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/student/{studentid}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var students = await response.Content.ReadFromJsonAsync<StudentDto>();
                    return students;
                }
                else
                {
                    throw new Exception($"Server error code {response.StatusCode}");
                    //return new StudentDto();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
                //return new StudentDto();
            }
        }

        public async Task<string> CreateStudent(StudentDto student)
        {
            try
            {
                var company = JsonSerializer.Serialize(student);
                var requestContent = new StringContent(company, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("api/Student", requestContent);
                string resultContent = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    return resultContent;
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                return "";
            }
        }

        public async Task<bool> Delet(StudentDto student)
        {
            try
            {
                int id = (await getstudentbyid(student.StudemtId, student.year, student.Namecourse)).Id;
                var response = await client.DeleteAsync($"api/student/{id.ToString()}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<bool> DeletCours(string coursname)
        {
            try
            {

                var response = await client.DeleteAsync($"api/student/course/{coursname}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeletYear(string year)
        {
            try
            {

                var response = await client.DeleteAsync($"api/student/year/{year}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<StudentDto> updet(StudentDto student)
        {
            StudentDto studentDto = (await getstudentbyid(student.StudemtId, student.year, student.Namecourse));
            student.Id = studentDto.Id;
            student.Name= studentDto.Name;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var json = JsonConvert.SerializeObject(student, Formatting.Indented);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PutAsync($"api/student/{studentDto.Id}", httpContent);
                response.EnsureSuccessStatusCode();
                return studentDto;
            }
            catch
            {
                return null;
            }
        }


        public async Task<StudentDto> getstudentbyid(string id, string year, string cours)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/student/{id}/{year}/{cours}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var students = await response.Content.ReadFromJsonAsync<StudentDto>();
                    if (students != null)
                        return students;

                    MessageBox.Show($"Server error \"not found\"");
                    return null;
                }
                else
                {
                    MessageBox.Show($"Server error code {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                return null;
            }
        }

    }
}




