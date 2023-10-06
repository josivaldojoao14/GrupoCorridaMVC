using CorridaWebApp.Controllers;
using CorridaWebApp.Interfaces;
using CorridaWebApp.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CorridaWebApp.Tests.Controller
{
  public class ClubControllerTests
  {
    private ClubController _clubController;
    private readonly IClubRepository _clubRepository;
    private readonly IPhotoService _photoService;
    private readonly IHttpContextAccessor _httpContextAcessor;

    public ClubControllerTests()
    {
      _clubRepository = A.Fake<IClubRepository>();
      _photoService = A.Fake<IPhotoService>();
      _httpContextAcessor = A.Fake<IHttpContextAccessor>();

      // sut
      _clubController = new ClubController(_clubRepository, _photoService, _httpContextAcessor);
    }

    [Fact]
    public void ClubController_Index_ReturnsSuccess()
    {
      // Arrange
      var clubs = A.Fake<IEnumerable<Club>>();
      A.CallTo(() => _clubRepository.GetAllAsync()).Returns(clubs);

      // Act
      var result = _clubController.Index();

      // Assert
      result.Should().BeOfType<Task<IActionResult>>();
    }

    [Fact]
    public void ClubController_Detail_ReturnsSuccess()
    {
      // Arrange
      var id = 1;
      var club = A.Fake<Club>();
      A.CallTo(() => _clubRepository.GetByIdAsync(id)).Returns(club);

      // Act
      var result = _clubController.Detail(id);

      // Assert
      result.Should().BeOfType<Task<IActionResult>>();
    }
  }
}
