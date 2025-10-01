using MyWebApp.Repositories;
using System.Web.Mvc;
using System.Linq;

namespace MyWebApp.Controllers
{
    public class SmestajController : Controller
    {
        private readonly SmestajRepository smestajRepo = new SmestajRepository();

        public ActionResult DetaljiJedinice(int jedinicaId)
        {
            var jedinica = smestajRepo.GetAll()
                .SelectMany(s => s.SmestajneJedinicee)
                .FirstOrDefault(j => j.Id == jedinicaId);
            if (jedinica == null) return HttpNotFound();
            return PartialView("_SmestajnaJedinica", jedinica);
        }
    }
}

