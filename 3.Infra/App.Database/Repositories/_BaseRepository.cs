using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using App.Domain;
using App.Core;

namespace App.Database
{
    public class BaseRepository<T, Key> : IBaseRepository<T, Key>
        where T : class, IBaseEntity<Key>
    {
        private DataContext _context;
        private readonly IMapper _mapper;

        private DbSet<T>? _entities;
        public DbSet<T> Entities
        {
            get
            {
                return _entities ?? (_entities = _context.Set<T>());
            }
        }

        public DbContext Context
        {
            get 
            { 
                return _context; 
            }
        }

        public BaseRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> SaveToDB()
        {
            return  (await _context.SaveChangesAsync() >= 0);
        }

        public async Task<T?> GetAsync(Key id)
        {
            var result = await Entities.Where(c => c.id != null && c.id.Equals(id)).FirstOrDefaultAsync();

            return result;
        }

        public async Task<T> InsertAsync(T entity)
        {
            var result = Entities.Add(entity);

            if (!await SaveToDB())
                throw new Exception($"Unable to add new item to database.");

#pragma warning disable CS8603 // Possible null reference return.
            return await GetAsync(result.Entity.id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<T?> UpdateAsync(Key id, object model)
        {           
            var existingItem = await GetAsync(id);

            if (existingItem == null)
                throw new Exception($"No item in database.");

            _mapper.Map(model, existingItem);

            if (!await SaveToDB())
                throw new Exception($"Unable to save item to database.");

            return existingItem;
        }
        public async Task DeleteAsync(Key id)
        {
            var existingItem = await GetAsync(id);

            if (existingItem == null)
                throw new Exception($"No item in database.");
            else
            {
                Entities.Remove(existingItem);
                if (!await SaveToDB())
                    throw new Exception($"Unable to delete item from database.");
            }
        }

        public async Task InsertWithoutCommitAsync(T entity)
        {
            await Entities.AddAsync(entity);
        }

        public async Task UpdateWithoutCommitAsync(Key id, object model)
        {
            var existingItem = await GetAsync(id);

            if (existingItem == null)
                throw new Exception($"No item in database.");

            _mapper.Map(model, existingItem);
        }

        public async Task DeleteWithoutCommitAsync(Key id)
        {
            var existingItem = await GetAsync(id);

            if (existingItem == null)
                throw new Exception($"No item in database.");

            Entities.Remove(existingItem);
        }
    }
}


