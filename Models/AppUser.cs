using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DragonFlyBugTrackerNet6.Models
{
  public class AppUser : IdentityUser
  {
    [Required]
    [Display(Name ="First Name")]
    [StringLength(50, ErrorMessage ="The {0} must be at least {2} and at most {1} characters long.", MinimumLength =2)]
    public string? FirstName { get; set; }    
    
    [Required]
    [Display(Name ="Last Name")]
    [StringLength(50, ErrorMessage ="The {0} must be at least {2} and at most {1} characters long.", MinimumLength =2)]
    public string? LastName { get; set; }

    [NotMapped]
    public string? FullName { get { return $"{FirstName} {LastName}"; } }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? AvatarFormFile { get; set; }

    public byte[]? AvatarFileData { get; set; }

    [Display(Name = "Avatar")]
    public string? AvatarFileName { get; set; }

    [Display(Name = "File Extension")]
    public string? AvatarContentType { get; set; }

    public int? CompanyId { get; set; }


    public virtual Company? Company { get; set; }
    public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

  }
}
