namespace Pokemon_REST_Api.Models
{
    public class SortOrder
    {
        public string Field { get; set; } ="pokemon.pok_id";
        public bool Desending { get; set; } = false;
    }
}