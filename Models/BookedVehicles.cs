public class BookedVehicles
{
    public int Id { get; set; } 
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserPhoneNumber { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string Location { get; set; } = string.Empty;
}
