namespace Labb3Api2.Models
{
    public class AddInterestsToPersonDTO
    {
        public int FkPersonId { get; set; }
        public List<int> InterestIds { get; set; }
    }
}
