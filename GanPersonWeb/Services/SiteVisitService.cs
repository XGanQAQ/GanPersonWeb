using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace GanPersonWeb.Services
{
    public interface ISiteVisitService
    {
        Task RecordVisitAsync();
        Task<List<SiteVisit>> GetVisitsAsync();
    }
    public class SiteVisitService : ISiteVisitService
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

        public async Task<List<SiteVisit>> GetVisitsAsync()
        {
            return await _databaseService.GetAllAsync<SiteVisit>();
        }
    }
    
    // 缓冲定时存储法
    public class SiteVisitService_Extension : ISiteVisitService
    {
        private readonly ISiteVisitService _siteVisitService;

        private readonly DatabaseService _databaseService;
        private static readonly ConcurrentDictionary<DateTime, int> _visitCache = new();
        private static readonly object _lock = new();
        private static bool _timerStarted = false;

        public SiteVisitService_Extension(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            StartDailySaveTimer();
        }

        public Task RecordVisitAsync()
        {
            var today = DateTime.UtcNow.Date;
            _visitCache.AddOrUpdate(today, 1, (_, old) => old + 1);
            return Task.CompletedTask;
        }

        // 此方法可以利用传入的参数来存储更多访问信息
        //public Task RecordVisitAsync(SiteVisit siteVisit)
        //{
        //    var today = DateTime.UtcNow.Date;
        //    _visitCache.AddOrUpdate(today, 1, (_, old) => old + 1);
        //    return Task.CompletedTask;
        //}

        private void StartDailySaveTimer()
        {
            if (_timerStarted) return;
            lock (_lock)
            {
                if (_timerStarted) return;
                var timer = new System.Timers.Timer(TimeSpan.FromDays(1).TotalMilliseconds)
                {
                    AutoReset = true,
                    Enabled = true
                };
                timer.Elapsed += async (s, e) => await SaveVisitsToDbAsync();
                timer.Start();
                _timerStarted = true;
            }
        }

        private async Task SaveVisitsToDbAsync()
        {
            foreach (var kvp in _visitCache)
            {
                var db = _databaseService.GetDbContext();
                var visit = await db.SiteVisits.FirstOrDefaultAsync(v => v.VisitDate == kvp.Key);
                if (visit == null)
                {
                    visit = new SiteVisit { VisitDate = kvp.Key, Count = kvp.Value };
                    db.SiteVisits.Add(visit);
                }
                else
                {
                    visit.Count = kvp.Value;
                }
                await db.SaveChangesAsync();
            }
            _visitCache.Clear();
        }

        public async Task<List<SiteVisit>> GetVisitsAsync()
        {
            return await _databaseService.GetAllAsync<SiteVisit>();
        }
    }
}
