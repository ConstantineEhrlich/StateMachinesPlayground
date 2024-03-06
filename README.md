# State Machines Playground
The project `BookStore` is a simple book store application, that carries order handling process implemented as state machine.
The project is built on .Net 6 and uses Stateless 3.0 library to manage the state machine.

## BookStoreDemo Project
This project provides a demonstration of using the IBookOrderProcessor to process the order.
See [DEMO.md](DEMO.md) for details.


## BookStore Project

### Structure
The **BookStore** project contains of two high-level dependencies:
1. `BookOrder` - encapsulates the details of the order and related information:
    - A list of `OrderLine` objects, representing the ordered books, quantities and price
    - `DeliveryDestination` string, representing a delivery address
    - `PaymentInfo` object, representing an account with a balance
2. `IBookOrderProcessor` - provides a control over the `BookOrder` lifecycle through two methods:
   - `Process()` triggers transition of the order to the next possible state
   - `Cancel()` undoes all performed operations and transitions to the cancelled state

Additionally, the project provides several services, that are intended to run as singletons:
1. `IInventoryService` - interface that handles inventory allocation of order lines
2. `IDeliveryService` - interface that checks if delivery is possible to the specified destination
3. `IPaymentService` - interface responsible for charging the payment from the `PaymentInfo`

Since the `BookOrderProcessor` class requires `BookOrder` in the constructor, it can't be registered in the .Net dependency injection container.
Therefore, factory pattern was used in the `OrderProcessorFactory`.
The factory can be injected as a scoped service and provides method to create order processor. 

### State Machines
There are two state machines in the project:
#### OrderLine machine
`OrderLine` class configures the state machine within itself and provides following states:
- `Updated`
- `PartiallyAllocated`
- `Allocated`
- `Approved`
- `Cancelled`

The transitions are triggered by invoking the following methods of the `OrderLine` class:
- `UpdateOrdered()` allows changing the ordered quantity and transitions the machine to the `Updated` state.
- `Allocate()` updates the allocated amount and transitions the machine to the `Allocated` or `PartiallyAllocated` state.
This method is `internal` and should be called only from `InventoryService`.
- `Cancel()` transitions the machine to the `Cancelled` state. This method is also `internal` and should be used only by `InventoryService`.
- `Approve()` transitions the machine to the final `Approved` state, where updating the ordered quantity is not permitted anymore.

#### BookOrder machine
The `BookOrder` class defines the States and Triggers that are used by the machine.
Since the processing of the order requires inventory, delivery and payment services,
the machine itself is configured in the `BookOrderProcessor` class.

**States**:
- `Draft`
- `LinesApproved`
- `InsufficientInventory`
- `PaymentRejected`
- `PaymentApproved`
- `Returned`
- `Delivered`
- `Cancelled`

The transitions are triggered by calling the corresponding methods on the `BookProcessor` instance:
- `Process()`, according to the machine's state, executes a corresponding action, like inventory allocation, payment charge,
or delivery attempt. The success or the failure of the action dictates the state, to which the machine is transitioned.
For example, in state `Lines Approved`, calling `Process()` will try to charge the payment from the `PaymentInfo` object of the order.
- `Cancel()`, according to the machine's state, executes an actions to cancel the order. For example, if the machine is in state
`PaymentApproved`, calling `Cancel()` will refund the payment and also return the inventory to the stock.