# shopping-basket-api
Sample REST API for an online shopping basket

## How To Run

**Prerequisites** </br>
To run the application, you will need to have [docker](https://docs.docker.com/get-docker/) installed on your machine along with [docker compose](https://docs.docker.com/compose/install/).
</br>

**Build and run the application** </br>
Open a command prompt and navigate to the project folder. Start up the application by running the `run-api.sh` bash script.
```shell
$ ./run-api.sh
```
Compose will build the image and run the application.

The app should now be running on http://localhost:8000. 

**Endpoints**</br>
Information on the endpoints can be found on
http://localhost:8000/swagger.

## API Usage

The Shopping Basket API follows a simple workflow:

1. **Create a basket** - Start by creating an empty shopping basket
2. **Add items** - Add items from the predefined catalog using their IDs
3. **Apply discounts** (optional) - Use discount codes or add discounted items
4. **Set shipping** - Configure shipping destination for cost calculation
5. **Get totals** - Retrieve basket totals with or without VAT (20%)

### Basic Workflow Example

```bash
# 1. Create a basket
curl -X POST http://localhost:8000/baskets

# 2. Add items to the basket (using basket ID from step 1)
curl -X POST http://localhost:8000/baskets/{basketId}/items \
  -H "Content-Type: application/json" \
  -d '{"itemId": "11111111-1111-1111-1111-111111111111", "quantity": 1}'

# 3. Apply a discount code
curl -X POST http://localhost:8000/baskets/{basketId}/discount \
  -H "Content-Type: application/json" \
  -d '{"discountCode": "SAVE10"}'

# 4. Set shipping destination
curl -X POST http://localhost:8000/baskets/{basketId}/shipping \
  -H "Content-Type: application/json" \
  -d '{"country": "UK"}'

# 5. Get basket total
curl -X GET http://localhost:8000/baskets/{basketId}/total?includeVat=true
```

## Available Items

The API uses a hardcoded catalog of items. Use these Item IDs when adding items to baskets:

| Item ID | Name | Price |
|---------|------|-------|
| `11111111-1111-1111-1111-111111111111` | Laptop | £1,000.00 |
| `22222222-2222-2222-2222-222222222222` | Wireless Mouse | £30.00 |
| `33333333-3333-3333-3333-333333333333` | Mechanical Keyboard | £150.00 |
| `44444444-4444-4444-4444-444444444444` | USB-C Hub | £80.00 |
| `55555555-5555-5555-5555-555555555555` | Wireless Headphones | £200.00 |
| `66666666-6666-6666-6666-666666666666` | External Monitor | £300.00 |
| `77777777-7777-7777-7777-777777777777` | Smartphone | £700.00 |
| `88888888-8888-8888-8888-888888888888` | Tablet | £400.00 |

You can also retrieve all available items using:
```bash
curl -X GET http://localhost:8000/items
```

## Shipping Information

Shipping costs are automatically calculated based on the destination country:

- **UK**: £5.99 (fixed rate)
- **International** (any country other than UK): £12.99 (fixed rate)

The country comparison is case-insensitive, so "UK", "uk", "Uk", and "uK" all qualify for UK shipping rates.

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
