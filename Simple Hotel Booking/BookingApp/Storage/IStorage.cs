using System.Collections.Generic;

namespace BookingApp.Storage
{
    // Abstraction for storage (File I/O)
    public interface IStorage
    {
        void Save<T>(string filename, IEnumerable<T> items);
        IEnumerable<T> Load<T>(string filename);
    }
}
