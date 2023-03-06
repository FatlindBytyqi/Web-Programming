using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class DeptBranch
    {

        //relationships
        [Key]
        [Column(Order =1)]
        public int DepartmentID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int BranchID { get; set; }    

        public virtual Department? Department { get; set; }
        public virtual Branch? Branch { get; set; }
    }
}

