using GanPersonWeb.Shared.Models;
using GanPersonWeb.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions; // Add this for Expression<Func<...>>

namespace GanPersonWeb.Services
{
    public class DatabaseService
    {
        private readonly GanPersonDbContext _context;

        public DatabaseService(GanPersonDbContext context)
        {
            _context = context;
        }

        // Generic CRUD methods
        public async Task<List<T>> GetAllAsync<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync<T>(int id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetRangeAsync<T>(int skip, int take) where T : class
        {
            return await _context.Set<T>().Skip(skip).Take(take).ToListAsync();
        }

        // New method to get count
        public async Task<int> GetCountAsync<T>() where T : class
        {
            return await _context.Set<T>().CountAsync();
        }

        // 通用Distinct查询（修正：使用Expression<Func<...>>，保持IQueryable链式调用）
        public async Task<List<TProperty>> GetDistinctAsync<T, TProperty>(Expression<Func<T, TProperty>> selector) where T : class
        {
            return await _context.Set<T>().Select(selector).Distinct().ToListAsync();
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        // Add overload for AddAsync for Image entity (optional, generic already exists)
        public async Task AddImageAsync(Image image)
        {
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(int id) where T : class
        {
            var entity = await GetByIdAsync<T>(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public GanPersonDbContext GetDbContext()
        {
            return _context;
        }
    }
}
