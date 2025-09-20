namespace ShoppingBasket.Api.Dtos;

public record AddBasketItemsRequestDto(
    List<AddBasketItemDto> Items);
