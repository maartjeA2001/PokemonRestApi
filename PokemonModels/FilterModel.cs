using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokemonRESTApi.Models
{
    public class FilterModel
    {
        public int Amount { get; set; } = 10;
        public SortOrder Sort { get; set; } = new SortOrder();
        public string[] CanHaveAbility { get; set; } = Array.Empty<string>();
        public string[] HasType { get; set; } = Array.Empty<string>();
        public int[] HasId { get; set; } = Array.Empty<int>();
        public string[] HasName { get; set; } = Array.Empty<string>();
    }
}