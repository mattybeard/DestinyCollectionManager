using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace CollectionManagerSite.Models
{
    public class TypeResults
    {
        public string ItemType { get; set; }
        public DateTime NextReset { get; set; }
        public string NextResetFormat
        {
            get
            {
                if (NextReset == DateTime.MaxValue || NextReset == DateTime.MinValue)
                    return "";

                var serverTimezone = DateTime.SpecifyKind(NextReset, DateTimeKind.Utc);
                var localTime = serverTimezone.ToLocalTime();

                return $"{localTime.Year}-{localTime.Month}-{localTime.Day}T{localTime.Hour}:00:00Z"; }
        }

        public List<FactionDetails> ForSale { get; set; }
        public List<FactionDetails> Needed { get; set; }

        public TypeResults()
        {
            NextReset = DateTime.MinValue;
            ForSale = new List<FactionDetails>();
            Needed = new List<FactionDetails>();
        }
    }
}