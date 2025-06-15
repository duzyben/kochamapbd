namespace RetakeTest1.Data.Dtos;

public class GetCustomerDataByIdDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Address { get; set; } = String.Empty;
    public List<RentalInfoDto> Rentals { get; set; } = new List<RentalInfoDto>();
}

public class RentalInfoDto
{
    public String VIN { get; set; } = String.Empty;
    public string Color { get; set; } = String.Empty;
    public string Model {get; set; } = String.Empty;
    public DateTime DateFrom { get; set; } = DateTime.Today;
    public DateTime DateTo { get; set; } = DateTime.Today;
    public int TotalPrice { get; set; }
}