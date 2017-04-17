using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models
{
    public class SSBookmark
    {
        [Key]
        public string SSBookmarkId { get; set; }

        public string UserId { get; set; }
        public string ItemId { get; set; }
    }
}
