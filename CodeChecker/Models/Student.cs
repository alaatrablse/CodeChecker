using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeChecker.Models
{
    public class Student
    {
        [JsonPropertyName("id")]
        public string? id { get; set; }

        [JsonPropertyName("name")]
        public string? name { get; set; }
    }
}
