using RetakeTest1.Data.Dtos;

namespace RetakeTest1.Services;

public interface IDbService
{
    Task<GetCustomerDataByIdDto> GetCustomerById(int clientId);
    
    Task<int> AddNewClient(AddClientDto client);
}