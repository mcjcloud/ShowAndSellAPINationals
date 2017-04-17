using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models.Http
{
    public class AddGroupRequest
    {
        public SSGroup Group { get; set; }
        public string Password { get; set; }
    }
}
