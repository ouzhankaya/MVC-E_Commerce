using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETicaret.Models.ViewModel
{
    public class CommentModel
    {
        public List<Comment> comment { get; set; }
        public Product product { get; set; }
    }
}