using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CollectionManagerSite.Models.Authentication
{
    public static class SHA1
    {
        public static string Encode(string value)
        {
            var hash = System.Security.Cryptography.SHA1.Create();
            var encoder = new System.Text.ASCIIEncoding();
            var encryptionKey = ConfigurationManager.AppSettings["EncrpytionKey"];
            var combined = encoder.GetBytes($"{value}{encryptionKey}");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
    }
}