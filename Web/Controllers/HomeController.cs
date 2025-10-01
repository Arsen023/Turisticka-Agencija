using MyWebApp.Models;
using MyWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AranzmanRepository aranzmanRepo = new AranzmanRepository();
        private readonly KomentarRepository komentariRepo = new KomentarRepository();
        private readonly SmestajRepository smestajRepo = new SmestajRepository();
        private readonly SmestajnaJedinicaRepository jedinicaRepo = new SmestajnaJedinicaRepository();

      
        public ActionResult Index(
            string naziv,
            string tipPrevoza,
            string tipAranzmana,
            DateTime? datumOd,
            DateTime? datumDo,
            DateTime? krajOd,
            DateTime? krajDo,
            string sort
        )
        {
            var aranzmani = aranzmanRepo.GetAll();

            
            if (!string.IsNullOrEmpty(naziv))
                aranzmani = aranzmani
                    .Where(a => a.Naziv.ToLower().Contains(naziv.ToLower()))
                    .ToList();

            if (!string.IsNullOrEmpty(tipPrevoza))
                aranzmani = aranzmani
                    .Where(a => a.tipPrevoza.ToString() == tipPrevoza)
                    .ToList();

            if (!string.IsNullOrEmpty(tipAranzmana))
                aranzmani = aranzmani
                    .Where(a => a.tipAranzmana.ToString() == tipAranzmana)
                    .ToList();

            if (datumOd.HasValue)
                aranzmani = aranzmani
                    .Where(a => a.DatumPocetkaPutovanja >= datumOd.Value)
                    .ToList();

            if (datumDo.HasValue)
                aranzmani = aranzmani
                    .Where(a => a.DatumPocetkaPutovanja <= datumDo.Value)
                    .ToList();

            if (krajOd.HasValue)
                aranzmani = aranzmani
                    .Where(a => a.DatumZavrsetkaPutovanja >= krajOd.Value)
                    .ToList();

            if (krajDo.HasValue)
                aranzmani = aranzmani
                    .Where(a => a.DatumZavrsetkaPutovanja <= krajDo.Value)
                    .ToList();

           
            switch (sort)
            {
                case "nazivAsc":
                    aranzmani = aranzmani.OrderBy(a => a.Naziv).ToList();
                    break;
                case "nazivDesc":
                    aranzmani = aranzmani.OrderByDescending(a => a.Naziv).ToList();
                    break;
                case "pocetakAsc":
                    aranzmani = aranzmani.OrderBy(a => a.DatumPocetkaPutovanja).ToList();
                    break;
                case "pocetakDesc":
                    aranzmani = aranzmani.OrderByDescending(a => a.DatumPocetkaPutovanja).ToList();
                    break;
                case "krajAsc":
                    aranzmani = aranzmani.OrderBy(a => a.DatumZavrsetkaPutovanja).ToList();
                    break;
                case "krajDesc":
                    aranzmani = aranzmani.OrderByDescending(a => a.DatumZavrsetkaPutovanja).ToList();
                    break;
            }

            return View(aranzmani);
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

        
        public ActionResult Detalji(string naziv)
        {
            if (string.IsNullOrWhiteSpace(naziv))
                return RedirectToAction("Index");

            var aranzman = aranzmanRepo.GetAll()
                .FirstOrDefault(a => a.Naziv.Equals(naziv, StringComparison.OrdinalIgnoreCase));

            if (aranzman == null)
                return HttpNotFound();

            
            var smestaji = new List<Smestaj>();

            if (aranzman.SmestajId != null && aranzman.SmestajId.Any())
            {
                foreach (var smId in aranzman.SmestajId)
                {
                    var sm = smestajRepo.GetById(smId);
                    if (sm != null)
                    {
                      
                        sm.SmestajneJedinicee = jedinicaRepo.GetAll()
                            .Where(j => j.SmestajID == sm.Id)
                            .ToList();


                        
                        sm.Komentari = komentariRepo.GetAll()
                            .Where(k => k.SmestajNaziv.Equals(sm.Naziv, StringComparison.OrdinalIgnoreCase) && k.Odobren)
                            .ToList();

                        smestaji.Add(sm);
                    }
                }
            }

            ViewBag.Smestaji = smestaji;
            return View(aranzman);
        }
    }
}