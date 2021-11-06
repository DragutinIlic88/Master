using System;
namespace OnlineBankingActorSystem.Messagess.NotificationMessages
{
	public record DeleteNotification(ulong RequestId, Guid MessageId, string Title, bool IsRead, string Content, string Date, string Time)
	{
		public override string ToString()
		{
			return $"{nameof(DeleteNotification)} message: request id: {RequestId}, message id: {MessageId}, title: {Title}, is read: {IsRead}, content: {Content}, date: {Date}, time: {Time}";
		}
	}
}
