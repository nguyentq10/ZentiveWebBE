using DAL.DBcontext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Basic
{
    public class GenericRepository<T> where T : class
    {
        protected readonly ZenthicDBContext _context;

        // Chỉ giữ lại constructor này
        public GenericRepository(ZenthicDBContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(params object[] keyValues)
        {
            return await _context.Set<T>().FindAsync(keyValues);
        }

        // Các hàm này chỉ "chuẩn bị" thay đổi, không Save
        public void Create(T entity)
        {
            _context.Add(entity);
        }

        public void Update(T entity)
        {
            // Cách làm đơn giản và an toàn hơn
            _context.Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().CountAsync(predicate);
        }

        // You might also want a version without a condition (counts all)
        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }
        // Các hàm Save sẽ được gọi bởi UnitOfWork
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}