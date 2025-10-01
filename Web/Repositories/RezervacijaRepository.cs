using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using MyWebApp.Models;
using Newtonsoft.Json;


namespace MyWebApp.Repositories
{
    public class RezervacijaRepository
    {
        private readonly string filePath = HttpContext.Current.Server.MapPath("~/App_Data/rezervacije.json");

        public List<Rezervacija> GetAll()
        {
            if (!File.Exists(filePath))
                return new List<Rezervacija>();

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Rezervacija>>(json) ?? new List<Rezervacija>();
        }

        public void SaveAll(List<Rezervacija> rezervacije)
        {
            var json = JsonConvert.SerializeObject(rezervacije, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void Add(Rezervacija rezervacija)
        {
            var rezervacije = GetAll();
            rezervacije.Add(rezervacija);
            SaveAll(rezervacije);
        }

        public Rezervacija FindById(int id)
        {
            return GetAll().FirstOrDefault(r => r.Id == id);
        }


        public void Update(Rezervacija updated)
        {
            var rezervacije = GetAll();
            var index = rezervacije.FindIndex(r => r.Id == updated.Id);
            if (index != -1)
            {
                rezervacije[index] = updated;
                SaveAll(rezervacije);
            }
        }
        

    }
}

