namespace DotNetFive.Core.Caching
{
    public interface IConverter<T>
    {
        string Serialize(object obj);

        T Deserialize(string value);
    }
}