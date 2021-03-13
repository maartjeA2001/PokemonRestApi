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
        [HttpPost("api/pokemon")]
        public ActionResult<PokemonResponse> Get(FilterModel filter)
        {
            return new PokemonResponse();
        }
    }
}
