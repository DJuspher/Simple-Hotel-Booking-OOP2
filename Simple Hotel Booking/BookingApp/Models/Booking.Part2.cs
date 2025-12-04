using System;

namespace BookingApp.Models
{
    public partial class Booking
    {
        // Method to check overlap with another booking (polymorphic utility)
        public bool ConflictsWith(Booking other)
        {
            return RoomId == other.RoomId &&
                   CheckIn < other.CheckOut &&
                   other.CheckIn < CheckOut;
        }

        public override string ToString() => GetInfo();
    }
}
