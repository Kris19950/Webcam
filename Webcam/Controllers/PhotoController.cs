using System;
using System.Web.Mvc;
using System.IO;
using System.IO.Ports;
using System.Web.Services;
using System.Net;
using Webcam;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Webcam.Models;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace WebcamMVC.Controllers
{
    

    public class PhotoController : Controller
    {
        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        private ApplicationDbContext DbContext => HttpContext.GetOwinContext().Get<ApplicationDbContext>();

        [HttpGet]
        public ActionResult Index()
        {




            Session["val"] = "";
            return View();


        }

        [HttpPost]
        public ActionResult Index(string Imagename)
        {

            string sss = Session["val"].ToString();
            ViewBag.pic = "/Images/WebImages/" + Session["val"].ToString();

            return View();
        }

        [HttpGet]
        public ActionResult Changephoto()
        {
         

   
            if (Convert.ToString(Session["val"]) != string.Empty)
            {
                ViewBag.pic = "../../Images/WebImages/" + Session["val"].ToString();
            }
            else
            {
                ViewBag.pic = "../../Images/WebImages/person.jpg";
            }

            var model = new List<Photo>();

            var userId = User.Identity.GetUserId();
            var user = DbContext.Users
                .Include(x => x.Photos)
                .FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {
                model = user.Photos;
            }

            return View(model);
        }

        public FileContentResult GetPhoto(string id)
        {
            var photo = DbContext.Photos.FirstOrDefault(x => x.Id.ToString() == id);

            if (photo != null)
            {
                var bytes = String_To_Bytes2(photo.Bytes);

                return File(bytes, photo.ImageType);
            }

            return null;
        }

        public JsonResult Rebind()
        {
            string path = "/Images/WebImages/" + Session["val"].ToString();

            return Json(path, JsonRequestBehavior.AllowGet);
        }
        public Boolean lewo=false;
        public Boolean prawo = false;
        public Boolean gora = false;
        public Boolean dol = false;

        public void Gora()
        {
            gora = true;
            SerialPort ardo;
            ardo = new SerialPort();
            ardo.PortName = "COM5";
            ardo.BaudRate = 9600;
            ardo.Open();
            ardo.Write("2");
            ardo.Close();


        }

        public void Dol()
        {
            dol = true;
            SerialPort ardo;
            ardo = new SerialPort();
            ardo.PortName = "COM5";
            ardo.BaudRate = 9600;
            ardo.Open();
            ardo.Write("1");
            ardo.Close();


        }
        public void Lewo()
        {
            lewo=true;
            SerialPort ardo;
            ardo = new SerialPort();
            ardo.PortName = "COM5";
            ardo.BaudRate = 9600;
            ardo.Open();
            ardo.Write("3");
            ardo.Close();


        }
        public void Prawo()
        {
            prawo = true;
            SerialPort ardo;
            ardo = new SerialPort();
            ardo.PortName = "COM5";
            ardo.BaudRate = 9600;
            ardo.Open();
            ardo.Write("4");
            ardo.Close();


        }
        public void Off()
        {
            SerialPort ardo;
            ardo = new SerialPort();
            ardo.PortName = "COM5";
            ardo.BaudRate = 9600;
            ardo.Open();
            ardo.Write("5");
            ardo.Close();


        }
        //    public ActionResult Brak()
        // {

        //  }
        public ActionResult Capture()
        {
            
            DateTime dt = DateTime.Now;
            string folderName = "~/Images/WebImages/";
            string subFolder = dt.ToString("MM.dd.yyyy");
            string pathString = System.IO.Path.Combine(folderName, subFolder);
            if (Directory.Exists(pathString) )
            {
               

                var stream = Request.InputStream;
                string dump;

                var userId = User.Identity.GetUserId();

                var user = DbContext.Users
                    .Include(x => x.Photos)
                    .FirstOrDefault(x => x.Id.ToString() == userId);

                using (var reader = new StreamReader(stream))
                {
                    dump = reader.ReadToEnd();
                    
                    DateTime nm = DateTime.Now;

                    string date = nm.ToString("yyyy.MM.dd hh.mm");

                    var path = Server.MapPath(pathString+"/" + date + ".jpg");

                    System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));

                    ViewData["path"] = date + ".jpg";

                    Session["val"] = date + ".jpg";

                    if (user != null)
                    {
                        var photo = new Photo
                        {
                            Id = Guid.NewGuid(),
                            Bytes = dump,
                            ImageType = "image/jpeg"
                        };

                        user.Photos.Add(photo);
                        DbContext.SaveChanges();

                        ViewData["path"] = photo.Id.ToString();
                    }
                }
                Off();
            }
            else
            {
               
               
               // Directory.CreateDirectory(pathString);
                var stream = Request.InputStream;
                string dump;

                var userId = User.Identity.GetUserId();

                var user = DbContext.Users
                    .Include(x => x.Photos)
                    .FirstOrDefault(x => x.Id.ToString() == userId);

                using (var reader = new StreamReader(stream))
                {
                    dump = reader.ReadToEnd();

                    DateTime nm = DateTime.Now;

                    string date = nm.ToString("yyyy.MM.dd hh.mm");
                    date += " Oświetlenie ";
                    if(gora==true)
                    { date += "góra "; }
                    if (dol == true)
                    { date += "dół "; }
                    if (prawo == true)
                    { date += "prawo "; }
                    if (lewo == true)
                    { date += "lewo "; }
                    if (gora != true&& dol!=true&&prawo!=true&&lewo!=true)
                    { date += "wyłączone "; }

                    var path = Server.MapPath("~/Images/WebImages/" +  date + ".jpg");
                    System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));
  

                    ViewData["path"] = date + ".jpg";
                   
                    Session["val"] = date + ".jpg";

                    if (user != null)
                    {
                        var photo = new Photo
                        {
                            Id = Guid.NewGuid(),
                            Bytes = dump,
                            ImageType = "image/jpeg"
                        };

                        user.Photos.Add(photo);
                        DbContext.SaveChanges();

                        ViewData["path"] = photo.Id.ToString();
                    }
                }
                Off();
            }
            return View("Index");

        }       

        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;

            byte[] bytes = new byte[numBytes];

            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }
  
        public ActionResult Auto()
        {
            Session["val"] = "";
            return View();
        }

        public ActionResult UserInfo()
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.
                Users.FirstOrDefault(x => x.Id == userId);

            var model = new UserInfo();

            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return PartialView("_UserInfo", model);
        }


        // IsValid is my Director prop same name give    



    }


}



