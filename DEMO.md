# Book Order Processor Demo

The following demonstrates the usage of `BookOrderProcessor` to handle book orders.
The `Process()` and `Cancel()` methods of the order processor provide complete control
of the order's lifecycle, and trigger `BookOrder` state transitions.


### 1. Initialize inventory
To check inventory balances, request the service from the service provider:
```csharp
var inventory = serviceProvider.GetService<IInventoryService>()!;
inventory.PrintInventory();
```
The output prints inventory by item:
```text
| Item        |  Quantity |
---------------------------
| Black Book  |       100 |
| Red Book    |       100 |
| Blue Book   |       100 |
| Green Book  |       100 |
| White Book  |       100 |
```

### 2. Create an order
```csharp
var order = new BookOrder()
{
    OrderId = "ORD_001", 
    DeliveryDestination = "London",
    PaymentInfo = new("Visa", 100),
    OrderLines = new List<OrderLine>()
    {
        new("Red Book", 250, 15),
        new("Green Book", 20, 25),
        new("Blue Book", 30, 35),
        new("Black Book", 40, 45),
    }
};
order.PrintOrder();
```
Output - new order in status **Draft**:
```text
Order:		ORD_001
Status:		Draft
Destination:	London
Card Id:	Visa
Balance:	100
| Ln | Book Name   |  Ordered |  Allocated |  Price |   Status |
----------------------------------------------------------------
| 0  | Red Book    |      250 |          0 |     15 |  Updated |
| 1  | Green Book  |       20 |          0 |     25 |  Updated |
| 2  | Blue Book   |       30 |          0 |     35 |  Updated |
| 3  | Black Book  |       40 |          0 |     45 |  Updated |
```


### 3. Process the order
Request order processor factory from the service provider. Then, use the factory
to create a processor for the order. At the end, invoke `Process()`:
```csharp
var procFactory = serviceProvider.GetService<OrderProcessorFactory>();
var proc = procFactory!.GetProcessor(order);
proc.Process();
order.PrintOrder();
```
After processing the order, maximum available quantity of `Red Book` was allocated
in the inventory. However, not all ordered quantity was available, therefore
the status of the order is `InsufficientInventory`:
```text
Order:		ORD_001
Status:		InsufficientInventory
Destination:	London
Card Id:	Visa
Balance:	100
| ## | Book Name   |  Ordered |  Allocated |  Price |              Status |
---------------------------------------------------------------------------
| 0  | Red Book    |      250 |        100 |     15 |  PartiallyAllocated |
| 1  | Green Book  |       20 |         20 |     25 |           Allocated |
| 2  | Blue Book   |       30 |         30 |     35 |           Allocated |
| 3  | Black Book  |       40 |         40 |     45 |           Allocated |
```

### 4. Update ordered amount
```csharp
order.OrderLines[0].UpdateOrdered(25);
order.OrderLines[2].UpdateOrdered(50);
order.PrintOrder();
```
Updating the amount of the lines `0` and `2` changes the Status of the lines to `Updated`:
```
Order:		ORD_001
Status:		InsufficientInventory
Destination:	London
Card Id:	Visa
Balance:	100
| ## | Book Name   |  Ordered |  Allocated |  Price |     Status |
------------------------------------------------------------------
| 0  | Red Book    |       25 |        100 |     15 |    Updated |
| 1  | Green Book  |       20 |         20 |     25 |  Allocated |
| 2  | Blue Book   |       50 |         30 |     35 |    Updated |
| 3  | Black Book  |       40 |         40 |     45 |  Allocated |
```

### 5. Process the order
```csharp
proc.Process();
order.PrintOrder();
```
After processing the updated lines, the order status changes to `LinesApproved`.
At this stage the user is not allowed to call `UpateOrdered()` on the order lines
(this will throw an exception).
```
Order:		ORD_001
Status:		LinesApproved
Destination:	London
Card Id:	Visa
Balance:	100
| ## | Book Name   |  Ordered |  Allocated |  Price |    Status |
-----------------------------------------------------------------
| 0  | Red Book    |       25 |         25 |     15 |  Approved |
| 1  | Green Book  |       20 |         20 |     25 |  Approved |
| 2  | Blue Book   |       50 |         50 |     35 |  Approved |
| 3  | Black Book  |       40 |         40 |     45 |  Approved |
```

