using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Models
{
    public class Property : Entity
    {
        public string Name { get; set; }
        public string Address { get; set; }

        // Collections in use: List<Room>
        public List<Room> Rooms { get; set; } = new();

        public Property() { }
        public Property(string name, string address)
        {
            Name = name;
            Address = address;
        }

        public void AddRoom(Room r) => Rooms.Add(r);

        public override string GetInfo()
        {
            return $"Property: {Id} - {Name} ({Address}) Rooms: {Rooms.Count}";
        }

        public override string ToString() => GetInfo();
    }
}
