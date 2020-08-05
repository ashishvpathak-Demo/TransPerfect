using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FIleDataFetch.Controllers
{

    public class RootObject
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("symbol")]
        public Symbol[] symbol { get; set; }
    }

    public class Symbol
    {
        [JsonProperty("seq")]
        public int seq { get; set; }

        [JsonProperty("data")]
        public string data { get; set; }

        [JsonProperty("error")]
        public string error { get; set; }
    }
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object 
            var Resultdata=string.Empty;
            string JsnVal = string.Empty;
            if (Request.Files.Count > 0)
            {

                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    string fname = string.Empty;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string[] stringArray = System.Configuration.ConfigurationManager.AppSettings["FileType"].ToString().Split(',');
                        string extension = System.IO.Path.GetExtension(Request.Files[i].FileName);
                        int pos = Array.IndexOf(stringArray, extension.Replace(".","").ToString().ToLower());
                        if (pos > -1)
                        {
                            HttpPostedFileBase file = files[i];

                            fname = Path.Combine(Server.MapPath("~/UploadFiles/"), Request.Files[i].FileName);
                            if (System.IO.File.Exists(fname))
                            {
                                System.IO.File.Delete(fname);
                            }
                            file.SaveAs(fname);

                            var client = new RestClient("http://api.qrserver.com/v1/read-qr-code/");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddFile("file", fname);
                            IRestResponse response = client.Execute(request);
                            Console.WriteLine(response.Content);
                           // Resultdata = response.Content;
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            RootObject[] ObjRootObject = js.Deserialize<RootObject[]>(response.Content);
                            Resultdata = new JavaScriptSerializer().Serialize(ObjRootObject);
                            //   return Json(response.Content);
                        }
                        else
                        {
                            Resultdata = "Wrong File";
                        //  return  Json("Invalid file: " + fname + "format(Only.xls file)!");
                        }
                    }
                    // Returns message that successfully uploaded  
               //  return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    Resultdata = "Some Error";
                   // return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                Resultdata = "No File";
               // return Json("No files selected.");
            }

           
           
            return Json(JsonConvert.SerializeObject(Resultdata));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}