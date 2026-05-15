using BookRight.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.UseCases.Repositories;

public interface IPractitionerRepo
{
    Task AddPractitionerAsync(Practitioner practitioner);
    Task<Practitioner?> GetPractitioner_ByIdAsync(Guid id);
}
