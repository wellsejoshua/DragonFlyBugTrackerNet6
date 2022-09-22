using System.ComponentModel.DataAnnotations;

namespace DragonFlyBugTrackerNet6.Models
{
  public class Invite
  {
    public int Id { get; set; }

    [Display(Name ="Date Sent")]
    public DateTime? InviteDate { get; set; }

    [Display(Name = "Join Date")]
    public DateTime? JoinDate { get; set; }

    [Display(Name ="Code Token")]
    public Guid CompanyToken { get; set; }

    [Display(Name ="Company")]
    public int CompanyId { get; set; }

    [Display(Name ="Project")]
    public int ProjectId { get; set; }

    [Display(Name = "Invitor")]
    public string? InvitorId { get; set; }

    [Display(Name = "Invitee")]
    public string? InviteeId { get; set; }

    [EmailAddress]
    [Display(Name = "Invitee Email")]
    public string? InviteeEmail { get; set; }

    [Display(Name = "Invitee First Name")]
    public string? InviteeFirstName { get; set; }

    [Display(Name = "Invitee Last Name")]
    public string? InviteeLasstName { get; set; }
    //Flag
    public bool IsValid { get; set; }


    public virtual Company? Company { get; set; }
    public virtual Project? Project { get; set; }
    public virtual AppUser? Invitor { get; set; }
    public virtual AppUser? Invitee { get; set; }

  }
}
