using System.Linq.Expressions;
using System.Reflection;

namespace DynamicLinq;

public static class LinqExtensions
{
    public static IQueryable<T> DynamicWhere<T>(this IQueryable<T> query, Condition condition)
    {
        return query.DynamicWhere(condition.Property, condition.Operator, condition.Value);
    }

    public static IQueryable<T> DynamicWhere<T>(this IQueryable<T> query, string propertyName, string operation, object value)
    {
        var type = typeof(T);
        var target = type.GetProperty(propertyName);
        if (target is null) throw new Exception($"Property does not exist: {propertyName}");
        else if (value.GetType() != target.PropertyType)
        {
            try
            {
                value = Convert.ChangeType(value, target.PropertyType);
            }
            catch (Exception)
            {
                throw new Exception($"Invalid value for property: {propertyName} - {value}");
            }
        }
        var expression = CreateExpression<T>(type, propertyName, operation, value);
        return query.Where(expression);
    }

    private static Func<MemberExpression, ConstantExpression, Expression> GetOperation(string operation) => operation.ToLower() switch
    {
        "is" => CreateEqual,
        "not" => CreateNotEqual,
        "greater" => CreateGreaterThan,
        "lesser" => CreateLessThan,
        "contains" => CreateContains,
        "without" => CreateWithout,
        _ => throw new Exception($"Invalid operation: {operation}")
    };

    public static Expression<Func<T, bool>> CreateExpression<T>(Type type, string propertyName, string operation, object val)
    {
        var param = Expression.Parameter(type);
        var property = Expression.Property(param, propertyName);
        var value = Expression.Constant(val);
        var res = GetOperation(operation)(property, value);
        return Expression.Lambda<Func<T, bool>>(res, param);
    }

    private static Expression CreateEqual(MemberExpression member, ConstantExpression constant)
    {
        return Expression.Equal(member, constant);
    }
    private static Expression CreateNotEqual(MemberExpression member, ConstantExpression constant)
    {
        return Expression.NotEqual(member, constant);
    }
    private static Expression CreateGreaterThan(MemberExpression member, ConstantExpression constant)
    {
        return Expression.GreaterThan(member, constant);
    }
    private static Expression CreateLessThan(MemberExpression member, ConstantExpression constant)
    {
        return Expression.LessThan(member, constant);
    }
    private static MethodInfo Contains = typeof(string).GetMethod("Contains", new Type[] { typeof(string), typeof(StringComparison) })!;
    private static Expression CreateContains(MemberExpression member, ConstantExpression constant)
    {
        return Expression.Call(member, Contains, constant, Expression.Constant(StringComparison.OrdinalIgnoreCase));
    }
    private static Expression CreateWithout(MemberExpression member, ConstantExpression constant)
    {
        return Expression.Not(Expression.Call(member, Contains, constant, Expression.Constant(StringComparison.OrdinalIgnoreCase)));
    }
}