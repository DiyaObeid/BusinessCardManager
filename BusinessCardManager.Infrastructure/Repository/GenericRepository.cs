// GenericRepository.cs implements a generic repository pattern for managing 
// data access operations for any entity type T. It provides standard methods 
// for adding, retrieving, and removing entities from the database using 
// Entity Framework Core. This repository enhances code reusability and 
// maintainability across different entity types in the application.

using BusinessCardManager.Core.DTOs;
using BusinessCardManager.Core.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardManager.Infrastructure.Repository
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        // Constructor to initialize the repository with the application context
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Ensure context is not null
            _dbSet = context.Set<T>(); // Set the DbSet for the entity type
        }

        // Adds a new entity to the database asynchronously
        public async Task<ResultDto> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity); // Add the entity
                await _context.SaveChangesAsync(); // Save changes to the database
                return new ResultDto { Succeeded = true, Message = "Entity added successfully." };
            }
            catch (Exception ex)
            {
                return new ResultDto { Succeeded = false, Message = $"Failed to add entity: {ex.Message}" }; // Improved error message
            }
        }

        // Asynchronously retrieves an entity of type T from the database based on its unique identifier.
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }



        // Retrieves all entities from the database asynchronously
        public async Task<IEnumerable<T?>> GetAllAsync()
        {
            return await _dbSet.ToListAsync(); // Return all entities as a list
        }

        // Retrieves entities based on a specified filter expression asynchronously
        public async Task<IEnumerable<T?>> GetByFiltersAsync(Expression<Func<T, bool>>? filterExpression = null)
        {
            IQueryable<T> query = _dbSet;

            // If a filter expression is provided, apply it
            if (filterExpression != null)
            {
                query = query.Where(filterExpression);
            }

            return await query.ToListAsync(); // Return filtered entities as a list
        }

        // Removes an entity from the database asynchronously
        public async Task<ResultDto> RemoveAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity); // Remove the entity
                await _context.SaveChangesAsync(); // Save changes to the database
                return new ResultDto()
                {
                    Succeeded = true,
                    Message = "Entity removed successfully." // Improved success message
                };
            }
            catch (Exception ex)
            {
                return new ResultDto()
                {
                    Succeeded = false,
                    Message = $"Failed to remove entity: {ex.Message}" // Improved error message
                };
            }
        }
    }
}
