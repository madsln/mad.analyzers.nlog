namespace TestDataGen.Cli.Commands;

internal static class AllCommand
{
    public static int Run(string[] args)
    {
        if (args.Contains("--help"))
        {
            HelpTexts.PrintAll();
            return 0;
        }

        string? demoOut = ParseArg(args, "--demo-out");
        if (demoOut is null)
        {
            Console.Error.WriteLine("Fehler: Parameter '--demo-out' ist erforderlich.");
            Console.Error.WriteLine();
            HelpTexts.PrintAll();
            return 1;
        }

        // --- demo ---
        var demoArgs = new List<string> { "--out", demoOut };
        int demoCode = DemoCommand.Run([.. demoArgs]);
        if (demoCode != 0) return demoCode;

        // --- test ---
        string? testOut = ParseArg(args, "--test-out");
        var testArgs = new List<string> { "--source", demoOut };
        if (testOut is not null)
        {
            testArgs.Add("--out");
            testArgs.Add(testOut);
        }

        return TestCommand.Run([.. testArgs]);
    }

    private static string? ParseArg(string[] args, string key)
    {
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == key) return args[i + 1];
        return null;
    }
}
