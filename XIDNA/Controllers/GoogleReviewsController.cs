using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XIDNA.Models;
using XIDNA.ViewModels.GoogleReviews;
using static XIDNA.Models.GoogleReviews;

namespace XIDNA.Controllers
{
    public class GoogleReviewsController : Controller
    {
        // GET: GoogleReview
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>  
        /// This method is used to get the place list  
        /// </summary>  
        /// <param name="SearchText"></param>  
        /// <returns></returns>  
        [HttpGet, ActionName("GetEventVenuesList")]
        public JsonResult GetEventVenuesList(string SearchText)
        {
            string placeApiUrl = ConfigurationManager.AppSettings["GooglePlaceAPIUrl"];
            try
            {
                placeApiUrl = placeApiUrl.Replace(";", "&");
                placeApiUrl = placeApiUrl.Replace("{0}", SearchText);
                placeApiUrl = placeApiUrl.Replace("{1}", ConfigurationManager.AppSettings["GooglePlaceAPIKey"]);
                var result = new System.Net.WebClient().DownloadString(placeApiUrl);
                var Jsonobject = JsonConvert.DeserializeObject<RootObject>(result);
                List<Prediction> list = Jsonobject.predictions;
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        string URL = string.Empty;
        [HttpGet]
        public ActionResult GettingReviews(string placeId)
        {
            List<VMGoogleReviews> ReviewsList = new List<VMGoogleReviews>();
            try
            {
                string placeApiUrl = ConfigurationManager.AppSettings["PlaceIDAPIKey"];
                placeApiUrl = placeApiUrl.Replace(";", "&");
                placeApiUrl = placeApiUrl.Replace("{0}", placeId);
                placeApiUrl = placeApiUrl.Replace("{1}", ConfigurationManager.AppSettings["GooglePlaceAPIKey"]);
                //placeApiUrl = "https://maps.googleapis.com/maps/api/place/details/json?placeid="+ placeId + "&key=AIzaSyBWUUOyiWmq15TzwSAoz_ErHHccqeud_Po";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(placeApiUrl.Trim());
                // request.ContentType = "text/html";
                // request.Method = "GET";
                // request.Proxy = WebProxy.GetDefaultProxy();
                // request.Proxy.Credentials = CredentialCache.DefaultCredentials;
                string InputData = string.Empty;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        InputData = sr.ReadToEnd();
                        dynamic jObj = JsonConvert.DeserializeObject(InputData);
                        foreach (var package in jObj)
                        {
                            var ListofData = package.Next.Last;
                            var Reviews = ListofData.reviews;
                            if (Reviews != null)
                            {
                                foreach (var item in Reviews)
                                {
                                    VMGoogleReviews ReviewDetails = new VMGoogleReviews();
                                    ReviewDetails.Review = item.text;
                                    ReviewDetails.Rating = item.rating;
                                    ReviewDetails.AuthorName = item.author_name;
                                    ReviewDetails.Months = item.relative_time_description;
                                    ReviewDetails.PlaceID = placeId;
                                    ReviewsList.Add(ReviewDetails);
                                }
                            }
                            else
                            {
                                VMGoogleReviews ReviewDetails = new VMGoogleReviews();
                                ReviewDetails.Review = "No Reviews";
                                ReviewsList.Add(ReviewDetails);
                            }
                            break;
                        }
                        sr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Session["ReviewsList"] = ReviewsList;
            return PartialView("_GetReviews", ReviewsList);
        }
        [HttpPost]
        public ActionResult SavingReviews(List<VMGoogleReviews> Reviews1)
        {
            var ListofReviews = Session["ReviewsList"] as List<VMGoogleReviews>;
            List<VMGoogleReviews> re = new List<VMGoogleReviews>();
            ModelDbContext db = new ModelDbContext();
            var List = db.GoogleReviewsDetails.ToList();
            var PlaceID = List.Where(n => n.sPlaceID == ListofReviews.FirstOrDefault().PlaceID).ToList();
            foreach (var item in ListofReviews)
            {
                GoogleReviewData Details = new GoogleReviewData();
                //var DuplicateAuthor=List.Where(k=>k.sAuthorName==PlaceID.Select(u=>u.sAuthorName)).
                var author = List.Where(n => n.sAuthorName == item.AuthorName).FirstOrDefault();
                if (author != null)
                {
                    //Details.sAuthorName = item.AuthorName;
                    //Details.sRating = item.Rating;
                    //Details.sReview = item.Review;
                    //Details.sMonths = item.Months;
                    //db.SaveChanges();
                }
                else
                {
                    Details.sPlaceID = item.PlaceID;
                    Details.sAuthorName = item.AuthorName;
                    Details.sRating = item.Rating;
                    Details.sReview = item.Review;
                    Details.sMonths = item.Months;
                    db.GoogleReviewsDetails.Add(Details);
                    db.SaveChanges();
                }

            }
            return null;
        }

    }
}