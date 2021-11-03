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
        public async Task<string> PostAdress(Adress adress)
        {
            var location = await RunAsync(adress.Zipcode);
            if (location != null)
            {
                _context.Adresses.Add(adress);
                await _context.SaveChangesAsync();
                var res = CreatedAtAction("GetAdress", new { id = adress.ID }, adress);
                return "Address created";
            }
            return "Sorry, your zipcode is empty or invalid. Make sure you use a Dutch zipcode [1234AB].";
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

        static async Task<Location> RunAsync(string addressLine)
        {
            using (var client = new HttpClient())
            {
                string bingkey = "AsOwuPhU8FPK9TfxRKzV2wXAyywiAIM6fDnMlqNgTbQ-fWcUdVfZcr_FSkZGEeYv";
                var _addressLine = addressLine;
                if(addressLine == "")
                {
                    return null;
                }
                //http://dev.virtualearth.net/REST/v1/Locations?countryRegion={countryRegion}&adminDistrict={adminDistrict}&locality={locality}&postalCode={postalCode}&addressLine={addressLine}&userLocation={userLocation}&userIp={userIp}&usermapView={usermapView}&includeNeighborhood={includeNeighborhood}&maxResults={maxResults}&key={BingMapsKey}

                //TODO - send HTTP requests
                client.BaseAddress = new Uri("http://dev.virtualearth.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //-----
                //vervang NL door variabele country omgezet naar NL, UK, US, etc.
                HttpResponseMessage response = await client.GetAsync("REST/v1/Locations/NL/" + _addressLine + "/?o=json&key=" + bingkey);
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
                            if (jsonObject.resourceSets[0].resources[0].name != "Netherlands")
                            {
                                lat = jsonObject.resourceSets[0].resources[0].point.coordinates[0];
                                lon = jsonObject.resourceSets[0].resources[0].point.coordinates[1];
                                Location l = new Location(lat, lon);
                                return l;
                            }
                            return null;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            Console.WriteLine("Sorry, we only support Dutch addresses.");
            return null;
        }
        public class Location
        {
            public Location(double lat, double lon)
            {
                Lat = lat;
                Lon = lon;
            }
            public double Lat { get; }
            public double Lon { get; }
        }
    }
}
