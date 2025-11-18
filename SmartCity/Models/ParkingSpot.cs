namespace SmartCity.Models
{
    public class ParkingSpot
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Hours { get; set; } = "Not Available";
        public string Notes { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Lon { get; set; }
        public decimal Price { get; set; }
        public int AvailableSpots { get; set; } = -1;
    }
}