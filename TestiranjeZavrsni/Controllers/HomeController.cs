using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestiranjeZavrsni.App_Data;

namespace TestiranjeZavrsni.Controllers
{
    public class HomeController : Controller
    {
        
        onlineTestingEntities db = new onlineTestingEntities();
        
        public ActionResult Index()
        {
           if(Session["user"]==null && Session["admin"]==null) Session.Abandon();
           
            return View();
        }

            
        }
    }