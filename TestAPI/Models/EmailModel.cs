using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models
{
    public class EmailModel
    {
        public SSItem Item { get; set; }
        public SSUser Buyer { get; set; }
        public SSGroup Supplier { get; set; }
    }
}
