using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employees.Models
{
    public class Employee
    {
        public int ID { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Range(18, 65)]
        public int Age { get; set; }

        public int DepartmentID { get; set; }

        [ForeignKey("DepartmentID")]
        public Department? Department { get; set; }

        [StringLength(100), EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Position { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ProfilePicture { get; set; }
    }
}
