Snippet 1 — PaymentFactory: Creational. A factory that must be edited for every new PaymentKind, and BankTransfer has no case → throws at runtime.
Snippet 2 — XmlExportAdapter: Structural (Adapter). The `Inner` property leaks the wrapped legacy library, defeating the adapter's isolation.
Snippet 3 — ShippingCalculator: Behavioral (algorithm selection). if/else instead of polymorphism, and the catch-all `else` silently undercharges unknown speeds.

> Each snippet is categorizable from the Pattern Classification table alone. The "flag" answers rely only on general C# experience — no pattern internals are needed yet.

---

## Snippet 1 — `PaymentFactory`

| Question | Answer |
| --- | --- |
| **Category** | Creational |
| **Trying to do** | Centralize creation of `IPaymentProcessor` instances (a simple/static factory). |
| **What to flag** | **Not open for extension** — every new `PaymentKind` forces editing the `switch` (OCP smell). Also **`BankTransfer` is a declared enum value with no case**, so `Create(PaymentKind.BankTransfer)` throws at runtime rather than failing at compile time. Fix: register processors by kind (dictionary / DI). |

---

## Snippet 2 — `XmlExportAdapter`

| Question | Answer |
| --- | --- |
| **Category** | Structural |
| **Trying to do** | Adapt the third-party `LegacyXmlExporter` to the app's `IReportExporter` interface (Adapter). |
| **What to flag** | **The `Inner` property leaks the adaptee.** An adapter exists to isolate the rest of the code from the wrapped library; exposing `LegacyXmlExporter` lets callers bind to it again and undoes that isolation. Fix: drop `Inner`, keep the adaptee private. |

---

## Snippet 3 — `ShippingCalculator` (subtle violation)

| Question | Answer |
| --- | --- |
| **Category** | Behavioral |
| **Trying to do** | Select a pricing algorithm at runtime based on `speed` (the intent of Strategy). |
| **What to flag** | **Two issues.** (1) *Design:* algorithm selection is an `if/else` chain instead of polymorphism — every new `ShippingSpeed` edits this method (conditional-complexity smell). (2) *Correctness (subtle):* the final `else` is a catch-all, so an unrecognized speed **silently returns the base rate** (undercharges) instead of failing. With an enum it looks safe, but a cast/overflow value or a newly added member slips into the wrong branch. Fix: one rule per speed (Strategy), or an exhaustive `switch` expression that throws on unknown values. |
