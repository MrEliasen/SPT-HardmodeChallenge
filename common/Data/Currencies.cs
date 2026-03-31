namespace Vagabond.Common.Data;

public static class Currencies
{
    public static string Dollar => CurrencyIds[0];
    public static string Euro => CurrencyIds[1];
    public static string Ruble => CurrencyIds[2];
    
    public static readonly List<string> CurrencyIds = new()
    {
        "5696686a4bdc2da3298b456a", // Dollars
        "569668774bdc2da2298b4568", // Euros
        "5449016a4bdc2d6f028b456f" // Ruble
    };
}