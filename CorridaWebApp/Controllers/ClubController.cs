using CorridaWebApp.Interfaces;
using CorridaWebApp.Models;
using CorridaWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CorridaWebApp.Controllers
{
  public class ClubController : Controller
  {
    private readonly IClubRepository _clubRepository;
    private readonly IPhotoService _photoService;
    private readonly IHttpContextAccessor _contextAccessor;

    public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor contextAccessor)
    {
      _clubRepository = clubRepository;
      _photoService = photoService;
      _contextAccessor = contextAccessor;
    }

    public async Task<IActionResult> Index()
    {
      IEnumerable<Club> clubs = await _clubRepository.GetAllAsync();
      return View(clubs);
    }

    public async Task<IActionResult> Detail(int id)
    {
      Club club = await _clubRepository.GetByIdAsync(id);
      return View(club);
    }

    public IActionResult Create()
    {
      var currentUserId = _contextAccessor.HttpContext?.User.GetUserId();
      var createClubViewModel = new CreateClubViewModel { AppUserId = currentUserId };
      return View(createClubViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClubViewModel clubViewModel)
    {
      if (ModelState.IsValid)
      {
        var result = await _photoService.AddPhotoAsync(clubViewModel.Image);

        var club = new Club
        {
          AppUserId = clubViewModel.AppUserId,
          Title = clubViewModel.Title,
          Description = clubViewModel.Description,
          Image = result.Url.ToString(),
          Address = new Address
          {
            City = clubViewModel.Address.City,
            State = clubViewModel.Address.State,
            StreetName = clubViewModel.Address.StreetName,
          }
        };

        _clubRepository.Add(club);
        return RedirectToAction("Index");
      }
      else
      {
        ModelState.AddModelError("", "Photo upload failed.");
      }

      return View(clubViewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var club = await _clubRepository.GetByIdAsync(id);
      if (club == null) return View("Error");

      var clubViewModel = new EditClubViewModel
      {
        Title = club.Title,
        Description = club.Description,
        AddressId = club.AddressId,
        Address = club.Address,
        URL = club.Image,
        ClubCategory = club.ClubCategory,
      };

      return View(clubViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
    {
      if (!ModelState.IsValid)
      {
        ModelState.AddModelError("", "Failed to edit club.");
        return View("Edit", clubVM);
      }

      var userClub = await _clubRepository.GetByIdAsync(id);
      if (userClub != null)
      {
        try
        {
          var fi = new FileInfo(userClub.Image);
          var publicId = Path.GetFileNameWithoutExtension(fi.Name);
          await _photoService.DeletePhotoAsync(publicId);
        }
        catch (Exception)
        {
          ModelState.AddModelError("", "Could not delete photo");
          return View(clubVM);
        }
        var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

        userClub.Title = clubVM.Title;
        userClub.Description = clubVM.Description;
        userClub.ClubCategory = clubVM.ClubCategory;
        userClub.Image = photoResult.Url.ToString();
        userClub.AddressId = clubVM.AddressId;
        userClub.Address = clubVM.Address;

        _clubRepository.Update(userClub);

        return RedirectToAction("Index");
      }
      else
      {
        return View(clubVM);
      }
    }

    public async Task<IActionResult> Delete(int id)
    {
      var clubDetail = await _clubRepository.GetByIdAsync(id);
      if (clubDetail == null) return View("Error");

      return View(clubDetail);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteClub(int id)
    {
      var clubDetail = await _clubRepository.GetByIdAsync(id);
      if (clubDetail == null) return View("Error");

      _clubRepository.Delete(clubDetail);
      return RedirectToAction("Index");
    }
  }
}
