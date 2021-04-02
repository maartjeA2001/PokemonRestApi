using Microsoft.AspNetCore.Mvc;
using PokemonRESTApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonRestApi.Controllers
{
    [ApiController]
    public class TypeController : ControllerBase
    {
        private readonly ITypeRepository repository;

        public TypeController(ITypeRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("api/pokemonTypes")]
        public async Task<ActionResult<PokemonType[]>> GetTypes()
            => await repository.GetTypeData(0);

        [HttpGet("api/pokemonTypes/{id:int}")]
        public async Task<ActionResult<PokemonType[]>> Get(int id)
        {
            return await repository.GetTypeData(id);
        }

        [HttpPut("api/pokemonTypes")]
        public async Task updateOrAdd(PokemonType type)
            => await repository.UpdateOrAddTypeData(type);

        [HttpDelete("api/pokemonTypes/{id:int}")]
        public async Task Delete(int id)
            => await repository.DeleteTypeData(id);
    }
}
