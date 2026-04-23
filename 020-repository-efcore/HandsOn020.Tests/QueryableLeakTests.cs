using System.Reflection;
using HandsOn020.Domain;
using Xunit;

namespace HandsOn020.Tests;

public class QueryableLeakTests
{
    [Fact]
    public void IOrderRepository_does_not_expose_IQueryable()
    {
        var methods = typeof(IOrderRepository).GetMethods();

        foreach (var method in methods)
        {
            Assert.False(
                IsOrContainsQueryable(method.ReturnType),
                $"Method '{method.Name}' returns IQueryable — this is a repository anti-pattern. " +
                $"Return IReadOnlyList<T>, T?, or a projection DTO instead.");
        }
    }

    [Fact]
    public void IOrderRepository_returns_only_domain_types()
    {
        var repoType = typeof(IOrderRepository);

        var findMethod = repoType.GetMethod(nameof(IOrderRepository.FindByIdWithItemsAsync))!;
        Assert.Contains("Task", findMethod.ReturnType.Name);

        var getPlacedMethod = repoType.GetMethod(nameof(IOrderRepository.GetPlacedOrdersAsync))!;
        var returnType = getPlacedMethod.ReturnType;
        Assert.True(returnType.IsGenericType);
        var innerType = returnType.GetGenericArguments()[0];
        Assert.True(innerType.IsGenericType);
        Assert.Equal(typeof(IReadOnlyList<>), innerType.GetGenericTypeDefinition());
    }

    private static bool IsOrContainsQueryable(Type type)
    {
        if (type == typeof(IQueryable) ||
            (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>)))
            return true;

        if (type.IsGenericType)
        {
            foreach (var arg in type.GetGenericArguments())
            {
                if (IsOrContainsQueryable(arg))
                    return true;
            }
        }

        return false;
    }
}
