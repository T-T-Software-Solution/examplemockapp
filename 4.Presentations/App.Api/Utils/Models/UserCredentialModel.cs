using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI2.Models
{
    public class UserCredentialModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserCredentialSocialModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string SocialId { get; set; }
    }
}
