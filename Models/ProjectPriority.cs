using System.ComponentModel.DataAnnotations;

namespace DragonFlyBugTrackerNet6.Models
{
  public class ProjectPriority
  {
    public int Id { get; set; }

    [Display(Name ="Priority Name")]
    public string? Name { get; set; }
  }
}
