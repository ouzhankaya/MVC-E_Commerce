using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETicaret.Models.ViewModel
{
    public class ProfilModel
    {
        public Member member { get; set; }
        public List<Adress> adress { get; set; }
    }
}