using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokemonRESTApi.Models
{
    public class PokemonResponse
    {
        public Pokemon[] Pokemons { get; set; } = Array.Empty<Pokemon>();
        public PokemonType[] Types { get; set; } = Array.Empty<PokemonType>();
    }
}