namespace Labb3Api2.Models
{
    public class AddLinkToPersonInterestDTO
    {
        public int FkPersonId { get; set; }
        public List<LinkDTO> Links { get; set; }
    }
}
