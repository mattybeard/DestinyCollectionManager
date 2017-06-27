using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollectionManagerSite.Models
{
    public class CompleteTypeResults
    {
        public TypeResults Shaders { get; set; }
        public DateTime ShadersExpiryDate { get; set; }
        public TypeResults Emblems { get; set; }
        public TypeResults Ships { get; set; }
        public TypeResults Sparrows { get; set; }

        public CompleteTypeResults()
        {
            Shaders = new TypeResults();
            Emblems = new TypeResults();
            Ships = new TypeResults();
            Sparrows = new TypeResults();
        }
    }
}