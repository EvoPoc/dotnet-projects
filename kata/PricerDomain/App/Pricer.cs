namespace PricerDomain.Application;

public class Pricer(IPriceProvider priceProvider, IFxProvider fxProvider) : IPricer
{
    public double GetPrice(string targetCurrency)
    {
        // Implement pricing logic here
        throw new NotImplementedException();
    }
}
