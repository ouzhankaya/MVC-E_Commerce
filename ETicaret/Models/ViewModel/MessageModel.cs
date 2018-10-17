using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETicaret.Models.ViewModel
{
    public class MessageModel
    {
        public List<SelectListItem> Users  { get; set; }
        public List<Message> Messages { get; set; }
    }
}