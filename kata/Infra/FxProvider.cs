using Domain.Ports;

namespace Infra;

public class FxProvider : IFxProvider
{
    public decimal? GetFxRate(string fromCurrency, string toCurrency)
    {
        throw new NotImplementedException();
    }
}
