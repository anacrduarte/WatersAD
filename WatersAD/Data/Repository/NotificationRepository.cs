using Microsoft.EntityFrameworkCore;
using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly DataContext _context;

        public NotificationRepository(DataContext context):base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync()
        {
            return await _context.Notifications.ToListAsync();  
        }

        public async Task<IEnumerable<RequestWaterMeter>> GetRequestWaterMeterAsync()
        {
            return await _context.RequestWaterMeters.Include(n=> n.Notifications).Where(rwm => !rwm.Resolved).ToListAsync();
        }
        public async Task<Notification> GetNotificationAndRequestByIdAsync(int id)
        {
            return await _context.Notifications.Include(n=> n.RequestWaterMeter).FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<RequestWaterMeter> GetRequestWaterMeterByIdAsync(int id)
        {
            return await _context.RequestWaterMeters.FirstOrDefaultAsync(r=>r.Id == id);
        }
        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);  
            if (notification != null)
            {
                
                notification.IsRead = true; 
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpadateRequestAsync(int requestId, int waterMeterId)
        {
            var request = await _context.RequestWaterMeters.FindAsync(requestId);

            var waterMeter = await _context.WaterMeters.FindAsync(waterMeterId);
            if (request != null)
            {
                request.WaterMeter = waterMeter;
                request.Resolved = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpadateDateRequestAsync(RequestWaterMeter requestWM)
        {
            var request = await _context.RequestWaterMeters.FindAsync(requestWM.Id);
            if (request != null)
            {
               _context.RequestWaterMeters.Update(request);
                _context.SaveChanges();
            }
        }
    }
}
