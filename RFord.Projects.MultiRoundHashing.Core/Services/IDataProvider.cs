namespace RFord.Projects.MultiRoundHashing.Core.Services
{
    public interface IDataProvider
    {
        Stream GetStream(HashSource sourceType, string sourceData);
    }
}
