//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ETicaret.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StarPoint { get; set; }
        public string ImageName { get; set; }
        public bool IsContinued { get; set; }
        public int UnitsInStock { get; set; }
        public int StarGivenMemberCount { get; set; }
        public System.DateTime AddeDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int CategoryId { get; set; }
        public int OwnerUserId { get; set; }
    }
}
