using System.ComponentModel.DataAnnotations;

namespace CorridaWebApp.Models
{
  public class Address
  {
    [Key]
    public int Id { get; set; }
    public string StreetName { get; set; }
    public string City { get; set; }
    public string State { get; set; }
  }
}
