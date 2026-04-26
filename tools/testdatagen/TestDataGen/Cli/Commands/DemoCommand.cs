using TestDataGen.Demo;
using TestDataGen.IO;

namespace TestDataGen.Cli.Commands;

internal static class DemoCommand
{
    public static int Run(string[] args)
    {
        if (args.Contains("--help"))
        {
            HelpTexts.PrintDemo();
            return 0;
        }

        string? outPath = ParseArg(args, "--out");
        if (outPath is null)
        {
            Console.Error.WriteLine("Fehler: Parameter '--out' ist erforderlich.");
            Console.Error.WriteLine();
            HelpTexts.PrintDemo();
            return 1;
        }

        var writer = new OutputWriter();
        var gen    = new DemoGenerator(writer);
        int n      = gen.Generate(outPath);
        Console.WriteLine($"demo: {n} Dateien generiert in {Path.GetFullPath(outPath)}");
        return 0;
    }

    private static string? ParseArg(string[] args, string key)
    {
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == key) return args[i + 1];
        return null;
    }
}
