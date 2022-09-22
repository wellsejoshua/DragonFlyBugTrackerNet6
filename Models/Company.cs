using System.ComponentModel.DataAnnotations;

namespace DragonFlyBugTrackerNet6.Models
{
  public class Company
  {
    public int Id { get; set; }

    [Display(Name ="Company Name")]
    public string? Name { get; set; }

    [Display(Name ="Company Description")]
    public string? Description { get; set; }


    public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    public virtual ICollection<AppUser> Members { get; set; } = new HashSet<AppUser>();
  }
}
