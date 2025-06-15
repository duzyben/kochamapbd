using System.Data.Common;
using System.Data.SqlClient;
using RetakeTest1.Data.Dtos;
using RetakeTest1.Exceptions;

namespace RetakeTest1.Services;

public class DbService : IDbService
{
    private readonly string _connectionString;

    public DbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    }


    public async Task<GetCustomerDataByIdDto> GetCustomerById(int clientId)
    {
        var query =
            @"SELECT c.ID, c.FirstName, c.LastName, c.Address, car.VIN, coll.Name, m.Name, cr.DateFrom, cr.DateTo, cr.TotalPrice
            FROM car_rentals cr
            JOIN clients c ON cr.ClientId = c.ID
            JOIN cars car ON cr.CarId = car.ID
            JOIN models m ON car.ModelId = m.ID
            JOIN colors coll ON car.ColorID = coll.ID
            WHERE c.Id = @clientId";
        
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        await connection.OpenAsync();
        
        command.Parameters.AddWithValue("@clientId", clientId);
        var reader = await command.ExecuteReaderAsync();

        GetCustomerDataByIdDto? client = null;
        
        while (await reader.ReadAsync())
        {
            if (client is null)
            {
                client = new GetCustomerDataByIdDto
                {
                    Id = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Address = reader.GetString(3),
                    Rentals = new List<RentalInfoDto>()
                };
            }

            var rental = new RentalInfoDto
            {
                VIN = reader.GetString(4),
                Color = reader.GetString(5),
                Model = reader.GetString(6),
                DateFrom = reader.GetDateTime(7),
                DateTo = reader.GetDateTime(8),
                TotalPrice = reader.GetInt32(9),
            };
            client.Rentals.Add(rental);
        }
        if (client is null)
        {
            throw new NotFoundException("No rentals data found.");
        }
        
        return client;
    }

    public async Task<int> AddNewClient(AddClientDto client)
    {
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            command.Parameters.Clear();
            command.CommandText =
                @"INSERT INTO clients
            VALUES(@FirstName, @LastName, @Address);
            SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@FirstName", client.ClientInfo.FirstName);
            command.Parameters.AddWithValue("@LastName", client.ClientInfo.LastName);
            command.Parameters.AddWithValue("@Address", client.ClientInfo.Address);
            var result = await command.ExecuteScalarAsync();
            int newClientId = Convert.ToInt32(result);


            command.Parameters.Clear();
            command.CommandText = "SELECT PricePerDay FROM cars WHERE ID = @CarId;";
            command.Parameters.AddWithValue("@CarId", client.Car.CarId);

            var priceForCar = await command.ExecuteScalarAsync();
            if (priceForCar is null)
            {
                throw new NotFoundException("Car with this id doesnt exist");
            }

            int days = (client.Car.DateTo - client.Car.DateFrom).Days;
            int price = days * (int)priceForCar;

            command.Parameters.Clear();
            command.CommandText =
                @"INSERT INTO car_rentals
                        VALUES(@ClientID, @CarID, @DateFrom, @DateTo, @TotalPrice);
                        SELECT SCOPE_IDENTITY()";

            command.Parameters.AddWithValue("@ClientID", newClientId);
            command.Parameters.AddWithValue("@CarID", client.Car.CarId);
            command.Parameters.AddWithValue("@DateFrom", client.Car.DateFrom);
            command.Parameters.AddWithValue("@DateTo", client.Car.DateTo);
            command.Parameters.AddWithValue("@TotalPrice", price);

            await command.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
            return newClientId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}