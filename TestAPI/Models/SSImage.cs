using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models
{
    public class SSImage
    {
        [Key]
        public string ItemId { get; set; }
        public string Thumbnail { get; set; }
    }
}