### 6. Check inventory
```csharp
inventory.PrintInventory();
```
After the allocation of the order, inventory balances for the ordered items are updated accordingly:
```
| Item        |  Quantity |
---------------------------
| Red Book    |        75 |
| Blue Book   |        50 |
| Green Book  |        80 |
| Black Book  |        60 |
| White Book  |       100 |
```


### 7. Process the order
```csharp
proc.Process();
order.PrintOrder();
```
Now when the order is completely allocated, the `Process()` will attempt to charge the payment.
Since the balance on the card is only 100, the payment is rejected:
```
Order:		ORD_001
Status:		PaymentRejected
Destination:	London
Card Id:	Visa
Balance:	100
| ## | Book Name   |  Ordered |  Allocated |  Price |    Status |
-----------------------------------------------------------------
| 0  | Red Book    |       25 |         25 |     15 |  Approved |
| 1  | Green Book  |       20 |         20 |     25 |  Approved |
| 2  | Blue Book   |       50 |         50 |     35 |  Approved |
| 3  | Black Book  |       40 |         40 |     45 |  Approved |
```


### 8. Update payment info and process again
Assign another payment card to the order with balance of 10,000:
```csharp
order.PaymentInfo = new("Master Card", 10000);
proc.Process();
order.PrintOrder();
```
The payment is approved now and the balance of the card is updated accordingly:
```
Order:		ORD_001
Status:		PaymentApproved
Destination:	London
Card Id:	Master Card
Balance:	5575
| ## | Book Name   |  Ordered |  Allocated |  Price |    Status |
-----------------------------------------------------------------
| 0  | Red Book    |       25 |         25 |     15 |  Approved |
| 1  | Green Book  |       20 |         20 |     25 |  Approved |
| 2  | Blue Book   |       50 |         50 |     35 |  Approved |
| 3  | Black Book  |       40 |         40 |     45 |  Approved |
```

### 9. Process the order
```csharp
proc.Process();
order.PrintOrder();
```
The order status updated to Returned, because the delivery to London is not supported:
```
Order:		ORD_001
Status:		Returned
Destination:	London
Card Id:	Master Card
Balance:	5575
| ## | Book Name   |  Ordered |  Allocated |  Price |    Status |
-----------------------------------------------------------------
| 0  | Red Book    |       25 |         25 |     15 |  Approved |
| 1  | Green Book  |       20 |         20 |     25 |  Approved |
| 2  | Blue Book   |       50 |         50 |     35 |  Approved |
| 3  | Black Book  |       40 |         40 |     45 |  Approved |
```


### 10. Cancel the order
At this stage it's possible to update the delivery destination, or to cancel the order:
```csharp
proc.Cancel();
order.PrintOrder();
```
After cancelling the order, the payment is refunded and the balance of the card becomes 10,000 again:
```
Order:		ORD_001
Status:		Cancelled
Destination:	London
Card Id:	Master Card
Balance:	10000
| ## | Book Name   |  Ordered |  Allocated |  Price |     Status |
------------------------------------------------------------------
| 0  | Red Book    |       25 |         25 |     15 |  Cancelled |
| 1  | Green Book  |       20 |         20 |     25 |  Cancelled |
| 2  | Blue Book   |       50 |         50 |     35 |  Cancelled |
| 3  | Black Book  |       40 |         40 |     45 |  Cancelled |
```

### 11. Check the inventory
After cancelling, the allocated inventory is returned back to the stock:
```
| Item        |  Quantity |
---------------------------
| Red Book    |       100 |
| Blue Book   |       100 |
| Green Book  |       100 |
| Black Book  |       100 |
| White Book  |       100 |
```

