// IRepository.cs defines a generic repository interface that outlines 
// standard data access operations for any entity type T. It provides 
// methods for adding, retrieving, and removing entities, promoting 
// code reusability and separation of concerns within the application.

using BusinessCardManager.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessCardManager.Core.IRepositories
{
    public interface IRepository<T> where T : class
    {
        // Asynchronously adds a new entity to the data store
        Task<ResultDto> AddAsync(T entity);

        // Asynchronously retrieves a business card by its unique identifier.
        Task<T?> GetByIdAsync(int id);

        // Asynchronously retrieves all entities from the data store
        Task<IEnumerable<T?>> GetAllAsync();

        // Asynchronously retrieves entities from the data store based on the specified filter expression
        Task<IEnumerable<T?>> GetByFiltersAsync(Expression<Func<T, bool>>? filterExpression = null);

        // Asynchronously removes an existing entity from the data store
        Task<ResultDto> RemoveAsync(T entity);
    }
}
