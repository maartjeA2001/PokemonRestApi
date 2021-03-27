using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pokemon_REST_Api.Models
{
    public class Ability
    {
        public int abilityId { get; set; }
        public String abilityName { get; set; } = string.Empty;
        public Boolean isHidden { get; set; }
    }
}