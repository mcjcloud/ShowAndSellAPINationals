using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowAndSellAPI.Models.Http
{
    public class UpdateUserRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewGroupId { get; set; }

        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string NewEmail { get; set; }
    }
}
