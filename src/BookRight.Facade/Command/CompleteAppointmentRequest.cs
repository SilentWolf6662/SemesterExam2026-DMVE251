using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Command
{
    public record CompleteAppointmentRequest(Guid AppointmentId, string Note);
}
