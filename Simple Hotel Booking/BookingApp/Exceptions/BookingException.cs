using System;

namespace BookingApp.Exceptions
{
    // Custom exception
    public class BookingException : Exception
    {
        public BookingException(string message) : base(message) { }
    }
}
