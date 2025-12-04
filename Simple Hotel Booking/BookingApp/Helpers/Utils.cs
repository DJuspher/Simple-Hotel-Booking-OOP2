using System;
using BookingApp.Models;

namespace BookingApp.Helpers
{
    public static class Utils
    {
        public static bool IsPhoneValid(string phone) => !string.IsNullOrEmpty(phone) && phone.Length <= 11 && long.TryParse(phone, out _);

        public static decimal CalculateTotal(Room room, DateTime checkIn, DateTime checkOut)
        {
            var nights = (checkOut.Date - checkIn.Date).Days;
            if (nights <= 0) nights = 1;
            return room.PricePerNight * nights;
        }
    }
}
