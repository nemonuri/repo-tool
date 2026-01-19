
using Nemonuri.RepoTools.TestRuntime.Extensions;

namespace Nemonuri.RepoTools.TestRuntime;

public static class MockMSBuildTheory
{
    public static ITaskItem CreateTaskItem(string itemSpec, IDictionary<string, string>? metaDatas = null)
    {
        Dictionary<string, string> clonedMdTable = metaDatas is { } v ? new (v) : new();

        // https://github.com/devlooped/moq/wiki/Quickstart
        var taskItem = new Mock<ITaskItem>(MockBehavior.Strict);
        taskItem.Setup(x => x.ItemSpec).Returns(itemSpec);
        taskItem.Setup(x => x.GetMetadata(It.IsAny<string>()))
                .Returns((string mdName) => (clonedMdTable.TryGetValue(mdName, out var mdValue) && mdValue is { } v) ? v : "");
        taskItem.Setup(x => x.SetMetadata(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>(clonedMdTable.AddOrUpdate);
        return taskItem.Object;
    }
}

