using Microsoft.AspNetCore.Mvc;
using PokemonRESTApi.Models;
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
        public async Task<ActionResult<PokemonResponse>> GetByFilter(FilterModel filter)
            => await repository.GetPokemonData(filter);

        [HttpGet("api/pokemon/{id:int}")]
        public async Task<ActionResult<PokemonResponse>> Get(int id)
        {
            FilterModel filter = new FilterModel { HasId = new int[] { id } };
            return await repository.GetPokemonData(filter);
        }

        [HttpPut("api/pokemon")]
        public async Task updateOrAdd(Pokemon pokemon)
            => await repository.UpdateOrAddPokemonData(pokemon);
        
        [HttpDelete("api/pokemon/{id:int}")]
        public async Task Delete(int id)
            => await repository.DeletePokemonData(id);
    }
}