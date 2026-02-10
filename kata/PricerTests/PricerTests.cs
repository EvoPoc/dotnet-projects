using System.Diagnostics;
using Domain.App;
using Domain.Ports;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace Tests;

public class PricerTests
{
    [Fact]
    public void Sould_Price_Wallet_With_Default_Currency()
    {
        var priceProvider = Substitute.For<IPriceProvider>();
        var fxProvider = Substitute.For<IFxProvider>();

        //arrange
        priceProvider.GetPrice("isin1", 1, "EUR").Returns(100m);
        priceProvider.GetPrice("isin2", 1, "EUR").Returns(100m);
        fxProvider.GetFxRate("EUR", "EUR").Returns(1m);

        var walletPricer = new WalletPricer(priceProvider, fxProvider)
        {
            Positions = [
            new Domain.Entities.Position("isin1", 1, "EUR"),
            new Domain.Entities.Position("isin2", 1, "EUR")
        ]
        };
        //act
        var price = walletPricer.PriceWithCurrency();
        //assert
        priceProvider.Received(2).GetPrice(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>());

        price.Should().Be(100m * 2);
    }

    [Fact]
    public void Sould_Price_Wallet_With_Different_Currency()
    {
        var priceProvider = Substitute.For<IPriceProvider>();
        var fxProvider = Substitute.For<IFxProvider>();

        //arrange
        priceProvider.GetPrice(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>()).Returns(100m);

        fxProvider.GetFxRate("EUR", "USD").Returns(0.9m);
        fxProvider.GetFxRate("USD", "EUR").Returns(1.1m);
        fxProvider.GetFxRate("EUR", "EUR").Returns(1);

        var walletPricer = new WalletPricer(priceProvider, fxProvider)
        {
            Positions = [
            new Domain.Entities.Position("isin1", 1, "EUR"),
            new Domain.Entities.Position("isin2", 1, "USD")
        ]
        };
        //act
        var price = walletPricer.PriceWithCurrency("EUR");
        //assert
        price.Should().Be(100m * 1.1m + 100m * 1);
    }


    [Fact]
    public void Sould_Throw_When_Currency_Not_Supported()
    {
        var priceProvider = Substitute.For<IPriceProvider>();
        var fxProvider = Substitute.For<IFxProvider>();

        //arrange
        priceProvider.GetPrice(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>()).Returns(100m);

        fxProvider.GetFxRate(Arg.Any<string>(), "AAA").ReturnsNull();
        fxProvider.GetFxRate("EUR", "USD").Returns(0.9m);
        fxProvider.GetFxRate("USD", "EUR").Returns(1.1m);
        fxProvider.GetFxRate("EUR", "EUR").Returns(1);

        var walletPricer = new WalletPricer(priceProvider, fxProvider)
        {
            Positions = [
            new Domain.Entities.Position("isin1", 1, "EUR"),
            new Domain.Entities.Position("isin2", 1, "USD")
        ]
        };
        //act
        Action pricingAction = () => walletPricer.PriceWithCurrency("AAA");

        //assert
        pricingAction.Should().Throw<InvalidOperationException>();

    }

    [Fact]
    public void Sould_Price_Bellow_300ms()
    {
        var priceProvider = Substitute.For<IPriceProvider>();
        var fxProvider = Substitute.For<IFxProvider>();
        var watcher = new Stopwatch();
        //arrange
        priceProvider.GetPrice(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>()).Returns(100m);

        fxProvider.GetFxRate(Arg.Any<string>(), "AAA").ReturnsNull();
        fxProvider.GetFxRate("EUR", "USD").Returns(0.9m);
        fxProvider.GetFxRate("USD", "EUR").Returns(1.1m);
        fxProvider.GetFxRate("EUR", "EUR").Returns(1);

        var walletPricer = new WalletPricer(priceProvider, fxProvider)
        {
            Positions = [.. Enumerable.Range(1, 10_000).Select(i => new Domain.Entities.Position($"isin{i}", 1, i == 1 ? "EUR" : "USD"))]
        };
        //act

        watcher.Start();

        walletPricer.PriceWithCurrency("EUR");

        watcher.Stop();
        //assert
        watcher.ElapsedMilliseconds.Should().BeLessThan(300);
        Trace.WriteLine($"Elapsed time: {watcher.ElapsedMilliseconds} ms");

    }


}