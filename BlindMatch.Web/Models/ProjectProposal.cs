using System.ComponentModel.DataAnnotations;

namespace BlindMatch.Web.Models
{
    public class ProjectProposal
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Abstract { get; set; } = string.Empty;

        [Required]
        public string TechnicalStack { get; set; } = string.Empty;

        public string? SupervisorId { get; set; }
        public string? StudentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? MatchedAt { get; set; }
        public string Status { get; set; } = "Pending";
    }
}