namespace IzhevskEventRadar.Contracts.Models;

public sealed class VkApiMethods
{
    public static readonly VkApiMethods WallGet = new("wall.get");

    public string Value { get; }

    private VkApiMethods(string value) => Value = value;
}
