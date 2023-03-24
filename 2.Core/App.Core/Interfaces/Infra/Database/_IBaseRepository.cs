using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using App.Domain;

namespace App.Core
{
    public interface IBaseRepository<T, Key>
        where T : class, IBaseEntity<Key>
    {
        DbSet<T> Entities { get; }
        public Task<bool> SaveToDB();
        Task<T?> GetAsync(Key id);
        Task<T> InsertAsync(T entity);
        Task<T?> UpdateAsync(Key id, object model);
        Task DeleteAsync(Key id);

        Task InsertWithoutCommitAsync(T entity);
        Task UpdateWithoutCommitAsync(Key id, object model);
        Task DeleteWithoutCommitAsync(Key id);
    }
}
