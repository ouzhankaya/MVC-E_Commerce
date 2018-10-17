using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETicaret.Models.ViewModel
{
    public class Basket
    {
        public Basket()
        {
            this.Products = new Dictionary<int, int>();
        }
        public Dictionary<int,int> Products { get; set; }
    }
}