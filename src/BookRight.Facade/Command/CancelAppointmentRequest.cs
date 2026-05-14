using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Command
{
    public record CancelAppointmentRequest(Guid AppointmentId);
}
