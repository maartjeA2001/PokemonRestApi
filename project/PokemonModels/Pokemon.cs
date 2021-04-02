using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokemonRESTApi.Models
{
    public class Pokemon
    {
        public int pokemonId { get; set; }
        public String pokemonName { get; set; } = string.Empty;
        public int height { get; set; }
        public int weight { get; set; }
        public int baseExperience { get; set; }
        public BaseStats baseStats { get; set; } = new BaseStats();
        public Ability[] abilities { get; set; } = Array.Empty<Ability>();
        public int[] pokemonTypeIds { get; set; } = Array.Empty<int>();
    }
}