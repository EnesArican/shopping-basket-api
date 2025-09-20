namespace ShoppingBasket.Api;

public static class ApiRoutes
{
    public static class Basket
    {
        public const string Create = "";
        public const string AddItem = "items";
        public const string RemoveItem = "items/{itemId}";
        public const string UpdateItemQuantity = "items/{itemId}/quantity";
        public const string GetTotal = "{id}/total";
    }

    public static class Item
    {
        public const string GetAll = "";
    }
}
