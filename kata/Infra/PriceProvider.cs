using Domain.Ports;

namespace Infra;

public class PriceProvider : IPriceProvider
{
    public decimal GetPrice(string isin, int quantity, string currency)
    {
        throw new NotImplementedException();
    }
}
