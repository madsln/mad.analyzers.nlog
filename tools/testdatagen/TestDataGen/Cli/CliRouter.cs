using TestDataGen.Cli.Commands;

namespace TestDataGen.Cli;

internal static class CliRouter
{
    public static int Run(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "help" or "-h")
        {
            HelpTexts.PrintGlobal();
            return args.Length == 0 ? 1 : 0;
        }

        string subcommand = args[0];
        string[] rest     = args[1..];

        return subcommand switch
        {
            "demo"    => DemoCommand.Run(rest),
            "test"    => TestCommand.Run(rest),
            "all"     => AllCommand.Run(rest),
            "migrate" => MigrateCommand.Run(rest),
            _         => UnknownSubcommand(subcommand)
        };
    }

    private static int UnknownSubcommand(string subcommand)
    {
        Console.Error.WriteLine($"Fehler: Unbekanntes Unterkommando '{subcommand}'.");
        Console.Error.WriteLine();
        HelpTexts.PrintGlobal();
        return 1;
    }
}
