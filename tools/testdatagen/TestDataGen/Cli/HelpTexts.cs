namespace TestDataGen.Cli;

internal static class HelpTexts
{
    public static void PrintGlobal()
    {
        Console.WriteLine("""
            TestDataGen – Testdaten-Generator für NLog-Analyzer MAD2017

            Verwendung:
              TestDataGen <Unterkommando> [Parameter]

            Unterkommandos:
              demo     Generiert C#-Demo-Quelldateien (Datengrundlage)
              test     Generiert MSTest-Unittest-Dateien aus Demo-Dateien
              all      Führt demo und test in einem Schritt aus
              migrate  Migriert Demo-Dateien auf das line:column:length-Format (ADR-012)

            Parameter:
              --help  Zeigt diese Hilfe an

            Hilfe zu einem Unterkommando:
              TestDataGen demo --help
              TestDataGen test --help
              TestDataGen all  --help

            Beispiele:
              TestDataGen demo --out ./tmp/demo
              TestDataGen test --source ./tmp/demo --out ./tmp/tests
              TestDataGen all  --demo-out ./tmp/demo
            """);
    }

    public static void PrintDemo()
    {
        Console.WriteLine("""
            TestDataGen demo – Generiert C#-Demo-Quelldateien

            Verwendung:
              TestDataGen demo --out <Pfad>

            Parameter:
              --out <Pfad>   (Pflicht) Ausgabeverzeichnis für die generierten Demo-C#-Dateien.
                             Wird automatisch erstellt, falls nicht vorhanden.

            Beispiel:
              TestDataGen demo --out ./tmp/demo
            """);
    }

    public static void PrintTest()
    {
        Console.WriteLine("""
            TestDataGen test – Generiert MSTest-Unittest-Dateien

            Verwendung:
              TestDataGen test --source <Pfad> [--out <Pfad>]

            Parameter:
              --source <Pfad>  (Pflicht) Verzeichnis mit den Demo-Quelldateien
                               (Ausgabe von 'demo'). Muss .cs-Dateien enthalten.
              --out <Pfad>     (Optional) Ausgabeverzeichnis für die Unittest-Dateien.
                               Standard: ../../test/mad.analyzers.nlog.Test/generated
                               relativ zu --source.

            Beispiele:
              TestDataGen test --source ./tmp/demo
              TestDataGen test --source ./tmp/demo --out ./tmp/tests
            """);
    }

    public static void PrintMigrate()
    {
        Console.WriteLine("""
            TestDataGen migrate – Migriert Demo-Dateien auf das line:column:length-Format

            Verwendung:
              TestDataGen migrate --source <Pfad>

            Parameter:
              --source <Pfad>  (Pflicht) Verzeichnis mit den Demo-Quelldateien.
                               Alle .cs-Dateien werden rekursiv migriert.

            Beispiel:
              TestDataGen migrate --source ./src/demo/mad.analyzers.nlog.demo
            """);
    }

    public static void PrintAll()
    {
        Console.WriteLine("""
            TestDataGen all – Generiert Demo- und Unittest-Dateien in einem Schritt

            Verwendung:
              TestDataGen all --demo-out <Pfad> [--test-out <Pfad>]

            Parameter:
              --demo-out <Pfad>  (Pflicht) Ausgabeverzeichnis für die Demo-Dateien.
              --test-out <Pfad>  (Optional) Ausgabeverzeichnis für die Unittest-Dateien.
                                 Standard: wie bei 'test' (relativ zu --demo-out).

            Beispiel:
              TestDataGen all --demo-out ./tmp/demo
              TestDataGen all --demo-out ./tmp/demo --test-out ./tmp/tests
            """);
    }
}
