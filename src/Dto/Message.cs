namespace Temperature.Receiver.Dto;
public record Message
{
    public string Time { get; init; } = default!;
    public IEnumerable<Row>? Rows { get; init; }
}