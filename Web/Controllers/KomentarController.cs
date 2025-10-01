using MyWebApp.Models;
using MyWebApp.Repositories;
using System.Linq;
using System.Web.Mvc;

namespace MyWebApp.Controllers
{
    public class KomentarController : Controller
    {
        private readonly KomentarRepository komentarRepo = new KomentarRepository();

        
        [HttpGet]
        public ActionResult Create(string smestajNaziv)
        {
            var user = (Korisnik)Session["Korisnik"];
            if (user == null || user.uloga != Uloga.Turista)
                return RedirectToAction("Login", "Korisnik");

            ViewBag.SmestajNaziv = smestajNaziv;
            return View();
        }

        [HttpPost]
        public ActionResult Create(string smestajNaziv, string tekst, int ocena)
        {
            var user = (Korisnik)Session["Korisnik"];
            if (user == null || user.uloga != Uloga.Turista)
                return RedirectToAction("Login", "Korisnik");

            

            var komentar = new Komentar
            {
                Turista = user.KorisnickoIme,
                SmestajNaziv = smestajNaziv,
                Tekst = tekst,
                Ocena = ocena,
                Odobren = false
            };

            komentarRepo.Add(komentar);

            TempData["Success"] = "Vaš komentar je zabeležen i prosleđen menadžeru. Nakon pregleda menadžer će odlučiti o vidljivosti komentara. Hvala na komentaru!";
            return RedirectToAction("Create", new { smestajNaziv });
        }

        
        public ActionResult ListaZaOdobravanje()
        {
            var user = (Korisnik)Session["Korisnik"];
            if (user == null || user.uloga != Uloga.Menadzer)
                return RedirectToAction("Login", "Korisnik");

            var komentari = komentarRepo.GetAll().Where(k => !k.Odobren).ToList();
            return View(komentari);
        }

        public ActionResult Odobri(int id)
        {
            var komentar = komentarRepo.GetAll().FirstOrDefault(k => k.Id == id);
            if (komentar != null)
            {
                komentar.Odobren = true;
                komentarRepo.Update(komentar);
            }
            return RedirectToAction("ListaZaOdobravanje");
        }

       
        
        public PartialViewResult PublicComments(string smestajNaziv)
        {
            var komentari = komentarRepo.GetAll()
                .Where(k => k.SmestajNaziv == smestajNaziv && k.Odobren)
                .ToList();
            return PartialView("_KomentariList", komentari);
        }

    }
}

