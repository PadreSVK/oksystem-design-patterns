# Hands-On 016 — Anti-Pattern Identification Lab

This lab focuses on identifying common anti-patterns in .NET applications and refactoring them using the design principles (GRASP, SOLID) and patterns (GoF, Enterprise) discussed in the course.

## Task

For each snippet in `Snippets.cs`:
1. Identify the primary anti-pattern.
2. Identify which GRASP or SOLID principle it violates.
3. Propose a design pattern to fix it.
4. Refactor the code to implement the fix.

## The 5 Snippets

1. **God Class:** `UserManager` handles too many unrelated responsibilities.
2. **Anemic Domain Model:** `OrderDto` is just data, while logic is externalized in a service.
3. **Conditional Complexity:** A large `switch` statement handles different payment types.
4. **Service Locator:** Using a global locator instead of dependency injection.
5. **Leaky Abstraction:** A repository interface that exposes implementation details (SQL).

## How to use this lab

- Open `Snippets.cs` and try to refactor the code yourself.
- Compare your results with `Snippets-Solution.cs`.
- Ensure your solutions follow .NET 8 idioms (Primary constructors, records, etc. where applicable).
