//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestiranjeZavrsni.App_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pitanje
    {
        public int id { get; set; }
        public string tekst { get; set; }
        public int kategorija { get; set; }
        public int odgovor { get; set; }
    
        public virtual Kategorija Kategorija1 { get; set; }
        public virtual Odgovor Odgovor1 { get; set; }
    }
}
