using System;
namespace Contracts.Models
{
	public class NotificationModel
	{
		public Guid MessageId { get; set; }
		public string Content { get; set; }
		public bool IsRead { get; set; }
		public string Date { get; set; }
		public string Time { get; set; }
		public string Title { get; set; }
		public string Type { get; set; }
	}
}
