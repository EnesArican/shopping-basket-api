# shopping-basket-api
Sample REST API for an online shopping basket

## Valid Discount Codes

The following discount codes are supported:

### SAVE Series
- `SAVE10` - 10% discount
- `SAVE15` - 15% discount  
- `SAVE20` - 20% discount
- `SAVE25` - 25% discount

### WINTER Series
- `WINTER10` - 10% discount
- `WINTER15` - 15% discount
- `WINTER20` - 20% discount

### STUDENT Series
- `STUDENT10` - 10% discount
- `STUDENT15` - 15% discount

### NEW USER Series
- `NEWUSER10` - 10% discount
- `NEWUSER20` - 20% discount

### LOYALTY Series
- `LOYALTY10` - 10% discount
- `LOYALTY15` - 15% discount

### VIP Series
- `VIP25` - 25% discount
- `VIP30` - 30% discount

## Discount Rules

- Discount codes apply only to items that are **not already discounted**
- Items with existing item-level discounts are excluded from basket-level discount calculations
- Discount codes are case-insensitive (e.g., `save20` equals `SAVE20`)
- Only one discount code can be applied per basket
