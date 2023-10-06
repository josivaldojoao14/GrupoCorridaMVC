using CorridaWebApp.Data;
using CorridaWebApp.Interfaces;
using CorridaWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CorridaWebApp.Repository
{
  public class ClubRepository : IClubRepository
  {
    private readonly ApplicationDbContext _context;

    public ClubRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public bool Add(Club club)
    {
      _context.Clubs.Add(club);
      return Save();
    }

    public bool Delete(Club club)
    {
      _context.Clubs.Remove(club);
      return Save();
    }

    public async Task<IEnumerable<Club>> GetAllAsync()
    {
      return await _context.Clubs.ToListAsync();
    }

    public async Task<Club> GetByIdAsync(int id)
    {
      return await _context.Clubs.Include(a => a.Address).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Club>> GetClubByCityAsync(string city)
    {
      return await _context.Clubs.Where(x => x.Address.City.Contains(city)).ToListAsync();
    }

    public bool Save()
    {
      var saved = _context.SaveChanges();
      return saved > 0 ? true : false;
    }

    public bool Update(Club club)
    {
      _context.Update(club);
      return Save();
    }
  }
}
