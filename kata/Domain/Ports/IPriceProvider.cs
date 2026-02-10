namespace Domain.Ports;

public interface IPriceProvider
{
    decimal GetPrice(string isin, int quantity, string currency);
}
