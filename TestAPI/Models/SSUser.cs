using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShowAndSellAPI.Models
{
    public class SSUser
    {
        // Properties
        [Key]
        public string SSUserId { get; set; }
        public string GroupId { get; set; }
        
        public string Email { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
