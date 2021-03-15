using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pokemon_REST_Api.Models
{
    public class FilterModel
    {
        public int Amount { get; set; } = 10;
        public SortOrder Sort { get; set; } = new SortOrder();
        public string[] CanHaveAbility { get; set; } = Array.Empty<string>();
        public string[] HasType { get; set; } = Array.Empty<string>();
    }
}