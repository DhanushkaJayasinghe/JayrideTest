namespace Jayride.API.Models
{
    public class VehicleCost
    {
        public string VehicleName { get; set; }
        public string VehicleTypeName { get; set; }
        public int MaxPassengers { get; set; }
        public decimal PricePerPassenger { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
