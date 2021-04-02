namespace PokemonRESTApi.Models
{
    public class SortOrder
    {
        public Field FieldName { get; set; }
        public bool Descending { get; set; } = false;
    }
}