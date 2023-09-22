namespace DynamicLinq;

public class QueryParser
{
    public static List<Condition> Parse(string query)
    {
        var parts = query?.Split('|') ?? new string[0];
        return parts.Select(x => x.Trim()).Select(x => new Condition(x)).ToList();
    }
}

public class Condition
{
    public Condition(string condition)
    {
        var parts = condition.Split(' ');
        if (parts.Length != 3) throw new Exception($"Invalid condition: {condition}");
        Property = parts[0];
        Operator = parts[1];
        Value = parts[2];
    }

    public string Property { get; set; }
    public string Operator { get; set; }
    public string Value { get; set; }
}
