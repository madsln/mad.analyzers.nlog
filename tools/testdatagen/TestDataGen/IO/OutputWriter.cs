using System.Text;

namespace TestDataGen.IO;

/// <summary>
/// Writes text files to disk using UTF-8 encoding (no BOM).
/// Creates parent directories automatically if they do not exist.
/// </summary>
internal class OutputWriter
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    /// <summary>
    /// Writes <paramref name="content"/> to <paramref name="filePath"/>.
    /// Intermediate directories are created as needed.
    /// </summary>
    public void Write(string filePath, string content)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(filePath, content, Utf8NoBom);
    }
}
