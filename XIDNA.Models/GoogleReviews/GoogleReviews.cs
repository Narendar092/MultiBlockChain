using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class GoogleReviews
    {
        public class Prediction
        {
            public string description { get; set; }
            public string id { get; set; }
            public string place_id { get; set; }
            public string reference { get; set; }
            public List<string> types { get; set; }
        }
        public class RootObject
        {
            public List<Prediction> predictions { get; set; }
            public string status { get; set; }
        }
        public class Venue
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }
            public string place_id { get; set; }
        }
    }
}
