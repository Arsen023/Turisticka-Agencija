using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MyWebApp.Models;
using Newtonsoft.Json;

namespace MyWebApp.Repositories
{
    public class AranzmanRepository
    {
        private readonly string filePath =
            HttpContext.Current.Server.MapPath("~/App_Data/aranzmani.json");

        public List<Aranzman> GetAll()
        {
            if (!File.Exists(filePath)) return new List<Aranzman>();

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Aranzman>>(json,
                new JsonSerializerSettings { DateFormatString = "dd/MM/yyyy" }
            ) ?? new List<Aranzman>();
        }

        private void SaveAll(List<Aranzman> data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings { DateFormatString = "dd/MM/yyyy" }
            );
            File.WriteAllText(filePath, json);
        }

        public void Add(Aranzman a)
        {
            var list = GetAll();
            if (a.Id == 0) a.Id = list.Any() ? list.Max(x => x.Id) + 1 : 1;
            list.Add(a);
            SaveAll(list);
        }

        public Aranzman FindById(int id) => GetAll().FirstOrDefault(a => a.Id == id);

        public Aranzman FindByNaziv(string naziv) =>
            GetAll().FirstOrDefault(a => a.Naziv.Equals(naziv, StringComparison.OrdinalIgnoreCase));

    
        public void Update(Aranzman updated)
        {
            var list = GetAll();
            var index = list.FindIndex(a => a.Id == updated.Id);
            if (index != -1)
            {
                list[index] = updated;
                SaveAll(list);
            }
        }

        
        public void SoftDelete(int id)
        {
            var list = GetAll();
            var a = list.FirstOrDefault(x => x.Id == id);
            if (a != null)
            {
                a.IsDeleted = true;
                SaveAll(list);
            }
        }
        public bool RemoveSmestajFromAll(int smestajId)
        {
            var list = GetAll();
            bool changed = false;

            foreach (var a in list)
            {
                if (a.SmestajId != null && a.SmestajId.Remove(smestajId))
                    changed = true;
            }

            if (changed)
            {
             
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.JsonSerializerSettings { DateFormatString = "dd/MM/yyyy" });
                System.IO.File.WriteAllText(
                    System.Web.HttpContext.Current.Server.MapPath("~/App_Data/aranzmani.json"), json);
            }

            return changed;
        }

   
        public void Remove(int id)
        {
            var list = GetAll();
            list.RemoveAll(x => x.Id == id);
            SaveAll(list);
        }
    }
}

