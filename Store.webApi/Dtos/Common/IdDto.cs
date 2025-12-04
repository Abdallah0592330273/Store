namespace Store.webApi.Dtos.Common
{
    public class IdDto
    {
        public int Id { get; set; }
    }

    public class IdsDto
    {
        public List<int> Ids { get; set; } = new List<int>();
    }

}
