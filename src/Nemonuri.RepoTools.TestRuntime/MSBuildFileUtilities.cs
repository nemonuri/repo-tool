// Copied from:
//   - https://github.com/dotnet/msbuild/blob/main/src/Shared/FileUtilities.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CommunityToolkit.Diagnostics;

namespace Nemonuri.RepoTools.TestRuntime;

public static class MSBuildFileUtilities
{
    /// <summary>
    /// Gets the canonicalized full path of the provided path.
    /// Guidance for use: call this on all paths accepted through public entry
    /// points that need normalization. After that point, only verify the path
    /// is rooted, using ErrorUtilities.VerifyThrowPathRooted.
    /// ASSUMES INPUT IS ALREADY UNESCAPED.
    /// </summary>
    public static string NormalizePath(string path)
    {
        Guard.IsNotNullOrWhiteSpace(path);
        string fullPath = GetFullPath(path);
        return FixFilePath(fullPath);
    }

    public static string NormalizePath(params string[] paths)
    {
        return NormalizePath(Path.Combine(paths));
    }

    private static string GetFullPath(string path)
    {
        // TODO: FEATURE_LEGACY_GETFULLPATH
        return Path.GetFullPath(path);
    }

    public static string FixFilePath(string path)
    {
        return string.IsNullOrEmpty(path) || Path.DirectorySeparatorChar == '\\' ? path : path.Replace('\\', '/'); // .Replace("//", "/");
    }
}