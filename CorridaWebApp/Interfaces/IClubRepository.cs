using CorridaWebApp.Models;

namespace CorridaWebApp.Interfaces
{
  public interface IClubRepository
  {
    Task<IEnumerable<Club>> GetAllAsync();
    Task<Club> GetByIdAsync(int id);
    Task<IEnumerable<Club>> GetClubByCityAsync(string city);
    bool Add(Club club);
    bool Update(Club club);
    bool Delete(Club club);
    bool Save();
  }
}
