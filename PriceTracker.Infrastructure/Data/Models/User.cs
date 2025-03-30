using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceTracker.Infrastructure.Data.Models
{
	[Comment("User DB Model")]
    public class User
    {
		[Key]
		[Comment("User Identifier")]
		public int UserId { get; set; }

		//Must be Unique and should be configured later in the fluent API
		[Required]
		[StringLength(DataConstants.userNameMaxLength)]
		[Comment("UserName")]
		public string UserName { get; set; } = null!;

		//Must be Unique and should be configured later in the fluent API 
		[Required]
		[EmailAddress]
		[StringLength(DataConstants.emailAddressMaxLength)]
		[Comment("User emailAdress")]
		public string Email { get; set; } = null!;

		[Required]
		[StringLength(DataConstants.passwordHashMaxLength)]
		[Comment("PasswordHash of the User")]
		public string PasswordHash { get; set; } = null!;

		[Comment("The time that the user have been created")]
		[NotMapped]
		public DateTime? CreatedAt { get; set; }
	}
}
