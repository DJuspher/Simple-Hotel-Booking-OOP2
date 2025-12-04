using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BookingApp.Storage
{
    public class JsonStorage : IStorage
    {
        private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

        public void Save<T>(string filename, IEnumerable<T> items)
        {
            try
            {
                var folder = Path.Combine(AppContext.BaseDirectory, "Data");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, filename);
                var json = JsonSerializer.Serialize(items, _opts);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Storage Error] Save failed: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<T> Load<T>(string filename)
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Data", filename);
                if (!File.Exists(path)) return new List<T>();

                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<T>>(json, _opts) ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Storage Error] Load failed: {ex.Message}");
                return new List<T>();
            }
        }
    }
}
