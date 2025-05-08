using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels.GoogleReviews
{
    public class VMGoogleReviews
    {
        public string AuthorName { get; set; }
        public string Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public string Months { get; set; }
        public string Review { get; set; }
        public string PlaceID { get; set; }
        public List<VMGoogleReviews> ReviewDetails { get; set; }
    }
}
