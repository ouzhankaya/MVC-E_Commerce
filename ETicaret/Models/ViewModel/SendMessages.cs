using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETicaret.Models.ViewModel
{
    public class SendMessages
    {
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public int ToUserId { get; set; }
    }
}