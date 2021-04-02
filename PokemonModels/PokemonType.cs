using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokemonRESTApi.Models
{
    public class PokemonType
    {
        public int typeId { get; set; }
        public String typeName { get; set; } = string.Empty;
    }
}