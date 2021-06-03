namespace Castaway.Assets
{
    public interface IAssetType
    {
        T To<T>(Asset a);
    }
}