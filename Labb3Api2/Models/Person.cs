using System.ComponentModel.DataAnnotations;

namespace Labb3Api2.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
