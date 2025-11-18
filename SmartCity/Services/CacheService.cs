using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartCity.Services
{
    public static class CacheService
    {
        public static string GetPath(string name) =>
            Path.Combine(FileSystem.AppDataDirectory, name);

        public static async Task SaveAsync<T>(string name, T obj)
        {
            var path = GetPath(name);
            var text = JsonSerializer.Serialize(obj);
            await File.WriteAllTextAsync(path, text);
        }

        public static async Task<T?> LoadAsync<T>(string name)
        {
            var path = GetPath(name);
            if (!File.Exists(path)) return default;
            var text = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<T>(text);
        }
    }
}
