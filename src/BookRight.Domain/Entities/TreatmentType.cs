using BookRight.Domain.Enums;
using BookRight.Domain.ValueObjects;
using System.Diagnostics;
using BookRight.Domain.Exceptions;

namespace BookRight.Domain.Entities;

public class TreatmentType : AggregateRoot
{
    public string Name { get; private set; }

    // Bestemmer hvilken type behandler der må udføre behandlingen (fx kun fysioterapeut)
    public AuthorizationType AuthorizationType { get; private set; }

    // Null betyder at behandlingen er individuel (én patient).
    // En værdi (fx 6) betyder at det er holdtræning med et maksimalt deltagerantal.
    public int? MaxParticipants { get; private set; }

    // Listen af gyldige prisvarianter for denne behandlingstype.
    // Hver TreatmentPrice indeholder en varighed i minutter og en tilhørende basispris.
    // Gemmes som JSON i databasen via EF Core ComplexCollection.
    public IReadOnlyList<TreatmentPrice> Prices { get; private set; } = [];

    private TreatmentType() { } // EF-Core kræver en parameterløs konstruktør

    private TreatmentType(string name, AuthorizationType authorizationType, int? maxParticipants, IEnumerable<TreatmentPrice> prices)
    {
        Name = name;
        AuthorizationType = authorizationType;
        MaxParticipants = maxParticipants;
        // ToList() laver en konkret liste fra IEnumerable så vi ikke evaluerer den løvent flere gange
        Prices = prices.ToList();
    }

    // Slår basisprisen op for en bestemt varighed.
    // Kaster DomainException hvis den ønskede varighed ikke har en defineret pris —
    // det sikrer at BookAppointmentUseCase ikke kan oprette en booking med en ugyldig varighed.
    public decimal GetBasePrice(int durationMinutes)
    {
        var price = Prices.FirstOrDefault(p => p.DurationMinutes == durationMinutes)
            ?? throw new DomainException($"Ingen pris fundet for {durationMinutes} minutters behandling");
        return price.BasePrice;
    }

    // Factory-metode — eneste måde at oprette en TreatmentType på udefra
    public static TreatmentType Create(string name, AuthorizationType authorizationType, int? maxParticipants, IEnumerable<TreatmentPrice> prices)
    {
        return new TreatmentType(name, authorizationType, maxParticipants, prices);
    }
}