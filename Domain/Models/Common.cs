namespace S_B_MicroService.Models;


public record Person
{
    public string? FirstName { get; init; }
    public string? Surname { get; init; }
}


public record Address
{
    public string? Street { get; init; }
    public string? Town { get; init; }
    public string? Country { get; init; }
}