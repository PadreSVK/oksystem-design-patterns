using OrderService.Domain;

namespace OrderService.Application;

// =============================================================================
// Task 2 — Implement the Application Service (Handler)
// =============================================================================
//
// The Application layer orchestrates use cases. It depends on Domain (for
// entities + repository interface) but NEVER on Infrastructure.
//
// YOUR TASKS:
//   1. Implement CreateOrderHandler with a constructor that takes IOrderRepository.
//   2. In HandleAsync:
//      a. Map CreateOrderCommand → Domain Order entity (use Order.Create + OrderLine.Create)
//      b. Call repository.AddAsync(order, ct)
//      c. Call repository.SaveChangesAsync(ct)
//      d. Return a CreateOrderResponse with the order details.
//
// IMPORTANT: This file must NOT reference anything from OrderService.Infrastructure.
// =============================================================================

public class CreateOrderHandler
{
    // TODO: Accept IOrderRepository via constructor

    /// <summary>
    /// TODO: Implement the use case:
    ///   1. Map command lines → OrderLine.Create(...)
    ///   2. Create the Order aggregate via Order.Create(...)
    ///   3. Add to repository and save
    ///   4. Return CreateOrderResponse
    /// </summary>
    public Task<CreateOrderResponse> HandleAsync(
        CreateOrderCommand command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
