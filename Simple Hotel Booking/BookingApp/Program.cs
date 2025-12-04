using System;
using System.Linq;
using BookingApp.Models;
using BookingApp.Repositories;
using BookingApp.Storage;
using BookingApp.Events;
using BookingApp.Exceptions;
using BookingApp.Helpers;
using System.Collections.Generic;

class Program
{
    // Repositories (generic)
    static Repository<User> Users = new(u => u.Id);
    static Repository<Property> Properties = new(p => p.Id);
    static Repository<Room> Rooms = new(r => r.Id);
    static Repository<Booking> Bookings = new(b => b.Id);

    static IStorage Storage = new JsonStorage();

    static void Main()
    {
        // Subscribe to booking created event with two handlers (delegate usage)
        BookingNotifier.BookingCreated += BookingNotifier_BookingCreated;
        BookingNotifier.BookingCreated += (s, e) =>
        {
            // Lambda expression + event subscriber — logs a short message
            Console.WriteLine($"[Lambda Handler] Booking #{e.Booking.Id} for Room {e.Booking.RoomId} created.");
        };

        LoadAll();

        SeedSampleDataIfEmpty();

        MainMenu();
    }

    static void BookingNotifier_BookingCreated(object? sender, BookingCreatedEventArgs e)
    {
        Console.WriteLine($"[Event] Booking Created: {e.Booking.GetInfo()}");
        // Mark room as unavailable (side-effect triggered by event)
        var room = Rooms.FindById(e.Booking.RoomId);
        if (room != null) room.IsAvailable = false;
    }

    static void MainMenu()
    {
        while (true)
        {
            Console.WriteLine("\n=== BookingApp Menu ===");
            Console.WriteLine("1. List Properties");
            Console.WriteLine("2. List Available Rooms");
            Console.WriteLine("3. Make Booking");
            Console.WriteLine("4. List Bookings");
            Console.WriteLine("5. Save Data");
            Console.WriteLine("6. Exit");
            Console.Write("Choose: ");
            var c = Console.ReadLine();
            switch (c)
            {
                case "1": ListProperties(); break;
                case "2": ListAvailableRooms(); break;
                case "3": MakeBookingFlow(); break;
                case "4": ListBookings(); break;
                case "5": SaveAll(); break;
                case "6": SaveAll(); return;
                default: Console.WriteLine("Invalid"); break;
            }
        }
    }

    static void ListProperties()
    {
        foreach (var p in Properties.GetAll())
        {
            Console.WriteLine(p);
            foreach (var r in p.Rooms) Console.WriteLine("  - " + r);
        }
    }

    static void ListAvailableRooms()
    {
        var available = Rooms.GetAll().Where(r => r.IsAvailable).ToList();
        if (!available.Any()) { Console.WriteLine("No rooms available."); return; }
        foreach (var r in available) Console.WriteLine(r);
    }

    static void MakeBookingFlow()
    {
        try
        {
            Console.Write("Enter user full name: ");
            var name = Console.ReadLine() ?? "";
            Console.Write("Phone (max 11 digits): ");
            var phone = Console.ReadLine() ?? "";
            if (!Utils.IsPhoneValid(phone)) throw new BookingException("Phone invalid. Must be numeric up to 11 digits.");

            Console.Write("Email: ");
            var email = Console.ReadLine() ?? "";

            var user = new User(name, phone, email);
            Users.Add(user);

            Console.WriteLine("Available Rooms:");
            var avail = Rooms.GetAll().Where(r => r.IsAvailable).ToList();
            if (!avail.Any()) throw new BookingException("No available rooms.");

            foreach (var r in avail) Console.WriteLine(r);
            Console.Write("Enter room id to book: ");
            if (!int.TryParse(Console.ReadLine(), out var roomId)) throw new BookingException("Invalid room id.");

            var room = Rooms.FindById(roomId);
            if (room == null) throw new BookingException("Room not found.");
            if (!room.IsAvailable) throw new BookingException("Room is not available.");

            Console.Write("Check-in (yyyy-mm-dd): ");
            var inStr = Console.ReadLine() ?? "";
            Console.Write("Check-out (yyyy-mm-dd): ");
            var outStr = Console.ReadLine() ?? "";
            if (!DateTime.TryParse(inStr, out var checkIn) || !DateTime.TryParse(outStr, out var checkOut))
                throw new BookingException("Invalid dates.");

            Console.Write("Number of guests: ");
            if (!int.TryParse(Console.ReadLine(), out var guests)) guests = 1;

            var total = Utils.CalculateTotal(room, checkIn, checkOut);

            // Check for conflicting bookings for same room (collections + lambda)
            var conflict = Bookings.GetAll()
                .Cast<Booking>()
                .Any(b => b.RoomId == roomId && b.CheckIn < checkOut && checkIn < b.CheckOut);

            if (conflict) throw new BookingException("Room already booked for the selected dates.");

            var booking = new Booking(user.Id, FindPropertyIdForRoom(roomId), roomId, checkIn, checkOut, guests, total);
            Bookings.Add(booking);

            // Trigger event via BookingNotifier (delegate invoked)
            BookingNotifier.OnBookingCreated(booking);

            Console.WriteLine($"Booking successful! Total: {total:C}");
        }
        catch (BookingException bex)
        {
            Console.WriteLine($"[Booking Error] {bex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Unexpected Error] {ex.Message}");
        }
        finally
        {
            // always save quick snapshot
            SaveAll();
        }
    }

    static int FindPropertyIdForRoom(int roomId)
    {
        foreach (var p in Properties.GetAll())
            if (p.Rooms.Any(r => r.Id == roomId)) return p.Id;
        return -1;
    }

    static void ListBookings()
    {
        var list = Bookings.GetAll().ToList();
        if (!list.Any()) { Console.WriteLine("No bookings."); return; }
        foreach (var b in list) Console.WriteLine(b);
    }

    static void LoadAll()
    {
        try
        {
            var users = Storage.Load<User>("users.json");
            foreach (var u in users) Users.Add(u);

            var properties = Storage.Load<Property>("properties.json");
            foreach (var p in properties)
            {
                Properties.Add(p);
                // add rooms to Rooms repository as well
                foreach (var r in p.Rooms) Rooms.Add(r);
            }

            var bookings = Storage.Load<Booking>("bookings.json");
            foreach (var b in bookings) Bookings.Add(b);

            Console.WriteLine("[Data] Loaded data from disk.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Load failed: {ex.Message}");
        }
    }

    static void SaveAll()
    {
        try
        {
            Storage.Save("users.json", Users.GetAll());
            Storage.Save("properties.json", Properties.GetAll());
            Storage.Save("bookings.json", Bookings.GetAll());
            Console.WriteLine("[Data] Saved data to disk.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Save failed: {ex.Message}");
        }
    }

    static void SeedSampleDataIfEmpty()
    {
        if (!Properties.GetAll().Any())
        {
            var p = new Property("Seabreeze Hotel", "123 Ocean Ave");
            var r1 = new Room("101", RoomType.Single, 120m);
            var r2 = new Room("102", RoomType.Double, 200m);
            var r3 = new Room("201 - Suite", RoomType.Suite, 400m);
            p.AddRoom(r1); p.AddRoom(r2); p.AddRoom(r3);

            Properties.Add(p);
            Rooms.Add(r1); Rooms.Add(r2); Rooms.Add(r3);

            var p2 = new Property("Mountain Inn", "77 Ridge Rd");
            var r4 = new Room("A1", RoomType.Single, 90m);
            p2.AddRoom(r4);
            Properties.Add(p2);
            Rooms.Add(r4);
        }
    }
}

