using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BungieDatabaseClient.D2;

namespace CollectionManagerSite.Models.Authentication
{
    public class User
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }

        public bool IsValid(UserInfo user, string password)
        {
            var passwordEncoded = SHA1.Encode(password);
            return user.passwordhash.Equals(passwordEncoded);
        }
    }
}