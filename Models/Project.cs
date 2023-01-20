using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonFlyBugTrackerNet6.Models
{
  public class Project
  {
    public int Id { get; set; }

    public int? CompanyId { get; set; }
    public int? ProjectPriorityId { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
    [Display(Name="Project Name")]
    public string? Name { get; set; }

    [Display(Name="Description")]
    public string? Description { get; set; }

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTimeOffset StartDate { get; set; }

    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? ImageFormFile { get; set; }

    public byte[]? ImageData { get; set; }

    [Display(Name="File Name")]
    public string? ImageFileName { get; set; }

    [Display(Name="File Extension")]
    public string? ImageFileContentType { get; set; }

    [Display(Name ="Archived")]
    public bool Archived { get; set; }

    public virtual Company? Company { get; set; }
    public virtual ProjectPriority? ProjectPriority { get; set; }
    public virtual ICollection<AppUser> Members { get; set; } = new HashSet<AppUser>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();



  }
}
