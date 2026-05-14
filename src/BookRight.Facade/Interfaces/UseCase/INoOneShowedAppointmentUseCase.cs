using BookRight.Facade.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.UseCase
{
    public interface INoOneShowedAppointmentUseCase
    {
        Task Execute(NoOneShowedAppointmentRequest request);
    }
}
