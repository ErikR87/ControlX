// See https://aka.ms/new-console-template for more information
using ControlX.Flow;
using ControlX.Flow.Core;
using ControlX.Flow.Core.Extensions;

// only for testing, implementation of CLI tbc

var automate = new Automate
{
    Actions = new IAction[]
        {
            new TestAction
            {
                Path = "$FileSystemEventArgs.FullPath"
            },
            new TestAction
            {
                Path = "Hallo Welt!"
            },
            new FTPAction(null)
            {
                Host = "127.0.0.1",
                Port = 22,
                Path = "/",
                UserName = "tester",
                Password = "password",
                SourceFile = "$FileSystemEventArgs.FullPath"
            }
        }
};

var json = automate.ToJson();

Console.WriteLine(json);

var json2 = File.ReadAllText("flow.json");
var automate2 = Automate.FromJson(json2);
Console.ReadKey();