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
    //https://localhost:44361/api/Locations

    [Route("api/[controller]")]
    [ApiController]
    public class LocationController2 : ControllerBase
    {

        private readonly SocialBrothersContext _context;

        public LocationController2(SocialBrothersContext context)
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
        [Route("api/location/{id:int}/{id2:int}")]
        public async Task<double> GetDistance(string id1, string id2)
        {
            double distance = 0;
            var loc1 = await _context.Adresses.FindAsync(id1);
            var loc2 = await _context.Adresses.FindAsync(id1);
            Location location1 = await RunAsync(loc1.City);
            Location location2 = await RunAsync(loc2.City);
            //get distance between these two locations
            if ((location1.Lat == location2.Lat) && (location1.Lon == location2.Lon))
            {
                return 0;
            }
            else
            {
                double theta = location1.Lon - location2.Lon;
                double dist = Math.Sin(deg2rad(location1.Lat)) * Math.Sin(deg2rad(location2.Lat)) + Math.Cos(deg2rad(location1.Lon)) * Math.Cos(deg2rad(location2.Lon)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                var unit = 'K';
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                Console.WriteLine(dist);
                return (dist);
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
                    HttpResponseMessage response = await client.GetAsync("REST/v1/Locations/" + _addressLine + "/?o=json&key=" + bingkey);
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
