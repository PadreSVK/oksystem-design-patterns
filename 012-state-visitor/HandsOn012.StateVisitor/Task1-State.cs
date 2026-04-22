// =============================================================================
// Hands-On 012 — Task 1: Order Lifecycle with State Pattern  (starter)
//
// Scenario: an Order moves through a fixed lifecycle:
//
//   Draft ──Confirm──► Confirmed ──Ship──► Shipped  (terminal)
//     │                   │
//     └──Cancel──►         └──Cancel──► Cancelled  (terminal)
//
// Any attempt to call an illegal transition (e.g. Ship a Draft order)
// must throw InvalidOperationException.
//
// Your task:
//   Implement the four concrete state classes.  Each one overrides only the
//   transitions that are *valid* from that state.  Invalid transitions are
//   handled automatically by OrderStateBase — you do NOT need to override them.
//
// State pattern reminder:
//   • IOrderState        = State interface
//   • OrderStateBase     = Abstract State (provided) — default behaviour: throw
//   • DraftState etc.    = Concrete States (your task)
//   • Order              = Context (provided) — delegates calls to CurrentState
// =============================================================================

namespace HandsOn.StateVisitor;

// ─── State interface ──────────────────────────────────────────────────────────

public interface IOrderState
{
    string StateName { get; }
    void Confirm(Order order);
    void Ship(Order order);
    void Cancel(Order order);
}

// ─── Abstract base state (provided — do not modify) ──────────────────────────

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

// ─── Order context (provided — do not modify) ────────────────────────────────

public sealed class Order
{
    public IOrderState CurrentState { get; private set; } = new DraftState();
    public string StatusName => CurrentState.StateName;

    /// <summary>Called by state objects to transition the order to a new state.</summary>
    internal void TransitionTo(IOrderState newState) => CurrentState = newState;

    public void Confirm() => CurrentState.Confirm(this);
    public void Ship()    => CurrentState.Ship(this);
    public void Cancel()  => CurrentState.Cancel(this);
}

// ─── Concrete states (your task) ─────────────────────────────────────────────

/// <summary>Initial state. Can be confirmed or cancelled; cannot be shipped.</summary>
public sealed class DraftState : OrderStateBase
{
    public override string StateName => "Draft";

    public override void Confirm(Order order)
    {
        // TODO: order.TransitionTo(new ConfirmedState())
        throw new NotImplementedException();
    }

    public override void Cancel(Order order)
    {
        // TODO: order.TransitionTo(new CancelledState())
        throw new NotImplementedException();
    }
}

/// <summary>Confirmed state. Can be shipped or cancelled; cannot be confirmed again.</summary>
public sealed class ConfirmedState : OrderStateBase
{
    public override string StateName => "Confirmed";

    public override void Ship(Order order)
    {
        // TODO: order.TransitionTo(new ShippedState())
        throw new NotImplementedException();
    }

    public override void Cancel(Order order)
    {
        // TODO: order.TransitionTo(new CancelledState())
        throw new NotImplementedException();
    }
}

/// <summary>Terminal state. No further transitions are allowed.</summary>
public sealed class ShippedState : OrderStateBase
{
    public override string StateName => "Shipped";
    // All transitions are invalid — base class handles them automatically
}

/// <summary>Terminal state. No further transitions are allowed.</summary>
public sealed class CancelledState : OrderStateBase
{
    public override string StateName => "Cancelled";
    // All transitions are invalid — base class handles them automatically
}
