using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models
{
    public class SSMessage
    {
        [Key]
        public string SSMessageId { get; set; }

        public string ItemId { get; set; }
        public string PosterId { get; set; }
        public string DatePosted { get; set; }

        public string Body { get; set; }
    }
}
