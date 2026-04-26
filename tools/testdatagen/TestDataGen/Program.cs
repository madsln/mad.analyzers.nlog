using TestDataGen.Cli;

try
{
    return CliRouter.Run(args);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Fehler: {ex.Message}");
    return 3;
}
