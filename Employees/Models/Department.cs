using System.ComponentModel.DataAnnotations;

namespace Employees.Models
{
    public class Department
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}
