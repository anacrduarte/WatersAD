using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        /// <summary>
        /// Asynchronously retrieves all unread notifications.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Notification"/> objects that are unread.</returns>
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync();

        /// <summary>
        /// Asynchronously marks a specific notification as read, based on the provided notification ID.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to be marked as read.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task MarkAsReadAsync(int notificationId);

        /// <summary>
        /// Asynchronously updates a request with the associated water meter.
        /// </summary>
        /// <param name="requestId">The unique identifier of the request.</param>
        /// <param name="waterMeterId">The unique identifier of the water meter to associate with the request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpadateRequestAsync(int requestId, int waterMeterId);

        /// <summary>
        /// Asynchronously retrieves all water meter requests.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="RequestWaterMeter"/> objects.</returns>
        Task<IEnumerable<RequestWaterMeter>> GetRequestWaterMeterAsync();

        /// <summary>
        /// Asynchronously retrieves a <see cref="RequestWaterMeter"/> object by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the water meter request.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="RequestWaterMeter"/> object if found; otherwise, null.</returns>
        Task<RequestWaterMeter> GetRequestWaterMeterByIdAsync(int id);

        /// <summary>
        /// Asynchronously retrieves a <see cref="Notification"/> object along with its associated water meter request by the notification's unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the notification.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Notification"/> object along with its associated request if found; otherwise, null.</returns>
        Task<Notification> GetNotificationAndRequestByIdAsync(int id);

        /// <summary>
        /// Asynchronously updates the details of a given <see cref="RequestWaterMeter"/> object, particularly its date or other relevant properties.
        /// </summary>
        /// <param name="requestWM">The <see cref="RequestWaterMeter"/> object containing updated information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpadateDateRequestAsync(RequestWaterMeter requestWM);
    }
}
