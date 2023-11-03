using ProtoBuf;

namespace Temperature.Receiver.Model;

[ProtoContract]
public record WeatherInformation
{
    [ProtoMember(1)]
    public float Temperature { get; set; }

    [ProtoMember(2)]
    public byte Humidity { get; set; }
}