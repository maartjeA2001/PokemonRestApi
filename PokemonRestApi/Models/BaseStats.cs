using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pokemon_REST_Api.Models
{
    public class BaseStats
    {
        public int hp { get; set; }
        public int atk { get; set; }
        public int def { get; set; }
        public int spAtk { get; set; }
        public int spDef { get; set; }
        public int speed { get; set; }
    }
}