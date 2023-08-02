using System.ComponentModel.DataAnnotations;

namespace StudentApi.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string StudemtId { get; set; }
        [Required]
        public string year { get; set; }
        [Required]
        public string Namecourse { get; set; }
        public double? CourseAverage { get; set; }
    }
}
