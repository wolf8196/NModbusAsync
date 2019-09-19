namespace NModbusAsync
{
    public interface IStreamResource<TResource> : IStreamResource
    {
        TResource UnderlyingResource { get; }
    }
}