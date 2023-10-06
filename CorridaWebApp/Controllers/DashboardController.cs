using CloudinaryDotNet.Actions;
using CorridaWebApp.Data;
using CorridaWebApp.Interfaces;
using CorridaWebApp.Models;
using CorridaWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CorridaWebApp.Controllers
{
  public class DashboardController : Controller
  {
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IHttpContextAccessor _httpContextAcessor;
    private readonly IPhotoService _photoService;

    public DashboardController(IDashboardRepository dashboardRepository,
      IHttpContextAccessor httpContextAcessor, IPhotoService photoService)
    {
      _dashboardRepository = dashboardRepository;
      _httpContextAcessor = httpContextAcessor;
      _photoService = photoService;
    }

    public async Task<IActionResult> Index()
    {
      var userRaces = await _dashboardRepository.GetAllUserRaces();
      var userClubs = await _dashboardRepository.GetAllUserClubs();

      var dashboardViewModel = new DashboardViewModel
      {
        Races = userRaces,
        Clubs = userClubs,
      };

      return View(dashboardViewModel);
    }

    public async Task<IActionResult> EditUserProfile()
    {
      var currentUserId = _httpContextAcessor.HttpContext?.User.GetUserId();
      var user = await _dashboardRepository.GetUserById(currentUserId);
      if (user == null) return View("Error");

      var editUserViewModel = new EditUserDashboardViewModel()
      {
        Id = currentUserId,
        City = user.City,
        Mileage = user.Mileage,
        Pace = user.Pace,
        State = user.State,
        ProfileImageUrl = user.ProfileImageUrl
      };

      return View(editUserViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editVM)
    {
      if (!ModelState.IsValid)
      {
        ModelState.AddModelError("", "Failed to edit profile");
        return View("EditUserProfile", editVM);
      }

      var user = await _dashboardRepository.GetUserByIdNoTracking(editVM.Id);
      if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
      {
        var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
        MapUserEdit(user, editVM, photoResult);
        _dashboardRepository.Update(user);
        return RedirectToAction("Index");
      }
      else
      {
        try
        {
          await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
        }
        catch (Exception ex)
        {
          ModelState.AddModelError("", "Could not delete photo");
          return View(editVM);
        }

        var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
        MapUserEdit(user, editVM, photoResult);
        _dashboardRepository.Update(user);
        return RedirectToAction("Index");
      }
    }

    private void MapUserEdit(AppUser user, EditUserDashboardViewModel editVM, ImageUploadResult photoResult)
    {
      user.Id = editVM.Id;
      user.Pace = editVM.Pace;
      user.Mileage = editVM.Mileage;
      user.ProfileImageUrl = photoResult.Url.ToString();
      user.City = editVM.City;
      user.State = editVM.State;
    }
  }
}
