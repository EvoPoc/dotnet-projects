namespace Domain.App;

public interface IWalletPricer
{
    public decimal PriceWithCurrency(string targetCurrency);
}