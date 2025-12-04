using System.Collections.Generic;

namespace BookingApp.Repositories
{
    // Abstraction: generic repository interface
    public interface IRepository<T>
    {
        void Add(T item);
        void Remove(T item);
        IEnumerable<T> GetAll();
        T FindById(int id);
        void Clear();
    }
}
