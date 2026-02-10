using Domain.Entities;
using Domain.Ports;

namespace Domain.App;

public class WalletPricer(IPriceProvider priceProvider, IFxProvider fxProvider) : IWalletPricer
{
    public IReadOnlyCollection<Position> Positions { get; init; } = Array.Empty<Position>();
    public decimal PriceWithCurrency(string targetCurrency = "EUR")
    {
        decimal total = 0m;
        foreach (var position in Positions)
        {
            var price = priceProvider.GetPrice(position.isin, position.quantity, position.currency);
            var fxRate = fxProvider.GetFxRate(position.currency, targetCurrency);
            if (fxRate == null)
            {
                throw new InvalidOperationException($"No FX rate available to convert from {position.currency} to {targetCurrency}");
            }
            total += price * fxRate.Value;
        }
        return Math.Round(total, 2);
    }

}
