namespace Pokemon_REST_Api.Models
{
    public enum Field
    {
        PokemonId,
        Name,
        Atk,
    }

    public class SortOrder
    {
        public Field Field { get; set; }
        public bool Desending { get; set; } = false;
    }
}