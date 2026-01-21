// Copied from:
//   - https://github.com/dotnet/msbuild/blob/main/src/Build/Evaluation/IntrinsicFunctions.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Nemonuri.RepoTools.TestRuntime;

public static class MSBuildIntrinsicFunctions
{
    /// <summary>
    /// Gets the canonicalized full path of the provided path and ensures it contains the correct directory separator characters for the current operating system.
    /// </summary>
    /// <param name="path">One or more paths to combine and normalize.</param>
    /// <returns>A canonicalized full path with the correct directory separators.</returns>
    public static string NormalizePath(params string[] path)
    {
        return MSBuildFileUtilities.NormalizePath(path);
    }
}
