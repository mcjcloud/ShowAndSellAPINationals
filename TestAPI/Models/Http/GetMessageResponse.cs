using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models.Http
{
    public class GetMessageResponse
    {
        public string SSMessageId { get; set; }

        public string ItemId { get; set; }
        public string PosterId { get; set; }
        public string PosterName { get; set; }
        public string AdminId { get; set; }
        public string AdminName { get; set; }
        public string DatePosted { get; set; }

        public string Body { get; set; }
    }
}
