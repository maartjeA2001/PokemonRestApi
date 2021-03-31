using Microsoft.AspNetCore.Mvc;
using Pokemon_REST_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonRestApi.Controllers
{
    [ApiController]
    public class AbilityController : ControllerBase
    {
        private readonly IAbilityRepository repository;

        public AbilityController(IAbilityRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("api/abilities")]
        public async Task<ActionResult<Ability[]>> GetAbilities()
            => await repository.GetAbilityData(0);

        [HttpGet("api/abilities/{id:int}")]
        public async Task<ActionResult<Ability[]>> Get(int id)
        {
            return await repository.GetAbilityData(id);
        }

        [HttpPut("api/abilities")]
        public async Task updateOrAdd(Ability ability)
            => await repository.UpdateOrAddAblityData(ability);

        [HttpDelete("api/abilities/{id:int}")]
        public async Task Delete(int id)
            => await repository.DeleteAbilityData(id);
    }
}
