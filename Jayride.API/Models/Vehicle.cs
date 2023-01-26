namespace Jayride.API.Models
{
    public class Vehicle
    {
        public string Name { get; set; }
        public decimal PricePerPassenger { get; set; }
        public VehicleType VehicleType { get; set; }
    }
}
