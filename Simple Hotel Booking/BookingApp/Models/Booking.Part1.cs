using System;

namespace BookingApp.Models
{
    // Partial class split to demonstrate Partial Classes
    public partial class Booking : Entity
    {
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public int RoomId { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }

        public decimal TotalAmount { get; set; }

        // Event-based flow: Booking creation will trigger events (see events file)
        public Booking() { }

        public Booking(int userId, int propertyId, int roomId, DateTime checkIn, DateTime checkOut, int guests, decimal amount)
        {
            UserId = userId;
            PropertyId = propertyId;
            RoomId = roomId;
            CheckIn = checkIn;
            CheckOut = checkOut;
            Guests = guests;
            TotalAmount = amount;
        }

        public override string GetInfo()
        {
            return $"Booking: {Id} | User {UserId} | Property {PropertyId} | Room {RoomId} | {CheckIn:yyyy-MM-dd} to {CheckOut:yyyy-MM-dd} | Guests {Guests} | {TotalAmount:C}";
        }
    }
}
