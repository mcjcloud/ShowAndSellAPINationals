using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models.Http
{
    public class GetBookmarkResponse
    {
        public string BookmarkId { get; set; }
        public SSItem Item { get; set; }
    }
}
