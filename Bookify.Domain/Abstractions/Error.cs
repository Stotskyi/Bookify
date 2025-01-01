namespace Bookify.Domain.Abstractions;

public record Error(string code,string name)
{
    public static Error None = new(string.Empty, string.Empty);
    
    public static Error NullValue = new("Error.NullValue","Null value was provided");
}
