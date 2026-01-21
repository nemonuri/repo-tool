#!/usr/bin/env dotnet

if (args.Length < 1 || string.IsNullOrWhiteSpace(args[0])) 
{ 
    Console.Error.WriteLine("First argument should be directory path.");
    return 1;
}

var dirFullPath = Path.GetFullPath(args[0]);
if (!Directory.Exists(dirFullPath)) { Directory.CreateDirectory(dirFullPath); }


List<string> toStdOuts = new();

// directory.fstar.exe
toStdOuts.Add(Directory.CreateDirectory(Path.Combine(dirFullPath, "directory.fstar.exe")).FullName );

// fstar.txt
{
    string txtPath = Path.Combine(dirFullPath, "fstar.txt");
    File.WriteAllText(txtPath, "");
    toStdOuts.Add(txtPath);
}

foreach (var item in toStdOuts)
{
    Console.WriteLine(item);
}

return 0;