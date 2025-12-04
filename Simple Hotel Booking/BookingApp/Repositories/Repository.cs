using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    // Generic & Collections: implements a generic repository using List<T> and Dictionary caches
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _items = new();
        private readonly Func<T, int> _idSelector;

        public Repository(Func<T, int> idSelector)
        {
            _idSelector = idSelector ?? throw new ArgumentNullException(nameof(idSelector));
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Remove(T item)
        {
            _items.Remove(item);
        }

        public IEnumerable<T> GetAll() => _items.AsReadOnly();

        public T FindById(int id)
        {
            // using LINQ + lambda
            return _items.FirstOrDefault(x => _idSelector(x) == id);
        }

        public void Clear() => _items.Clear();
    }
}
