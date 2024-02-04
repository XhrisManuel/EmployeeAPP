namespace EmployeeAPP.Models
{
    public class EmployeeViewModel
    {
        public Boolean isLoggedin { get; set; }
        public IList<Employee>? Employees { get; set; }

    }
}
