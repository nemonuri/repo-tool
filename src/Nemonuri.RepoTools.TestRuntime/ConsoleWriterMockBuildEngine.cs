
using System.Collections;

namespace Nemonuri.RepoTools.TestRuntime;

public class ConsoleWriterMockBuildEngine() : IBuildEngine
{
    public List<BuildErrorEventArgs> ErrorEvents {get;} = new();
    public List<BuildWarningEventArgs> WarningEvents {get;} = new();
    public List<BuildMessageEventArgs> MessageEvents {get;} = new();
    public List<CustomBuildEventArgs> CustomEvents {get;} = new();

    public void LogErrorEvent(BuildErrorEventArgs e)
    {
        Console.WriteLine(e.Message);
        ErrorEvents.Add(e);
    }

    public void LogWarningEvent(BuildWarningEventArgs e)
    {
        Console.WriteLine(e.Message);
        WarningEvents.Add(e);
    }

    public void LogMessageEvent(BuildMessageEventArgs e)
    {
        Console.WriteLine(e.Message);
        MessageEvents.Add(e);
    }

    public void LogCustomEvent(CustomBuildEventArgs e)
    {
        Console.WriteLine(e.Message);
        CustomEvents.Add(e);
    }

    public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
    {
        throw new NotSupportedException();
    }

    public bool ContinueOnError => false;

    public int LineNumberOfTaskNode => 0;

    public int ColumnNumberOfTaskNode => 0;

    public string ProjectFileOfTaskNode => "";
}