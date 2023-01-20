using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DragonFlyBugTrackerNet6.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }

        [Display(Name = "Updated Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset Updated { get; set; }

        [Display(Name = "Archived")]
        public bool Archived { get; set; }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }

        [Display(Name = "Ticket Status")]
        public int TicketStatusId { get; set; }

        [Display(Name = "Ticket Priority")]
        public int TicketPriorityId { get; set; }

        [Display(Name = "Ticket Owner")]
        public string? OwnerUserId { get; set; }

        [Display(Name = "Ticket Developer")]
        public string? DeveloperUserId { get; set; }

        //archive tickets 
        [DisplayName("Archived By Project")]
        public bool ArchivedByProject { get; set; }


        public virtual Project? Project { get; set; }
        public virtual TicketType? TicketType { get; set; }
        public virtual TicketPriority? TicketPriority { get; set; }
        public virtual TicketStatus? TicketStatus { get; set; }

        public virtual AppUser? OwnerUser { get; set; }
        public virtual AppUser? DeveloperUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

    }
}
