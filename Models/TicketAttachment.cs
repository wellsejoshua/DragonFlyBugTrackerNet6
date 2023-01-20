using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonFlyBugTrackerNet6.Models
{
    public class TicketAttachment
    {
    public int Id { get; set; }

    [Display(Name ="Ticket")]
    public int TicketId { get; set; }

    [Display(Name ="Team Member")]
    public string? AppUserId { get; set; }

    [Display(Name ="File Date Created")]
    [DataType(DataType.Date)]
    public DateTimeOffset Created { get; set; }

    [Display(Name ="File Description")]
    public string? Description { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? FormFile { get; set; }

    [Display(Name = "File Name")]
    public string? FileName { get; set; }

    public byte[]? FileData { get; set; }

    [Display(Name ="File Extension")]
    public string? FileContentType { get; set; }

    public virtual Ticket? Ticket{ get; set; }
    public virtual AppUser? AppUser { get; set; }
  }
}
