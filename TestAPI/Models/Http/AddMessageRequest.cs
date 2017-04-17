using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models.Http
{
    public class AddMessageRequest
    {
        public string ItemId { get; set; }
        public string Body { get; set; }
    }
}
