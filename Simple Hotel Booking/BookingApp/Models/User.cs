using System;

namespace BookingApp.Models
{
    // Inherits Entity, encapsulation through properties
    public class User : Entity
    {
        public string FullName { get; set; }
        public string Phone { get; set; } // we will validate length in Utils
        public string Email { get; set; }

        public User() { }

        public User(string name, string phone, string email)
        {
            FullName = name;
            Phone = phone;
            Email = email;
        }

        public override string GetInfo()
        {
            return $"User: {Id} - {FullName} ({Phone}, {Email})";
        }

        public override string ToString() => GetInfo();
    }
}
