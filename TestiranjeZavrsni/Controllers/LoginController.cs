using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Text.RegularExpressions;
using TestiranjeZavrsni.App_Data;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace TestiranjeZavrsni.Controllers
{
    public class LoginController : Controller
    {
        public onlineTestingEntities db = new onlineTestingEntities();
        public static int idd=0;
        private static int brojac = 1;

        public ActionResult Login()
        {
            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Request.Cookies["__LOGINCOOKIE__"].Value);

                if (ticket == null || ticket.Expired == true)
                    throw new Exception();

                Response.Cookies.Add(new HttpCookie("__LOGINCOOKIE__", ""));
            }
            catch
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, "Test", DateTime.Now, DateTime.Now.AddSeconds(5), false, "");
                string encryptedText = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie("__LOGINCOOKIE__", encryptedText));
                Response.Redirect(Request.Path);
            }
            return View();
        }


        public ActionResult Logout()
        {
            brojac = 0;
            if (Session["admin"] != null) Session["admin"] = null;
            else Session["user"] = null;
            db.Users.Where(s => s.id == idd).FirstOrDefault().logovan = 0;
            db.SaveChanges();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            return RedirectToAction("Index", "Home");
           
        }

        [HttpPost]
        public ActionResult Login(string name, string pass)
        {
            brojac = 0;
            string password = "aA1%";
            HashSet<char> specialCharacters = new HashSet<char>() { '%', '$', '#', '!', '?', '.', '=',  };
            if (Session["user"] != null || Session["admin"] != null) RedirectToAction("Index", "Home");
            MD5 md5Hash = MD5.Create();
            string pas = GetMd5Hash(md5Hash, pass);
            var korisnici = db.Users.Where(s => s.username.Equals(name) && s.password.Equals(pas));
            var user = korisnici.FirstOrDefault<User>();
            if (user != null)
            {
                if (user.username.Equals("admin")) Session["admin"] = new User { username = name, password = pas };
                else Session["user"] = new User { username = name, password = pas };
                db.Users.Where(s => s.id == user.id).FirstOrDefault().logovan = 1;
                idd = user.id;
                db.SaveChanges();
                
                return RedirectToAction("Index", "Home");
            }
            else ViewData["sql"] = "1";
               
            return View();
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public ActionResult Editovanje()
        {
            if (Session["admin"] == null && Session["user"] == null)
            {
                return new HttpNotFoundResult("Neovlasten pristup! Haj nazad!");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Editovanje(string stara, string pass, string pass1)
        {
            
            if (Session["admin"] == null && Session["user"] == null)
            {
                return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            }
            if (pass!=pass1)
            {
                ViewData["uspjelo"] = "0";
                return View();
            }     
            if(!ProvjeriSifru(pass))
            {
                ViewData["uspjelo"] = "4";
                return View();
            } 
            MD5 md5Hash = MD5.Create();
            string pas = GetMd5Hash(md5Hash, pass);
            if (brojac == 3)
            {
                ViewData["triviska"] = "Pogriješili ste password tri puta! Morate se logovati ponovo!";
                Logout();
                return RedirectToAction("Login", "Login");
            }
            if (db.Users.Where(s=> s.id==idd).FirstOrDefault().password!= GetMd5Hash(md5Hash, stara))
            {
                brojac++;
                ViewData["uspjelo"] = "3";
                return View();
            }
            db.Users.Where(s => s.id == idd).FirstOrDefault().password = pas;
            ViewData["uspjelo"] = "1";

            db.SaveChanges();

            return View();
        }

        public bool ProvjeriSifru(string pass)
        {
            string password = "aA1%";
            HashSet<char> specialCharacters = new HashSet<char>() { '%', '$', '#', '!', '?', '.' };
            if (pass.Any(char.IsLower) && pass.Any(char.IsUpper) && pass.Any(char.IsDigit)
                && pass.Any(specialCharacters.Contains) && (pass.Count() >= 8 && pass.Count() <= 15))
            {
                return true;
            }
            return false;
        }


    }
}