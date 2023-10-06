using CorridaWebApp.Interfaces;
using CorridaWebApp.Models;
using CorridaWebApp.Repository;
using CorridaWebApp.Services;
using CorridaWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CorridaWebApp.Controllers
{
  public class RaceController : Controller
  {
    private readonly IRaceRepository _raceRepository;
    private readonly IPhotoService _photoService;
    private readonly IHttpContextAccessor _contextAccessor;

    public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor contextAccessor)
    {
      _raceRepository = raceRepository;
      _photoService = photoService;
      _contextAccessor = contextAccessor;
    }

    public async Task<IActionResult> Index()
    {
      IEnumerable<Race> races = await _raceRepository.GetAllAsync();
      return View(races);
    }

    public async Task<IActionResult> Detail(int id)
    {
      Race race = await _raceRepository.GetByIdAsync(id);
      return View(race);
    }

    public IActionResult Create()
    {
      var currentUserId = _contextAccessor.HttpContext?.User.GetUserId();
      var createRaceViewModel = new CreateRaceViewModel { AppUserId = currentUserId };
      return View(createRaceViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRaceViewModel raceViewModel)
    {
      if (ModelState.IsValid)
      {
        var result = await _photoService.AddPhotoAsync(raceViewModel.Image);

        var race = new Race
        {
          AppUserId = raceViewModel.AppUserId,
          Title = raceViewModel.Title,
          Description = raceViewModel.Description,
          Image = result.Url.ToString(),
          Address = new Address
          {
            City = raceViewModel.Address.City,
            State = raceViewModel.Address.State,
            StreetName = raceViewModel.Address.StreetName,
          }
        };

        _raceRepository.Add(race);
        return RedirectToAction("Index");
      }
      else
      {
        ModelState.AddModelError("", "Photo upload failed.");
      }

      return View(raceViewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var race = await _raceRepository.GetByIdAsync(id);
      if (race == null) return View("Error");

      var raceViewModel = new EditRaceViewModel
      {
        Title = race.Title,
        Description = race.Description,
        AddressId = race.AddressId,
        Address = race.Address,
        URL = race.Image,
        RaceCategory = race.RaceCategory,
      };

      return View(raceViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
    {
      if (!ModelState.IsValid)
      {
        ModelState.AddModelError("", "Failed to edit race.");
        return View("Edit", raceVM);
      }

      var userRace = await _raceRepository.GetByIdAsync(id);
      if (userRace != null)
      {
        try
        {
          var fi = new FileInfo(userRace.Image);
          var publicId = Path.GetFileNameWithoutExtension(fi.Name);
          await _photoService.DeletePhotoAsync(publicId);
        }
        catch (Exception)
        {
          ModelState.AddModelError("", "Could not delete photo");
          return View(raceVM);
        }
        var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);

        userRace.Title = raceVM.Title;
        userRace.Description = raceVM.Description;
        userRace.RaceCategory = raceVM.RaceCategory;
        userRace.Image = photoResult.Url.ToString();
        userRace.AddressId = raceVM.AddressId;
        userRace.Address = raceVM.Address;

        _raceRepository.Update(userRace);

        return RedirectToAction("Index");
      }
      else
      {
        return View(raceVM);
      }
    }

    public async Task<IActionResult> Delete(int id)
    {
      var raceDetail = await _raceRepository.GetByIdAsync(id);
      if (raceDetail == null) return View("Error");

      return View(raceDetail);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteClub(int id)
    {
      var raceDetail = await _raceRepository.GetByIdAsync(id);
      if (raceDetail == null) return View("Error");

      _raceRepository.Delete(raceDetail);
      return RedirectToAction("Index");
    }
  }
}
