using BookRight.Domain.Enums;

namespace BookRight.UseCases.Discount.DiscountStrategy;

public class BlackFridayDiscountStrategy : IDiscountStrategy
{
    public string Name => "Black Friday Rabat";

    private decimal DiscountRate = 0.25m; // 25% rabat

    async Task<DiscountResult> IDiscountStrategy.Calculate(DiscountInput input)
    {
        // Hvis sluttiden for appointment ligger på black friday kan der gives rabat
        if (IsBlackFriday(input.From))
        {
            decimal discount = input.CurrentPrice * DiscountRate;

            // Returnere en DiscountResult med rabatten og angiver, at den er gyldig
            return new DiscountResult(Name, discount, true, DiscountType.BlackFriday);
        }
        else // ellers retuneres der ingen rabat
        {
            return new DiscountResult(Name, 0, false, DiscountType.None);
        }
    }

    private bool IsBlackFriday(DateTime date)
    {
        var today = date.Date; // lig dagens dato uden tidskomponent i en variabel

        // Returner true, hvis dagens dato ligger i november og er den sidste fredag
        return today.Month == 11 && today.Day == BlackFridayDay(today.Year);
    }

    private static int BlackFridayDay(int year)
    {
        var november1 = new DateTime(year, 11, 1);

        // Der bliver fundet, hvor mange dage der er fra den 1. november til den 1. fredag i november,
        // og derefter bruges modulus til at sikre, at det altid returnerer et tal mellem 0 og 6
        var daysToFirstFriday = ((int)DayOfWeek.Friday - (int)november1.DayOfWeek + 7) % 7;

        // Siden det returnerede tal er mellem 0-6, skal der ligges 1 til for at få den faktiske dato for den 1. fredag i november
        var firstFriday = daysToFirstFriday + 1;

        return firstFriday + 21; // Der ligges 3 uger (21 dage) på, da den 4. fredag i november er Black Friday
    }
}
