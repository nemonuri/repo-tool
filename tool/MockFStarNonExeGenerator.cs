#!/usr/bin/env dotnet

foreach (var a in args)
{
    Console.WriteLine(a);
}

if (args.Length != 1) 
{ 
    //Console.Error.WriteLine(string.Join(',')) 
}