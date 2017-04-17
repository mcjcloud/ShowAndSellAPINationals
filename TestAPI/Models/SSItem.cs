using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShowAndSellAPI.Models
{
    public class SSItem
    {
        // Properties
        [Key]
        public string SSItemId { get; set; }

        // used to map the item to the group it is in, and the user who owns it.
        public string GroupId { get; set; }
        public string OwnerId { get; set; }
        
        public string Name { get; set; }
        public string Price { get; set; }
        public string Condition { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public bool Approved { get; set; }
    }
}
