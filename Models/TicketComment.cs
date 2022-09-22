using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DragonFlyBugTrackerNet6.Models
{
  public class TicketComment
    {
    public int Id { get; set; }

    [Display(Name = "Ticket")]
    public int TicketId { get; set; }

    [Display(Name = "Team Member")]
    public string? AppUserId { get; set; }

    [Display(Name ="Member Comment")]
    public string? Comment { get; set; }

    [Display(Name ="Created Date")]
    [DataType(DataType.Date)]
    public DateTime? Created { get; set; }


    public virtual Ticket? Ticket { get; set; }
    public virtual AppUser? AppUser { get; set; }
  }
}
