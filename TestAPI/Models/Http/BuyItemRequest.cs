using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models.Http
{
    public class BuyItemRequest
    {
        public string Token { get; set; }
        public string PaymentMethodNonce { get; set; }
        public double Amount { get; set; }
    }
}
