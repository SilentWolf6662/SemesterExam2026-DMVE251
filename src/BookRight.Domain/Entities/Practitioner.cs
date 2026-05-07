using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Domain.Entities
{
    public class Practitioner : AggregateRoot
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public List<Clinic> Clinics { get; private set; }
        public List<Appointment> Appointments { get; private set; }


        public Practitioner(string firstName, string lastName, string phoneNumber, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            Clinics = new List<Clinic>();
            Appointments = new List<Appointment>();
        }
    }
}
