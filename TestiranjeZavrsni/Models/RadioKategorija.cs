using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestiranjeZavrsni.App_Data;

namespace TestiranjeZavrsni.Models
{
    public class RadioKategorija
    {
        public List<Kategorija> kategorije { get; set; }
        public string cekirano { get; set; }

    }
}