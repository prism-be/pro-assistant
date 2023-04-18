namespace Prism.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PartitionAttribute : Attribute
{
    public PartitionAttribute(string partition)
    {
        Partition = partition;
    }

    public string Partition { get; }

    public static string GetPartition<T>()
    {
        var name = (typeof(T).GetCustomAttributes(typeof(PartitionAttribute), true)
                .FirstOrDefault()
            as PartitionAttribute)?.Partition;

        if (string.IsNullOrWhiteSpace(name))
        {
            return "Id";
        }

        return name;
    }
}