using MyWebApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyWebApp.Repositories
{
    public class KorisnikRepository
    {
        private readonly string filePath = HttpContext.Current.Server.MapPath("~/App_Data/korisnici.json");

      
        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy",
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            NullValueHandling = NullValueHandling.Ignore
        };

        public List<Korisnik> GetAll()
        {
            if (!File.Exists(filePath))
                return new List<Korisnik>();

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Korisnik>>(json, jsonSettings) ?? new List<Korisnik>();
        }

        public void SaveAll(List<Korisnik> korisnici)
        {
            var json = JsonConvert.SerializeObject(korisnici, Formatting.Indented, jsonSettings);
            File.WriteAllText(filePath, json);
        }

        public void Add(Korisnik korisnik)
        {
            var korisnici = GetAll();
            korisnici.Add(korisnik);
            SaveAll(korisnici);
        }

        public Korisnik FindByUsername(string username)
        {
            return GetAll().FirstOrDefault(k => k.KorisnickoIme == username);
        }

       
        public Korisnik FindByUsernameAndPassword(string username, string password)
        {
            return GetAll().FirstOrDefault(k => k.KorisnickoIme == username && k.Lozinka == password);
        }

        public void Update(Korisnik updated)
        {
            var korisnici = GetAll();
            var index = korisnici.FindIndex(k => k.KorisnickoIme == updated.KorisnickoIme);
            if (index != -1)
            {
                korisnici[index] = updated;
                SaveAll(korisnici);
            }
        }
    }
}

