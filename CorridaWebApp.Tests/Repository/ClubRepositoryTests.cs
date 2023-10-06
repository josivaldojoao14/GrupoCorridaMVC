using CorridaWebApp.Data;
using CorridaWebApp.Data.Enum;
using CorridaWebApp.Models;
using CorridaWebApp.Repository;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace CorridaWebApp.Tests.Repository
{
  public class ClubRepositoryTests
  {
    private async Task<ApplicationDbContext> GetDbContext()
    {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

      var databaseContext = new ApplicationDbContext(options);
      databaseContext.Database.EnsureCreated();
      if (await databaseContext.Clubs.CountAsync() < 0)
      {
        for (var i = 0; i < 10; i++)
        {
          databaseContext.Add(
          new Club()
          {
            Title = "Running Club 1",
            Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
            Description = "This is the description of the first cinema",
            ClubCategory = ClubCategory.City,
            Address = new Address()
            {
              StreetName = "123 Main St",
              City = "Charlotte",
              State = "NC"
            }
          });
          await databaseContext.SaveChangesAsync();
        }
      }
      return databaseContext;
    }

    [Fact]
    public async void ClubRepository_Add_ReturnsBool()
    {
      // Arrange
      var club = new Club()
      {
        Title = "Running Club 1",
        Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
        Description = "This is the description of the first cinema",
        ClubCategory = ClubCategory.City,
        Address = new Address()
        {
          StreetName = "123 Main St",
          City = "Charlotte",
          State = "NC"
        }
      };
      var dbContext = await GetDbContext();
      var clubRepository = new ClubRepository(dbContext);

      // Act
      var result = clubRepository.Add(club);

      // Assert
      result.Should().BeTrue();
    }

    [Fact]
    public async void ClubRepository_GetByIdAsync_ReturnsClub()
    {
      // ARRANGE
      var id = 1;
      var dbContext = await GetDbContext();
      var clubRepository = new ClubRepository(dbContext);

      // ACT
      var result = clubRepository.GetByIdAsync(id);

      // ASSERT
      result.Should().NotBeNull();
      result.Should().BeOfType<Task<Club>>();
    }

    [Fact]
    public async void ClubRepository_Delete_ReturnsBool()
    {
      // ARRANGE
      var club = new Club()
      {
        Title = "Running Club 1",
        Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
        Description = "This is the description of the first cinema",
        ClubCategory = ClubCategory.City,
        Address = new Address()
        {
          StreetName = "123 Main St",
          City = "Charlotte",
          State = "NC"
        }
      };
      var dbContext = await GetDbContext();
      var clubRepository = new ClubRepository(dbContext);
      clubRepository.Add(club);

      // ACT
      var result = clubRepository.Delete(club);

      // ASSERT
      result.Should().BeTrue();
    }

    [Fact]
    public async void ClubRepository_GetAllAsync_ReturnsIENumerableOfClub()
    {
      // ARRANGE
      var dbContext = await GetDbContext();
      var clubRepository = new ClubRepository(dbContext);

      // ACT
      var result = clubRepository.GetAllAsync();

      // ASSERT
      result.Should().NotBeNull();
      result.Should().BeOfType<Task<IEnumerable<Club>>>();
    }

    [Fact]
    public async void ClubRepository_GetClubByCityAsync_ReturnsIENumerableOfClub()
    {
      // ARRANGE
      var city = "Charlotte";
      var dbContext = await GetDbContext();
      var clubRepository = new ClubRepository(dbContext);

      // ACT
      var result = clubRepository.GetClubByCityAsync(city);

      // ASSERT
      result.Should().NotBeNull();
      result.Should().BeOfType<Task<IEnumerable<Club>>>();
    }

    [Fact]
    public async void ClubRepository_Update_ReturnsBool()
    {
      // Arrange
      var club = new Club()
      {
        Title = "Running Club 1",
        Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
        Description = "This is the description of the first cinema",
        ClubCategory = ClubCategory.City,
        Address = new Address()
        {
          StreetName = "123 Main St",
          City = "Charlotte",
          State = "NC"
        }
      };
      var dbContext = await GetDbContext();
      var clubRepository = new ClubRepository(dbContext);

      // Act
      var result = clubRepository.Update(club);

      // Assert
      result.Should().BeTrue();
    }
  }
}
