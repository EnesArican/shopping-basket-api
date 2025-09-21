namespace ShoppingBasket.Application.Components.Services;

public static class DiscountCodeService
{
    private static readonly Dictionary<string, int> ValidDiscountCodes = new()
    {
        { "SAVE10", 10 },
        { "SAVE15", 15 },
        { "SAVE20", 20 },
        { "SAVE25", 25 },
        { "WINTER10", 10 },
        { "WINTER15", 15 },
        { "WINTER20", 20 },
        { "STUDENT10", 10 },
        { "STUDENT15", 15 },
        { "NEWUSER10", 10 },
        { "NEWUSER20", 20 },
        { "LOYALTY10", 10 },
        { "LOYALTY15", 15 },
        { "VIP25", 25 },
        { "VIP30", 30 }
    };

    public static bool IsValidDiscountCode(string discountCode)
    {
        if (string.IsNullOrWhiteSpace(discountCode))
            return false;

        var normalizedCode = discountCode.ToUpperInvariant();
        return ValidDiscountCodes.ContainsKey(normalizedCode);
    }

    public static bool TryGetDiscountPercentage(string discountCode, out int discountPercentage)
    {
        discountPercentage = 0;
        
        if (string.IsNullOrWhiteSpace(discountCode))
            return false;
            
        var normalizedCode = discountCode.ToUpperInvariant();
        return ValidDiscountCodes.TryGetValue(normalizedCode, out discountPercentage);
    }

    public static bool TryGetDiscountPercentage(string discountCode, out decimal discountPercentage)
    {
        discountPercentage = 0;
        
        if (TryGetDiscountPercentage(discountCode, out int intPercentage))
        {
            discountPercentage = intPercentage;
            return true;
        }
        
        return false;
    }

    public static IReadOnlyDictionary<string, int> GetAllValidCodes()
    {
        return ValidDiscountCodes.AsReadOnly();
    }
}
