using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeChecker.Models
{
    public class CourseInfo
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("nameCourse")]
        public string? nameCourse { get; set; }

        [JsonPropertyName("yearofcourse")]
        public string? yearofcourse { get; set; }

        [JsonPropertyName("language")]
        public string? language { get; set; }
        
        [JsonPropertyName("students")]
        public List<Student>? students { get; set; }
    }

}
