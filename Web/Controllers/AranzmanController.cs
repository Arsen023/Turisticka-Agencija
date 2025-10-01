using MyWebApp.Models;
using MyWebApp.Service;
using System.Web.Mvc;

namespace MyWebApp.Controllers
{
    public class AranzmanController : Controller
    {
        private readonly AranzmanService aranzmanService = new AranzmanService();

      
        public ActionResult Index()
        {
            var aranzmani = aranzmanService.GetAll();
            return View(aranzmani);
        }

        
        public ActionResult Details(int id)
        {
            var aranzman = aranzmanService.GetById(id);
            if (aranzman == null) return HttpNotFound();

           
            return View("~/Views/Home/Detalji.cshtml", aranzman);
        }
    }
}

