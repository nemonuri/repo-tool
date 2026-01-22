
using System.Collections;

namespace Nemonuri.RepoTools.TestRuntime;

public class MockTaskItem : ITaskItem
{
    public string ItemSpec { get; set; }

    private readonly Dictionary<string, string> _metadatas;

    public MockTaskItem(string itemSpec, Dictionary<string, string>? metadatas = null)
    {
        ItemSpec = itemSpec;
        _metadatas = metadatas is null ? new() : new(metadatas);
    }

    public string GetMetadata(string metadataName) => _metadatas[metadataName];

    public void SetMetadata(string metadataName, string metadataValue)
    {
        if (_metadatas.ContainsKey(metadataName))
        {
            _metadatas[metadataName] = metadataValue;
        }
        else
        {
            _metadatas.Add(metadataName, metadataValue);
        }
    }

    public void RemoveMetadata(string metadataName) => _metadatas.Remove(metadataName);

    public void CopyMetadataTo(ITaskItem destinationItem)
    {
        foreach (var kv in _metadatas)
        {
            destinationItem.SetMetadata(kv.Key, kv.Value);
        }
    }

    public IDictionary CloneCustomMetadata()
    {
        return new Hashtable(_metadatas);
    }


    public ICollection MetadataNames => _metadatas.Keys;

    public int MetadataCount => _metadatas.Count;
}
