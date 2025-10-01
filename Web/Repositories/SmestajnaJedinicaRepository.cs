using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using MyWebApp.Models;
using Newtonsoft.Json;

namespace MyWebApp.Repositories
{
    public class SmestajnaJedinicaRepository
    {
        private readonly string filePath = HttpContext.Current.Server.MapPath("~/App_Data/smestajneJedinice.json");

        public List<SmestajnaJedinica> GetAll()
        {
            if (!File.Exists(filePath))
                return new List<SmestajnaJedinica>();

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<SmestajnaJedinica>>(json) ?? new List<SmestajnaJedinica>();
        }

        public void SaveAll(List<SmestajnaJedinica> jedinice)
        {
            var json = JsonConvert.SerializeObject(jedinice, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void Add(SmestajnaJedinica jedinica)
        {
            var jedinice = GetAll();
            jedinice.Add(jedinica);
            SaveAll(jedinice);
        }

        public SmestajnaJedinica FindById(int id)
        {
            return GetAll().FirstOrDefault(j => j.Id == id);
        }

        public void Update(SmestajnaJedinica updated)
        {
            var jedinice = GetAll();
            var index = jedinice.FindIndex(j => j.Id == updated.Id);
            if (index != -1)
            {
                jedinice[index] = updated;
                SaveAll(jedinice);
            }
        }
    }
}

