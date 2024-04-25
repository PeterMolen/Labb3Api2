using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Labb3Api2.Models
{
    public class Link
    {
        [Key]
        public int LinkId { get; set; }
        public string URL { get; set; }

        [ForeignKey("Person")]
        public int? FkPersonId { get; set; }
        public Person? Person { get; set; }

        [ForeignKey("Interest")]
        public int FkInterestId { get; set; }
        public Interest? Interest { get; set; }
    }
}
