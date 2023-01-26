using System.Collections.Generic;

namespace Jayride.API.Models
{
    public class Transport
    {
        public string From { get; set; }
        public string To { get; set; }
        public List<Vehicle> Listings { get; set; }
    }
}
