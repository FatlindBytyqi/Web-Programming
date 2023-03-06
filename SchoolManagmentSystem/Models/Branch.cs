namespace SchoolManagmentSystem.Models
{
    public  enum Location
    {
        Ferizaj,
        Prishtinë,
        Gjilan,
        Pejë,
        Lipjan
    }
    public class Branch
    {
        public int BranchID { get; set; }
        public int SMIAL { get; set; }
        public string Name { get; set; }
        public Location? Location { get; set; }

        public virtual ICollection<DeptBranch>? DeptBranches { get; set; }

    }
}
