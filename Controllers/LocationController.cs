using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialBrothers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SocialBrothers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {

        private readonly SocialBrothersContext _context;
        double distance = 0;
        public LocationController(SocialBrothersContext context)
        {
            _context = context;
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
        // GET: api/location/{id_location1}/{id_location2}
        [HttpGet]
        [Route("{id1}/{id2}")]
        public async Task<double> GetDistance(int id1, int id2)
        {

            var loc1 = await _context.Adresses.FindAsync(id1);
            var loc2 = await _context.Adresses.FindAsync(id2);
            Location location1 = await RunAsync(loc1.Zipcode);
            Location location2 = await RunAsync(loc2.Zipcode);
            //get distance between these two locations
            if ((location1.Lat == location2.Lat) && (location1.Lon == location2.Lon))
            {
                return 0;
            }
            else
            {
                GeoCoordinate pin1 = new GeoCoordinate(location1.Lat, location1.Lon);
                GeoCoordinate pin2 = new GeoCoordinate(location2.Lat, location2.Lon);

                double distanceBetween = pin1.GetDistanceTo(pin2);
                Console.WriteLine(distanceBetween);
                return (distanceBetween);
            }


            static async Task<Location> RunAsync(string addressLine)
            {
                using (var client = new HttpClient())
                {
                    string bingkey = "AsOwuPhU8FPK9TfxRKzV2wXAyywiAIM6fDnMlqNgTbQ-fWcUdVfZcr_FSkZGEeYv";
                    var _addressLine = addressLine;

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
                                lat = jsonObject.resourceSets[0].resources[0].point.coordinates[0];
                                lon = jsonObject.resourceSets[0].resources[0].point.coordinates[1];
                                Location l = new Location(lat, lon);
                                return l;
                            }
                        }
                    }
                }
                return null;
            }
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
