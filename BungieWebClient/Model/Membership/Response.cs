using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BungieWebClient.Model.Membership
{
    public class Response
    {
        public string iconPath { get; set; }
        public int membershipType { get; set; }
        public string membershipId { get; set; }
        public string displayName { get; set; }
    }

    public class MessageData
    {
    }

    public class MembershipResponse
    {
        public List<Response> Response { get; set; }
        public int ErrorCode { get; set; }
        public int ThrottleSeconds { get; set; }
        public string ErrorStatus { get; set; }
        public string Message { get; set; }
        //public MessageData MessageData { get; set; }
    }
}

