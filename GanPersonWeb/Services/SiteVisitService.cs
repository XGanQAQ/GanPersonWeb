using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GanPersonWeb.Services
{
    public class SiteVisitService
    {
        private readonly DatabaseService _databaseService;

        public SiteVisitService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task RecordVisitAsync()
        {
            var today = DateTime.UtcNow.Date;
            var db = _databaseService.GetDbContext();
            var visit = await db.SiteVisits.FirstOrDefaultAsync(v => v.VisitDate == today);
            if (visit == null)
            {
                visit = new SiteVisit { VisitDate = today, Count = 1 };
                db.SiteVisits.Add(visit);
            }
            else
            {
                visit.Count += 1;
            }
            await db.SaveChangesAsync();
        }

        // 此方法可以利用传入的参数来存储更多访问信息
        public async Task RecordVisitAsync(SiteVisit siteVisit)
        {
            var today = DateTime.UtcNow.Date;
            var db = _databaseService.GetDbContext();
            var visit = await db.SiteVisits.FirstOrDefaultAsync(v => v.VisitDate == today);
            if (visit == null)
            {
                visit = new SiteVisit { VisitDate = today, Count = 1 };
                db.SiteVisits.Add(visit);
            }
            else
            {
                visit.Count += 1;
            }
            await db.SaveChangesAsync();
        }

        public async Task<List<SiteVisit>> GetVisitsAsync()
        {
            return await _databaseService.GetAllAsync<SiteVisit>();
        }
    }

}
