using System;
using BookingApp.Models;

namespace BookingApp.Events
{
    // Delegates & Events: define delegate and event args
    public delegate void BookingCreatedHandler(object sender, BookingCreatedEventArgs args);

    public class BookingCreatedEventArgs : EventArgs
    {
        public Booking Booking { get; }
        public BookingCreatedEventArgs(Booking b) => Booking = b;
    }

    public static class BookingNotifier
    {
        public static event BookingCreatedHandler? BookingCreated;

        // Trigger method
        public static void OnBookingCreated(Booking booking)
        {
            BookingCreated?.Invoke(null, new BookingCreatedEventArgs(booking));
        }
    }
}
