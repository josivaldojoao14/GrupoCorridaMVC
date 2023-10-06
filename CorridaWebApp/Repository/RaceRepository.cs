﻿using CorridaWebApp.Data;
using CorridaWebApp.Interfaces;
using CorridaWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CorridaWebApp.Repository
{
  public class RaceRepository : IRaceRepository
  {
    private readonly ApplicationDbContext _context;

    public RaceRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public bool Add(Race race)
    {
      _context.Races.Add(race);
      return Save();
    }

    public bool Delete(Race race)
    {
      _context.Races.Remove(race);
      return Save();
    }

    public async Task<IEnumerable<Race>> GetAllAsync()
    {
      return await _context.Races.ToListAsync();
    }

    public async Task<IEnumerable<Race>> GetAllRacesByCityAsync(string city)
    {
      return await _context.Races.Where(r => r.Address.City.Contains(city)).ToListAsync();
    }

    public async Task<Race> GetByIdAsync(int id)
    {
      return await _context.Races.Include(a => a.Address).FirstOrDefaultAsync(r => r.Id == id);
    }

    public bool Save()
    {
      var saved = _context.SaveChanges();
      return saved > 0 ? true : false;
    }

    public bool Update(Race race)
    {
      _context.Update(race);
      return Save();
    }
  }
}
