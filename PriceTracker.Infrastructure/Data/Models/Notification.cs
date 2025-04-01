using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceTracker.Infrastructure.Data.Models
{
	[Comment("Notification Db model")]
    public class Notification
    {
		[Key]
		[Comment("Notification identifier")]
		public int NotificationId { get; set; }

		[Required]
		[Comment("User identifier")]
		public int UserId { get; set; }

		[Required]
		[ForeignKey(nameof(UserId))]
		[Comment("Navigation property for User")]
		public virtual User User { get; set; } = null!;

		[Required]
		[Comment("Task identifier")]
		public int TaskId { get; set; }

		[Required]
		[ForeignKey(nameof(TaskId))]
		[Comment("Navigation property for Task")]
		public ToDoItem Task { get; set; } = null!;

		[Required]
		[StringLength(DataConstants.notificationMessageMaxLength)]
		[Comment("Notification message")]
		public string Message { get; set; } = string.Empty;

		[Comment("Track if the message is readed or not")]
		public bool IsRead { get; set; } = false;

		[Required]
		[Comment("Time of the notification")]
		public DateTime NotificationTime { get; set; }

		[Required]
		[Comment("Time when the notification was created")]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
