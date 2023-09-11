namespace Temperature.Receiver.Dto;
public record Message
{
    public string Time { get; init; }
    public IEnumerable<Row>? Rows { get; init; }
}