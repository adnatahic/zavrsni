using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestiranjeZavrsni.App_Data;

namespace TestiranjeZavrsni.Models
{
    public class GenerisiTest
    {
        public List<Pitanje> pitanja { get; set; }
        public List<Odgovor> netacniOdgovori { get; set; }
        public List<Kategorija> kategorije { get; set; }
    }
}