namespace Nemonuri.RepoTools.FStar.BuildTasks

open Microsoft.Build.Framework

module MSBuildTheory =

    let logError (task: Microsoft.Build.Utilities.Task) message valueExpr value =
        task.Log.LogError("{0}. {1} = {2}", message, valueExpr, value);
    
    let logErrorAndFalse task message valueExpr value =
        logError task message valueExpr value; false

    let logException (task: Microsoft.Build.Utilities.Task) (e: exn) =
        task.Log.LogErrorFromException e
#if DEBUG
        task.Log.LogError "Internal stack trace = "
#endif
        task.Log.LogError e.StackTrace
    
    let logExceptionAndFalse task e =
        logException task e; false
    
    let getItemSpec (item: ITaskItem) : string = item.ItemSpec

    let toTaskItem (itemSpec: string) : ITaskItem = Microsoft.Build.Utilities.TaskItem itemSpec