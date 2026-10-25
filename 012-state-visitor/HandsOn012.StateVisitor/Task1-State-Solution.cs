// =============================================================================
// Hands-On 012 — Task 1: Order Lifecycle with State Pattern  (solution)
// =============================================================================

namespace HandsOn.StateVisitor;

public interface IOrderState
{
    string StateName { get; }
    void Confirm(Order order);
    void Ship(Order order);
    void Cancel(Order order);
}

public abstract class OrderStateBase : IOrderState
{
    public abstract string StateName { get; }

    public virtual void Confirm(Order order) => ThrowInvalid("confirm");
    public virtual void Ship(Order order)    => ThrowInvalid("ship");
    public virtual void Cancel(Order order)  => ThrowInvalid("cancel");

    private void ThrowInvalid(string action) =>
        throw new InvalidOperationException(
            $"Cannot {action} an order that is in state '{StateName}'.");
}

public sealed class Order
{
    public IOrderState CurrentState { get; private set; } = new DraftState();
    public string StatusName => CurrentState.StateName;

    internal void TransitionTo(IOrderState newState) => CurrentState = newState;

    public void Confirm() => CurrentState.Confirm(this);
    public void Ship()    => CurrentState.Ship(this);
    public void Cancel()  => CurrentState.Cancel(this);
}

// ✅ Concrete states — each overrides only valid transitions;
//    invalid ones are blocked by the base class automatically.

public sealed class DraftState : OrderStateBase
{
    public override string StateName => "Draft";

    public override void Confirm(Order order) => order.TransitionTo(new ConfirmedState());
    public override void Cancel(Order order)  => order.TransitionTo(new CancelledState());
}

public sealed class ConfirmedState : OrderStateBase
{
    public override string StateName => "Confirmed";

    public override void Ship(Order order)   => order.TransitionTo(new ShippedState());
    public override void Cancel(Order order) => order.TransitionTo(new CancelledState());
}

public sealed class ShippedState : OrderStateBase
{
    public override string StateName => "Shipped";
}

public sealed class CancelledState : OrderStateBase
{
    public override string StateName => "Cancelled";
}
