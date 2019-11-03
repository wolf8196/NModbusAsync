namespace NModbusAsync
{
    public interface IPipeResource<out TResource> : IPipeResource
    {
        TResource Resource { get; }
    }
}