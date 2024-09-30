using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface INotificationRepository: IGenericRepository<Notification>
    {
        
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync();  
        Task MarkAsReadAsync(int notificationId);
    }
}
