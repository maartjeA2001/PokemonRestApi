namespace Pokemon_REST_Api.Models
{
    public class Filter
    {
        public string FieldName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool EquelsTo { get; set; } = true;
    }
}