using BookRight.Domain.Enums;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Domain.Entities
{
    public class Appointment : AggregateRoot
    {
        public AppointmentTime AppointmentTime { get; private set; }
        public Guid TreatmentTypeId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid PractitionerId { get; private set; }
        public string Note { get; private set; } = string.Empty;
        public TreatmentStatus Status { get; private set; }
        private Appointment() { } // EF Core
        // PRIVAT constructor — tvinger brug af factory-metoden Opret()
        private Appointment(AppointmentTime appointmentTime, Guid type, Guid patient, Guid practitioner)
        {

            AppointmentTime = appointmentTime;
            TreatmentTypeId = type;
            PatientId = patient;
            PractitionerId = practitioner;
            Status = TreatmentStatus.Booked;

        }
        // ── Factory-metode: eneste måde at oprette en Konsultation ──────
        public static Appointment Create(
            AppointmentTime appointmentTime,
            Guid treatmentTypeId,
            Guid patientId,
            Guid practitionerId,
            IEnumerable<Appointment> existingForPatient,
            IEnumerable<Appointment> existingForPractitioner)
        {
            var appointment = new Appointment(appointmentTime, treatmentTypeId, patientId, practitionerId);
            ValidateNoneOverlap(appointment, existingForPatient, existingForPractitioner);
            return appointment;
        }
        public void OpdaterBehandlingstype(Guid newType)
        {           //Hvis behandlingen er aflyst, gennemført eller Noshow, kan behandlingstypen ikke opdateres
            if (Status == TreatmentStatus.Cancelled || Status == TreatmentStatus.Completed || Status == TreatmentStatus.NoShow)
            {
                throw new DomainException("Kan ikke opdatere behandlingstype på en enten aflyst, noshow eller gennemført behandling");
            }
            TreatmentTypeId = newType;


        }
        public bool IsActive => Status == TreatmentStatus.Booked;
        private static void ValidateNoneOverlap(Appointment appointment, IEnumerable<Appointment> existingForPatient, IEnumerable<Appointment> existingForPractitioner)
        {
            var activeForPatient = existingForPatient.Where(k => k.IsActive);
            var activeForPractitioner = existingForPractitioner.Where(k => k.IsActive);

            if (activeForPatient.Any(existingAppointment => appointment.AppointmentTime.Overlapping(existingAppointment.AppointmentTime)))
            {
                throw new DomainException("Der er overlap mellem en anden behandling for patienten");
            }

            if (activeForPractitioner.Any(existingAppointment => appointment.AppointmentTime.Overlapping(existingAppointment.AppointmentTime)))
            {
                throw new DomainException("Der er overlap mellem en anden behandling for behandler");
            }
        }
    }
}
