using TestDataGen.IO;
using TestDataGen.Test;

namespace TestDataGen.Cli.Commands;

internal static class TestCommand
{
    public static int Run(string[] args)
    {
        if (args.Contains("--help"))
        {
            HelpTexts.PrintTest();
            return 0;
        }

        string? sourcePath = ParseArg(args, "--source");
        if (sourcePath is null)
        {
            Console.Error.WriteLine("Fehler: Parameter '--source' ist erforderlich.");
            Console.Error.WriteLine();
            HelpTexts.PrintTest();
            return 1;
        }

        if (!Directory.Exists(sourcePath))
        {
            Console.Error.WriteLine(
                $"Fehler: Quellverzeichnis '{sourcePath}' existiert nicht.");
            Console.Error.WriteLine("Hinweis: Führen Sie zuerst 'TestDataGen demo --out <Pfad>' aus.");
            return 2;
        }

        if (!Directory.EnumerateFiles(sourcePath, "*.cs", SearchOption.AllDirectories).Any())
        {
            Console.Error.WriteLine(
                $"Fehler: Quellverzeichnis '{sourcePath}' enthält keine .cs-Dateien.");
            Console.Error.WriteLine("Hinweis: Führen Sie zuerst 'TestDataGen demo --out <Pfad>' aus.");
            return 2;
        }

        string? outPath = ParseArg(args, "--out");
        if (outPath is null)
        {
            outPath = Path.GetFullPath(Path.Combine(
                sourcePath, "..", "..", "test",
                "mad.analyzers.nlog.Test", "generated"));
        }

        var writer   = new OutputWriter();
        var compiler = new TestCompiler(writer);
        int n        = compiler.Compile(sourcePath, outPath);
        Console.WriteLine($"test: {n} Testklassen-Dateien geschrieben nach {Path.GetFullPath(outPath)}");
        return 0;
    }

    private static string? ParseArg(string[] args, string key)
    {
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == key) return args[i + 1];
        return null;
    }
}
