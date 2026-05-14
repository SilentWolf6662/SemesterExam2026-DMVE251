using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Command
{
    public record BookAppointmentRequest(DateTime From, DateTime To, Guid TreatmentTypeId, Guid PatientId, Guid PractitionerId);
}
