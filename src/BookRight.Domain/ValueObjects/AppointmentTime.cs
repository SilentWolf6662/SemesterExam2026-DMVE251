using BookRight.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Domain.ValueObjects
{
    public record AppointmentTime
    {
        public DateTime Start { get; init; }
        public DateTime End { get; init; }

        public AppointmentTime(DateTime start, DateTime end)
        {
            // Hvis slut tiden er før start tiden, kastes en DomainException
            if (end <= start) throw new DomainException("Slut tiden kan ikke være før start tiden");

            Start = start;
            End = end;
        }

        // Beregner varigheden af tidsintervallet
        public TimeSpan Varighed => End - Start;

        // Hvis Start er mindre end other.End og End er større end other.Start, så overlapper de
        public bool Overlapping(AppointmentTime other) => Start < other.End && End > other.Start;
    }
}
