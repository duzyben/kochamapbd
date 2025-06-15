namespace RetakeTest1.Data.Dtos;

public class AddClientDto
{
    public String FirstName { get; set; } = String.Empty;
    public String LastName { get; set; } = String.Empty;
    public String Address { get; set; } = String.Empty;
    public CarInfoDto Car { get; set; } = new CarInfoDto();
    
}

public class CarInfoDto
{
    public int CarId { get; set; }
    public DateTime DateFrom { get; set; } = DateTime.Today;
    public DateTime DateTo { get; set; } = DateTime.Today;
}