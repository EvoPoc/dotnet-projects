using System;

namespace PricerDomain.Domain.Ports;

public interface IFxProvider
{
    decimal GetFxRate(string fromCurrency, string toCurrency);
}
