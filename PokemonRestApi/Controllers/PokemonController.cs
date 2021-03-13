using Microsoft.AspNetCore.Mvc;
using Pokemon_REST_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonRestApi.Controllers
{
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonRepository repository;

        public PokemonController(IPokemonRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("api/pokemon")]
        public async Task<ActionResult<PokemonResponse>> Get(FilterModel filter)
            => await repository.GetPokemonData(filter);
    }
}
