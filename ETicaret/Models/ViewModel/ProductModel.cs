using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETicaret.Models.ViewModel
{
    public class ProductModel
    {
        public Product products { get; set; }
        public List<Comment> comments { get; set; }
    }
}