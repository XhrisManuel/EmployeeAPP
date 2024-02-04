using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeAPP.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; }    

        public string Password { get; set; } = string.Empty;

        [NotMapped]
        public bool LoginValid { get; set; }
    }
}
