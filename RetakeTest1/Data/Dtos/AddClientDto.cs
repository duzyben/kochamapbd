namespace RetakeTest1.Data.Dtos;

public class AddClientDto
{
    public ClientInfoDto ClientInfo { get; set; } = new ClientInfoDto();
    public CarInfoDto Car { get; set; } = new CarInfoDto();
    
}

public class ClientInfoDto
{
    public String FirstName { get; set; } = String.Empty;
    public String LastName { get; set; } = String.Empty;
    public String Address { get; set; } = String.Empty;
}

public class CarInfoDto
{
    public int CarId { get; set; }
    public DateTime DateFrom { get; set; } = DateTime.Today;
    public DateTime DateTo { get; set; } = DateTime.Today;
}