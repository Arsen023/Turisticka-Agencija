using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MyWebApp.Models;
using MyWebApp.Service;
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

            try
            {
                var json = File.ReadAllText(filePath);
                
                // Prvo probaj sa CustomDateConverter
                try
                {
                    var settings = new JsonSerializerSettings 
                    { 
                        DateFormatString = "dd/MM/yyyy",
                        Converters = new List<JsonConverter> { new MyWebApp.Service.CustomDateConverter() }
                    };
                    return JsonConvert.DeserializeObject<List<Aranzman>>(json, settings) ?? new List<Aranzman>();
                }
                catch (FormatException)
                {
                    // Ako CustomDateConverter ne radi, probaj bez njega
                    var settings = new JsonSerializerSettings 
                    { 
                        DateFormatString = "dd/MM/yyyy"
                    };
                    return JsonConvert.DeserializeObject<List<Aranzman>>(json, settings) ?? new List<Aranzman>();
                }
            }
            catch (Exception ex)
            {
                // Log grešku - možete dodati logovanje ovde
                System.Diagnostics.Debug.WriteLine($"Greška pri čitanju aranzmana: {ex.Message}");
                return new List<Aranzman>();
            }
        }

        private void SaveAll(List<Aranzman> data)
        {
            var settings = new JsonSerializerSettings 
            { 
                DateFormatString = "dd/MM/yyyy",
                Converters = new List<JsonConverter> { new MyWebApp.Service.CustomDateConverter() }
            };
            var json = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
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
                var settings = new JsonSerializerSettings 
                { 
                    DateFormatString = "dd/MM/yyyy",
                    Converters = new List<JsonConverter> { new MyWebApp.Service.CustomDateConverter() }
                };
                var json = JsonConvert.SerializeObject(list, Formatting.Indented, settings);
                File.WriteAllText(filePath, json);
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

