using System;

namespace BookingApp.Models
{
    public enum RoomType { Single, Double, Suite }

    public class Room : Entity
    {
        public string Name { get; set; }
        public RoomType Type { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Room() { }

        public Room(string name, RoomType type, decimal price)
        {
            Name = name;
            Type = type;
            PricePerNight = price;
        }

        public override string GetInfo()
        {
            return $"Room: {Id} - {Name} [{Type}] - {PricePerNight:C} - {(IsAvailable ? "Available" : "Booked")}";
        }

        public override string ToString() => GetInfo();
    }
}
