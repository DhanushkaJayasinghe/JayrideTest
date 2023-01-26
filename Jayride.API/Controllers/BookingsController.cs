using Jayride.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jayride.API.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICommonFunctions _commonFunctions;
        public BookingsController(IConfiguration configuration, ICommonFunctions commonFunctions) {
            _configuration = configuration;
            _commonFunctions = commonFunctions;
        }

        [HttpGet("candidate")]
        public ActionResult<Candidate> GetAsync() {
            try {
                var candidate = new Candidate { Name = "test", Phone = "test" };
                return Ok(candidate);
            }
            catch (Exception) {
                return BadRequest();
            }
        }

        [HttpGet("location")]
        public async Task<ActionResult<string>> Get() {
            try {
                //Get public IP
                var ip = _commonFunctions.GetPublicIpAddress();

                if (string.IsNullOrWhiteSpace(ip)) {
                    return NoContent();
                }

                //Get IP Stack Access key
                var ipStackAccessKey = _configuration.GetValue<string>("IpStackAccessKey");

                string apiUrl = "http://api.ipstack.com/" + ip + "?access_key=" + ipStackAccessKey + "&fields=city";
                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode) {
                        var city = await response.Content.ReadAsStringAsync();
                        if (city != null && !string.IsNullOrWhiteSpace(city)) {
                            return Ok(city);
                        }
                    }
                }
                return NoContent();
            }
            catch (Exception) {
                return BadRequest();
            }
        }

        [HttpGet("listings")]
        public async Task<ActionResult<List<VehicleCost>>> Get(int numberOfPassengers) {
            var availableVehicles = new List<VehicleCost>();

            if (numberOfPassengers <= 0) {
                //Can return validations
                return NoContent();
            }

            try {
                string apiUrl = "https://jayridechallengeapi.azurewebsites.net/api/QuoteRequest";
                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode) {
                        var data = await response.Content.ReadAsStringAsync();
                        if (data != null) {
                            var transportOptions = JsonConvert.DeserializeObject<Transport>(data);
                            if (transportOptions != null) {
                                var listings = transportOptions.Listings.Where(m => m.VehicleType.MaxPassengers >= numberOfPassengers).OrderBy(m => m.PricePerPassenger).ToList();
                                if (listings.Any()) {
                                    foreach (var vehicle in listings) {
                                        availableVehicles.Add(new VehicleCost {
                                            VehicleName = vehicle.Name,
                                            VehicleTypeName = vehicle.VehicleType.Name,
                                            MaxPassengers = vehicle.VehicleType.MaxPassengers,
                                            PricePerPassenger = vehicle.PricePerPassenger,
                                            TotalPrice = vehicle.PricePerPassenger * numberOfPassengers
                                        });
                                    }
                                    return Ok(availableVehicles);
                                }
                            }
                        }
                    }
                }
                return NoContent();
            }
            catch (Exception) {
                return BadRequest();
            }
        }
    }
}
