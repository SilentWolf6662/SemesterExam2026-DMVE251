using BookRight.Facade.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.UseCase
{
    public interface ICompleteAppointmentUseCase
    {
        Task Execute(CompleteAppointmentRequest request);
    }
}
