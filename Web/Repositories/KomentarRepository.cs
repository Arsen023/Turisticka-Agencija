using MyWebApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyWebApp.Repositories
{
    public class KomentarRepository
    {
        private readonly string filePath = HttpContext.Current.Server.MapPath("~/App_Data/komentari.json");

        public List<Komentar> GetAll()
        {
            if (!File.Exists(filePath))
                return new List<Komentar>();

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Komentar>>(json) ?? new List<Komentar>();
        }

        public void SaveAll(List<Komentar> komentari)
        {
            var json = JsonConvert.SerializeObject(komentari, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void Add(Komentar komentar)
        {
            var komentari = GetAll();
            komentar.Id = komentari.Any() ? komentari.Max(k => k.Id) + 1 : 1;
            komentari.Add(komentar);
            SaveAll(komentari);
        }

        public void Update(Komentar komentar)
        {
            var komentari = GetAll();
            var index = komentari.FindIndex(k => k.Id == komentar.Id);
            if (index != -1)
            {
                komentari[index] = komentar;
                SaveAll(komentari);
            }
        }
    }
}

