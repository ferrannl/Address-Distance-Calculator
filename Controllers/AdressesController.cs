using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SocialBrothers.Models;

namespace SocialBrothers.Controllers
{

    //https://localhost:44361/api/Adresses
    [Route("api/[controller]")]
    [ApiController]
    public class AdressesController : ControllerBase
    {

        private readonly SocialBrothersContext _context;

        public AdressesController(SocialBrothersContext context)
        {
            _context = context;
        }

        // GET: api/Adresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Adress>>> GetAdresses()
        {
            RunAsync().Wait();

            return await _context.Adresses.ToListAsync();
        }

        // GET: api/Adresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Adress>> GetAdress(int id)
        {
            var adress = await _context.Adresses.FindAsync(id);

            if (adress == null)
            {
                return NotFound();
            }

            return adress;
        }

        // PUT: api/Adresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdress(int id, Adress adress)
        {
            if (id != adress.ID)
            {
                return BadRequest();
            }

            _context.Entry(adress).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Adresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Adress>> PostAdress(Adress adress)
        {
            _context.Adresses.Add(adress);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdress", new { id = adress.ID }, adress);
        }

        // DELETE: api/Adresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdress(int id)
        {
            var adress = await _context.Adresses.FindAsync(id);
            if (adress == null)
            {
                return NotFound();
            }

            _context.Adresses.Remove(adress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdressExists(int id)
        {
            return _context.Adresses.Any(e => e.ID == id);
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                string bingkey = "AsOwuPhU8FPK9TfxRKzV2wXAyywiAIM6fDnMlqNgTbQ-fWcUdVfZcr_FSkZGEeYv";
                var addressLine = "Marskramersgilde";

                //TODO - send HTTP requests
                client.BaseAddress = new Uri("http://dev.virtualearth.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //-----
                HttpResponseMessage response = await client.GetAsync("REST/v1/Locations?addressLine=" + addressLine + "&key=" + bingkey);
                double lat = 0;
                double lon = 0;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    dynamic jsonObject = JsonConvert.DeserializeObject(json);
                    if (jsonObject.resourceSets.Count > 0)
                    {
                        if (jsonObject.resourceSets[0].resources.Count > 0)
                        {
                            lat = jsonObject.resourceSets[0].resources[0].point.coordinates[0];
                            lon = jsonObject.resourceSets[0].resources[0].point.coordinates[1];
                        }
                    }
                }
            }
        }

        private Location getLocation()
        {
            Location location = new Location();
            var addressLine = "Marskramersgilde";
            //var bingurl = "http://dev.virtualearth.net/REST/v1/Locations?addressLine=" + addressLine + "&key=" + bingkey;
            location.Latitude = 12;
            location.Longitude = 13;

            return location;
        }


        //public double CalculateDistance(Location point1, Location point2)
        //{
        //    var d1 = point1.Latitude * (Math.PI / 180.0);
        //    var num1 = point1.Longitude * (Math.PI / 180.0);
        //    var d2 = point2.Latitude * (Math.PI / 180.0);
        //    var num2 = point2.Longitude * (Math.PI / 180.0) - num1;
        //    var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
        //             Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
        //    return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        //}
        public class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

    }
}
