﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pokemon_REST_Api.Models
{
    public class PokemonResponse
    {
        public Pokemon[] Pokemons { get; set; } = Array.Empty<Pokemon>();
        public Type[] Types { get; set; } = Array.Empty<Type>();
    }
}