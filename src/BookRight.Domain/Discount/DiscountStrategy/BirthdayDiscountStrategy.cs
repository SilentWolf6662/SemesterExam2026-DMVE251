using BookRight.Domain.Enums;
using BookRight.UseCases.Discount;

namespace BookRight.UseCases.Discount.DiscountStrategy;

public class BirthdayDiscountStrategy : IDiscountStrategy
{
    public string Name => "Fødselsdagsrabat";
    private decimal DiscountRate => 0.25m; // 25% rabat
    private readonly int MaxPerYear = 1; // Max antal gange rabatten kan bruges per år

    async Task<DiscountResult> IDiscountStrategy.Calculate(DiscountInput input)
    {
        // TODO: BugFix: bruger DateTime.Now.Month i stedet for input.From.Month — det betyder rabattjekket er baseret på i dag og ikke behandlingsdatoen (Kan i princippet ligge i næste måned).
        // Hvis patientens fødselsmåned matcher den nuværende måned, og de ikke har brugt fødselsdagsrabatten, kan der gives rabat
        if (await IsBirthday(input.PatientBirthDate) && input.BirthdayDiscountUsedCount < MaxPerYear)
        {
            decimal discount = input.CurrentPrice * DiscountRate;

            // Returner en DiscountResult med rabatten og angiv, at den er gyldig
            return new DiscountResult(Name, discount, true, DiscountType.Birthday);
        }
        else // ellers retuneres der ingen rabat
        {
            return new DiscountResult(Name, 0, false, DiscountType.None);
        }
    }

    private async Task<bool> IsBirthday(DateOnly patientBirthMonth)
    {
        // Tjek om dagen for appointmentDate matcher dagen for patientBirthMonth
        return DateTime.Now.Month == patientBirthMonth.Month;
    }
}
