using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceTracker.Infrastructure.Data.Models
{
    [Comment("Tasks Db model")]
    public class ToDoItem
    {
        [Key]
        [Comment("Task identifier")]
		public int TaskId { get; set; }

        [Required]
        [Comment("User identifier")]
		public int UserId { get; set; }

        [Required]
        [ForeignKey(nameof(UserId))]
        [Comment("User navigation property.")]
        public required virtual User User { get; set; }

        [Required]
        [StringLength(DataConstants.taskTitleMaxLength)]
        [Comment("Title of the current task")]
		public required string Title { get; set; }

        [StringLength(DataConstants.taskDescriptionMaxLength)]
        [Comment("Description of the current task")]
		public string? Description { get; set; }

        [Comment("Whitin the day that current task must be finished")]
		public DateTime? DueDate { get; set; }

        [Required]
        [Comment("Task priority level")]
        public TaskPriority Priority { get; set; } = TaskPriority.Low;

        [Required]
        [Comment("Task status")]
        public TaskStatus TaskStatus { get; set; } = TaskStatus.Pending;

        [Required]
        [Comment("The date that task is created")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        [Comment("One ToDo task can have many notifications")]
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
	}
}
