using System.ComponentModel.DataAnnotations;


namespace DragonFlyBugTrackerNet6.Models
{
  public class TicketStatus
  {
    public int Id { get; set; }

    [Display(Name = "Status Name")]
    public string? Name { get; set; }
  }
}
