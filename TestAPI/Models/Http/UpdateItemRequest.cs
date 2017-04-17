using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models.Http
{
    public class UpdateItemRequest
    {   
        public string NewName { get; set; }
        public string NewPrice { get; set; }
        public string NewCondition { get; set; }
        public string NewDescription { get; set; }
        public string NewThumbnail { get; set; }
        public bool Approved { get; set; }
    }
}
