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
        public Filter[] Filters { get; set; } = Array.Empty<Filter>();
    }
}