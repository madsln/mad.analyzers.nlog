using TestDataGen.Config;
using TestDataGen.IO;

namespace TestDataGen.Demo;

/// <summary>
/// Orchestrates the demo-file generation pipeline.
/// Iterates over all <c>LoggerTypes × CallTemplates × TestCases</c> combinations,
/// builds <see cref="DemoVariant"/>s via <see cref="VariantBuilder"/>, assembles file
/// content via <see cref="CodeFileBuilder"/>, and writes files via <see cref="OutputWriter"/>.
/// </summary>
internal class DemoGenerator(OutputWriter writer)
{
    /// <summary>
    /// Generates all demo files into <paramref name="outDir"/>.
    /// </summary>
    /// <param name="outDir">Root output directory. Subdirectory per logger type is created automatically.</param>
    /// <returns>Number of files written.</returns>
    public int Generate(string outDir)
    {
        int count = 0;

        foreach (var loggerType in LoggerConfig.LoggerTypes)
        {
            var templates = CallTemplateConfig.Templates
                .Where(t => t.LoggerType == loggerType);

            foreach (var template in templates)
            {
                foreach (var testCase in TestCaseConfig.TestCases)
                {
                    foreach (var variant in VariantBuilder.Build(loggerType, template, testCase))
                    {
                        var content = CodeFileBuilder.Build(variant);
                        var filePath = Path.Combine(outDir, "diagnostics", "MAD2017", loggerType.ToLower(), $"{variant.ClassName}.cs");
                        writer.Write(filePath, content);
                        count++;
                    }
                }
            }
        }

        return count;
    }
}
