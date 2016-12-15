using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BungieLoginApp.Model
{
    public class GamertagResponse : ResponseBase
    {
        public ResponseObj Response { get; set; }

        public class ResponseObj
        {
            public BungieUser user { get; set; }
            public string GamerTag { get; set; }
            public string PsnId { get; set; }
        }

        public class BungieUser
        {
            public string membershipId { get; set; }
            public string uniqueName { get; set; }
            public string displayName { get; set; }
            public int profilePicture { get; set; }
            public int profileTheme { get; set; }
            public int userTitle { get; set; }
            public string successMessageFlags { get; set; }
            public bool isDeleted { get; set; }
            public string about { get; set; }
            public string firstAccess { get; set; }
            public string lastUpdate { get; set; }
            public string xboxDisplayName { get; set; }
            public bool showActivity { get; set; }
            public string locale { get; set; }
            public bool localeInheritDefault { get; set; }
            public bool showGroupMessaging { get; set; }
            public string profilePicturePath { get; set; }
            public string profileThemeName { get; set; }
            public string userTitleDisplay { get; set; }
            public string statusText { get; set; }
            public string statusDate { get; set; }
        }
    }
}
