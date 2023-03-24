using App.Core;
using App.Domain;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace App.Database
{
	public static class Seed
	{
        public static async Task SeedAsync()
		{
			var db = new DataContext();

            if (!db.aliens.Any())
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string jsonMock = await System.IO.File.ReadAllTextAsync(Path.Combine(currentDirectory, "Mock", "mock.json"));
                var mockData = JsonConvert.DeserializeObject<RootObject>(jsonMock);

                if(mockData != null)
                {
                    db.AddRange(mockData.aliens);
                    db.AddRange(mockData.sightings);
                    await db.SaveChangesAsync();
                }
                
            }
        }
	}

    public class RootObject
    {
        public List<alienEntity> aliens { get; set; }
        public List<sightingEntity> sightings { get; set; }
    }
}
