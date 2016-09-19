using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TestiranjeZavrsni.App_Data;
using TestiranjeZavrsni.Controllers;

namespace TestiranjeZavrsni.Controllers
{
    public class RegistracijaController : Controller
    {
        static int greska=0;
        onlineTestingEntities db = new onlineTestingEntities();
        public ActionResult Registracija()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registracija(string name, string pass, string pass1)
        {
           
            if (pass != pass1)
            {
                ViewData["registracija"] = "2";
                return View("Registracija");
            }
            if (ProvjeriSifru(pass))
            {
                var korisnici = db.Users.Where(s => s.username.ToString()== name).FirstOrDefault();
                
                if(korisnici!=null)
                {
                    ViewData["registracija"] = "1";
                    return View("Registracija");
                }
                  
                MD5 md5Hash = MD5.Create();
                string pas = GetMd5Hash(md5Hash, pass);
                User us = new User { username = name, password = pas };
                db.Users.Add(us);
                db.SaveChanges();
                ViewData["registracija"] = "0";
            }
            else
            {
                ViewData["registracija"] = "3";
                return View("Registracija");

            }
        
            return RedirectToAction("Index", "Home");
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



    }
}