using Exercise3.Models;
using Exercise3.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

namespace Exercise3.Repositories
{
    public interface IAnimalsRepository 
    {
        Task<ICollection<Animal>> GetAnimals(string orderBy);
        Task AddAnimal(AnimalPOST animalPOST);
        Task<bool> DoesAnimalExists(int ID);
        Task<Animal> GetAnimalById(int ID);
        Task<AnimalPOST> UpdateAnimal(int ID, AnimalUpdate animal);
        Task DeleteAnimal(int ID);
    }

    public class AnimalsRepository : IAnimalsRepository
    {
        private readonly string _connectionString; 
        public AnimalsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task AddAnimal(AnimalPOST animalPOST)
        {
            var query = $"INSERT INTO [dbo].[Animal] ([ID], [Name], [Description], [Category], [Area]) VALUES (@ID, @Name, @Description, @Category, @Area)";

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", animalPOST.ID);
                command.Parameters.AddWithValue("@Name", animalPOST.Name);
                command.Parameters.AddWithValue("@Description", animalPOST.Description);
                command.Parameters.AddWithValue("@Category", animalPOST.Category);
                command.Parameters.AddWithValue("@Area", animalPOST.Area);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAnimal(int ID)
        {
            var query = $"DELETE FROM Animal WHERE ID=@ID";

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", ID);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DoesAnimalExists(int ID)
        {
            var query = $"SELECT * FROM Animal WHERE ID = {ID}";

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                await connection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<Animal> GetAnimalById(int ID)
        {
            var query = $"SELECT * FROM Animal WHERE ID = {ID}";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows) 
                    {
                        return null;
                    }

                    int tempID = reader.GetOrdinal("ID");
                    int Name = reader.GetOrdinal("Name");
                    int Description = reader.GetOrdinal("Description");
                    int Category = reader.GetOrdinal("Category");
                    int Area = reader.GetOrdinal("Area");

                    await reader.ReadAsync();

                    return new Animal
                    {
                        ID = reader.GetInt32(tempID),
                        Name = reader.GetString(Name),
                        Description = reader.GetString(Description),
                        Category = reader.GetString(Category),
                        Area = reader.GetString(Area)
                    };
                }
            }
        }

        public async Task<ICollection<Animal>> GetAnimals(string orderBy)
        {
            var query = $"SELECT * FROM Animal ORDER BY {orderBy}";
            var animals = new List<Animal>();

            using (var connection = new SqlConnection(_connectionString)) 
            {
                SqlCommand command = new SqlCommand(query, connection);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync()) 
                {
                    int ID = reader.GetOrdinal("ID");
                    int Name = reader.GetOrdinal("Name");
                    int Description = reader.GetOrdinal("Description");
                    int Category = reader.GetOrdinal("Category");
                    int Area = reader.GetOrdinal("Area");

                    while (await reader.ReadAsync()) {
                        animals.Add(new Animal
                        {
                            ID = reader.GetInt32(ID),
                            Name = reader.GetString(Name),
                            Description = reader.GetString(Description),
                            Category = reader.GetString(Category),
                            Area = reader.GetString(Area)
                        });
                    }
                }
            }
            return animals;
        }

        public async Task<AnimalPOST> UpdateAnimal(int ID, AnimalUpdate animal)
        {
            var query = $"UPDATE Animal SET ID=@ID, Name=@Name, Description=@Description, Category=@Category, Area=@Area WHERE ID=@ID";

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@Name", animal.Name.ToString());
                command.Parameters.AddWithValue("@Description", animal.Description);
                command.Parameters.AddWithValue("@Category", animal.Category);
                command.Parameters.AddWithValue("@Area", animal.Area);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            return new AnimalPOST
            {
                ID = ID,
                Name = animal.Name,
                Description = animal.Description,
                Category = animal.Category,
                Area = animal.Area
            };
        }
    }
}
