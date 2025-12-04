using System;

namespace BookingApp.Models
{
    // Abstraction: base abstract class
    public abstract class Entity
    {
        public int Id { get; protected set; } // encapsulated setter
        private static int _idCounter = 1;

        protected Entity()
        {
            Id = _idCounter++;
        }

        public abstract string GetInfo(); // polymorphic method to be overridden
    }
}
