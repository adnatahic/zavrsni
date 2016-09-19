using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using TestiranjeZavrsni.App_Data;
using TestiranjeZavrsni.Controllers;
using TestiranjeZavrsni.Models;

namespace TestiranjeZavrsni.Controllers
{
    public class TestiranjeController : Controller
    {
        onlineTestingEntities db = new onlineTestingEntities();
        public static GenerisiTest noviTest = new GenerisiTest();
        public static Test testZaBazu= new Test();
        private static int id_kategorije = 0;

        

        public ActionResult Testiranje()
        {
            if(!TestirajPrivilegijeAdmin()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            return View(db.Kategorijas.ToList());
            
        }

      

        #region Kategorija

        public ActionResult Create() {

            if(!TestirajPrivilegijeAdmin()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            
            return View();
        }
        

        [HttpPost]
        public ActionResult Create(string naziv, string opis)
        {

            var kategorija = db.Kategorijas.Where(s => s.naziv == naziv && s.opis == opis).FirstOrDefault();
            if(kategorija==null)
            {
                Kategorija k = new Kategorija { naziv = naziv, opis = opis };
                db.Kategorijas.Add(k);
                db.SaveChanges();
                ViewData["kategorija"] = "2";
            }
            else ViewData["kategorija"] = "1";
            
            return View();
        }

       

        public ActionResult Edit(int id = 0)
        {

            if (!TestirajPrivilegijeAdmin()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            Kategorija k = db.Kategorijas.Find(id);


            return View(k);
        }


        public ActionResult Delete(int id = 0)
        {

            if (!TestirajPrivilegijeAdmin()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            Kategorija k = db.Kategorijas.Find(id);
            db.Kategorijas.Remove(k);
            db.SaveChanges();
            return RedirectToAction("Testiranje", "Testiranje");
        }



        [HttpPost]
        public ActionResult Edit(Kategorija k)
        {
            db.Kategorijas.Where(s => s.id == k.id).FirstOrDefault().naziv = k.naziv;
            db.Kategorijas.Where(s => s.id == k.id).FirstOrDefault().opis = k.opis;
             db.SaveChanges();
            return RedirectToAction("Testiranje", "Testiranje");
        
        }
        #endregion

        #region Pitanje 
        public ActionResult Pitanja(int id=0)
        {
            IEnumerable<Pitanje> p = db.Pitanjes.Where(a => a.kategorija == id).ToList();
           IEnumerable<Kategorija> k = db.Kategorijas.ToList();
            var tuple= new Tuple<IEnumerable<Pitanje>, IEnumerable<Kategorija>>(p, k);
            return View(tuple);
        }

        public ActionResult Deletepitanje(int id = 0)
        {

            if (!TestirajPrivilegijeAdmin()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            Pitanje k = db.Pitanjes.Find(id);
            db.Pitanjes.Remove(k);
            db.SaveChanges();
            return RedirectToAction("Pitanja", "Testiranje", new { id = k.kategorija });
        }

        public ActionResult Editpitanje(int id = 0)
        {
            if (!TestirajPrivilegijeAdmin()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            Pitanje p = db.Pitanjes.Find(id);

            return View(p);
        }

        [HttpPost]
        public ActionResult Editpitanje(Pitanje p)
        {
            db.Pitanjes.Where(s => s.id == p.id).FirstOrDefault().tekst = p.tekst;
            db.Pitanjes.Where(s => s.id == p.id).FirstOrDefault().kategorija = p.kategorija;
            db.Pitanjes.Where(s => s.id == p.id).FirstOrDefault().odgovor = p.odgovor;
            db.SaveChanges();
            return RedirectToAction("Pitanja", "Testiranje", new { id= p.kategorija});

        }

        public ActionResult Createpitanje()
        {

            if (!TestirajPrivilegijeAdmin()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");

            return View();
        }

        [HttpPost]
        public ActionResult Createpitanje(int kategorija, string tekst, int odgovor)
        {

            var pitanje = db.Pitanjes.Where(s => s.tekst == tekst && s.kategorija == kategorija).FirstOrDefault();
            if (pitanje == null)
            {
               Pitanje p = new Pitanje { kategorija= kategorija , tekst= tekst, odgovor= odgovor};
               db.Pitanjes.Add(p);
                db.SaveChanges();
                ViewData["pitanje"] = "2";
            }
            else ViewData["pitanje"] = "1";

            return RedirectToAction("Pitanja", "Testiranje", new { id = kategorija });
        }

        #endregion

        #region Privilegije
        public bool TestirajPrivilegijeAdmin()
        {
            if (Session["admin"] != null) return true;
            return false;
        }

        public bool TestirajPrivilegijeUser()
        {
            if (Session["user"] != null) return true;
            return false;
        }

        #endregion

       
        public ActionResult OdaberiKategoriju()
        {
            if(!TestirajPrivilegijeAdmin() && !TestirajPrivilegijeUser()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            RadioKategorija rk = new RadioKategorija();
            rk.kategorije = new List<Kategorija>(db.Kategorijas.ToList());
            return View(rk);
        }

        [HttpPost]
        public ActionResult OdaberiKategoriju(string Kol)
        {
            int idd = 0;
            bool proslo = Int32.TryParse(Kol, out idd);
            List<Pitanje> pitanja = db.Pitanjes.Where(s => s.kategorija == idd).ToList();
            List<Odgovor> netacniodgovori = db.Odgovors.ToList();   
            foreach(Pitanje p in pitanja )
            {
                Odgovor odg = db.Odgovors.Where(s => s.id == p.odgovor).FirstOrDefault();
                netacniodgovori.Remove(odg);
            }

            List<string> nazivi=NapuniListu();
            foreach(string s in nazivi)
            {
                ViewData[s] = s;
            }
            
            noviTest.pitanja = new List<Pitanje>();
            Random rnd = new Random();
            pitanja.OrderBy(s => rnd.Next(0,9));
            noviTest.pitanja = pitanja ;
            noviTest.netacniOdgovori = new List<Odgovor>();
            noviTest.netacniOdgovori = netacniodgovori;
            NapuniTest(pitanja, idd);
           
            return RedirectToAction("KreirajTest");
        }



        public void NapuniTest(List<Pitanje> pit, int id)
        {
            testZaBazu.kategorija = id;
            testZaBazu.vrijeme = DateTime.Now;
            testZaBazu.korisnik = db.Users.Where(s => s.logovan == 1).FirstOrDefault().id;
            Kolekcija k = new Kolekcija();
            k.pitanje1 = pit[0].id;
            k.pitanje2 = pit[1].id;
            k.pitanje3 = pit[2].id;
            k.pitanje4 = pit[3].id;
            k.pitanje5 = pit[4].id;
            k.pitanje6 = pit[5].id;
            k.pitanje7 = pit[6].id;
            k.pitanje8 = pit[7].id;
            k.pitanje9 = pit[8].id;
            k.pitanje10 = pit[9].id;

            db.Kolekcijas.Add(k);
            db.SaveChanges();

        }

        public List<string> NapuniListu()
        {
            List<string> nazivi = new List<string>();
            
            nazivi.Add("prvi");
            nazivi.Add("drugi");
            nazivi.Add("treci");
            nazivi.Add("cetvrti");
            nazivi.Add("peti");
            nazivi.Add("secti");
            nazivi.Add("sedmi");
            nazivi.Add("osmi");
            nazivi.Add("deveti");
            nazivi.Add("deseti");

            return nazivi;
        }

        public ActionResult KreirajTest()
        {
            if (!TestirajPrivilegijeAdmin() && !TestirajPrivilegijeUser()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            return View(noviTest);
        }
        [HttpPost]
        public ActionResult KreirajTest(string prvi, string drugi, string treci, string cetvrti, string peti, string sesti, string sedmi, string osmi, string deveti, string deseti)
        {
            List<string> odabrano = new List<string>();
            int bodovi = 0;
            odabrano.Add(prvi);
            odabrano.Add(drugi);
            odabrano.Add(treci);
            odabrano.Add(cetvrti);
            odabrano.Add(peti);
            odabrano.Add(sesti);
            odabrano.Add(sedmi);
            odabrano.Add(osmi);
            odabrano.Add(deveti);
            odabrano.Add(deseti);
            foreach(string s in odabrano)
            {
                if (s != "-1" || s!=null) bodovi += 2;
            }

            testZaBazu.rezultat = bodovi;
            testZaBazu.kolekcija = db.Kolekcijas.ToList().Last().id;
            db.Tests.Add(testZaBazu);
            db.SaveChanges();

            return RedirectToAction("Bodovi", bodovi);
        }

        public ActionResult Bodovi()
        {
            if (!TestirajPrivilegijeAdmin() && !TestirajPrivilegijeUser()) return new HttpNotFoundResult("Neovlasten pristup! Vratite se na pocetnu!");
            ViewData["bodovi"] = testZaBazu.rezultat.ToString();
            int bodovi = testZaBazu.rezultat;
            testZaBazu = null;
            return View(bodovi);
        }

        public ActionResult Rezultat()
        {
            int id = db.Users.Where(s => s.logovan == 1).FirstOrDefault().id;
            return View(db.Tests.Where(s => s.korisnik == id).ToList());
        }

        public ActionResult Detaljnije(int id=0)
        {

            return View(db.Kolekcijas.Where(s=> s.id==id).FirstOrDefault());
        }

       

    }
}