namespace SteganographyNotepad.Store;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using SteganographyApp.Common;
using SteganographyApp.Common.Arguments;
using SteganographyApp.Common.Parsers;
using SteganographyNotepad.Models;

/// <summary>
/// Utility class to convert the <see cref="SettingsModel"/> to a string array
/// that mirrors how the CLI arguments would look if the SteganographyApp was
/// being executed from the command line.
/// </summary>
public static class Arguments
{
    /// <summary>
    /// Transforms the settings into a string array that replicates how the CLI
    /// arguments would look if the SteganographyApp would be run from the command line.
    /// </summary>
    /// <param name="settings">The settings to be converted into CLI arguments.</param>
    /// <returns>The CLI arguments in the form of a string array.</returns>
    public static string[] FormCliArguments(SettingsModel settings)
    {
        string imagePaths = string.Join(',', settings.CoverImages.Select(image => image.Path));
        var builder = new StringBuilder($"-c {imagePaths}");

        if (!string.IsNullOrEmpty(settings.Password))
        {
            builder.Append($" -p {settings.Password}");
        }
        if (!string.IsNullOrEmpty(settings.RandomSeed))
        {
            builder.Append($" -r {settings.RandomSeed}");
        }
        if (!string.IsNullOrEmpty(settings.AdditionalHashes))
        {
            builder.Append($" -a {settings.AdditionalHashes}");
        }
        if (!string.IsNullOrEmpty(settings.DummyCount))
        {
            builder.Append($" -d {settings.DummyCount}");
        }
        if (!string.IsNullOrEmpty(settings.IsCompressionEnabled))
        {
            builder.Append($" -co");
        }

        return builder.ToString().Split(" ");
    }
}

/// <summary>
/// The CLI parseable argument model.
/// </summary>
[SuppressMessage("Maintainability Rules", "SA1401", Justification = "Useless Rule")]
public sealed class StorageArguments : IArgumentConverter
{
    /// <summary>
    /// The set of cover images the data will be read from or written to.
    /// </summary>
    [Argument("--coverImages", "-c", true, parser: nameof(ParseImages))]
    public ImmutableArray<string> CoverImages = [];

    /// <summary>
    /// The optional password to encrypt the content being written to the cover images.
    /// </summary>
    [Argument("--password", "-p")]
    public string Password = string.Empty;

    /// <summary>
    /// The optional seed to be used in adding or randomizing the data being written
    /// to the cover images.
    /// </summary>
    [Argument("--randomSeed", "-r")]
    public string RandomSeed = string.Empty;

    /// <summary>
    /// The optional number of dummies to add to the data whne it is being written
    /// to the cover images.
    /// </summary>
    [Argument("--dummyCount", "-d")]
    public int DummyCount = 0;

    /// <summary>
    /// The additinoal number of times the encryption password will be hashed.
    /// </summary>
    [Argument("--additionalHashes", "-a")]
    public int AdditionalPasswordHashIterations = 0;

    /// <summary>
    /// Flag to indicating if data should be compressed/decompressed.
    /// </summary>
    [Argument("--compress", "-co")]
    public bool EnableCompression = false;

    /// <summary>
    /// Parses and returns list of images from the given CLI argument. This will fail
    /// if a path specified by the CLI argument points to a location that does not exist
    /// or if no images could be found based on the CLI argument.
    /// </summary>
    /// <param name="target">The IInputArguments instance the value returned by this method
    /// will be added to.</param>
    /// <param name="value">The raw CLI argument value to parse.</param>
    /// <returns>An array of paths pointing to the various images identified.</returns>
    public static object ParseImages(object target, string value) => ImagePathParser.ParseImages(value);

    /// <summary>
    /// Maps this model a mutable <see cref="IInputArguments"/> instance.
    /// </summary>
    /// <returns>A mutable IInputArguments instance containing all the public fields
    /// found on this model.</returns>
    public IInputArguments ToCommonArguments() => new CommonArguments
    {
        CoverImages = CoverImages,
        Password = Password,
        RandomSeed = RandomSeed,
        DummyCount = DummyCount,
        AdditionalPasswordHashIterations = AdditionalPasswordHashIterations,
        UseCompression = EnableCompression,
    };
}