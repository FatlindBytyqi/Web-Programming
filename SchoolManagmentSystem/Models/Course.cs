using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class Course
    {
        [Key]
        [Display(Name = "Number")]
        public int CourseID { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }   
        [Required]
        [Range(0, 5)]
        public int Credits { get; set; }

        //relationship
        public int DepartmentID { get; set; }
        [ForeignKey("DepartmentID")]
        public Department? Department { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }
        public ICollection<Professor>? Teachers { get; set; }
    }
}
