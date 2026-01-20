
using System.Collections;

namespace Nemonuri.RepoTools.TestRuntime;

public class MockTaskItem : ITaskItem
{
    public string GetMetadata(string metadataName)
    {
        throw new NotImplementedException();
    }

    public void SetMetadata(string metadataName, string metadataValue)
    {
        throw new NotImplementedException();
    }

    public void RemoveMetadata(string metadataName)
    {
        throw new NotImplementedException();
    }

    public void CopyMetadataTo(ITaskItem destinationItem)
    {
        throw new NotImplementedException();
    }

    public IDictionary CloneCustomMetadata()
    {
        throw new NotImplementedException();
    }

    public string ItemSpec { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ICollection MetadataNames => throw new NotImplementedException();

    public int MetadataCount => throw new NotImplementedException();
}
