using System.ComponentModel.DataAnnotations;

namespace DragonFlyBugTrackerNet6.Models
{
  public class TicketHistory
  {
    public int Id { get; set; }

    [Display(Name ="Ticket")]
    public int TicketId { get; set; }

    [Display(Name = "Team Member")]
    public string? AppUserId { get; set; }

    [Display(Name ="Updated Item")]
    public string? Property { get; set; }    
    
    [Display(Name ="Previous")]
    public string? OldValue { get; set; }

    [Display(Name = "Current")]
    public string? NewValue { get; set; }

    [Display(Name ="Date Modified")]
    [DataType(DataType.Date)]
    public DateTime? Created { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    public virtual Ticket? Ticket { get; set; }
    public virtual AppUser? AppUser { get; set; }


  }
}
