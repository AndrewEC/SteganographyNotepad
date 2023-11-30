namespace SteganographyNotepad.Store;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using SteganographyApp.Common.Arguments;
using SteganographyNotepad.Models;

public static class Arguments
{
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

public sealed class StorageArguments : IArgumentConverter
{
    [Argument("--coverImages", "-c", true, parser: nameof(ParseImages))]
    public ImmutableArray<string> CoverImages = [];

    [Argument("--password", "-p")]
    public string Password = string.Empty;

    [Argument("--randomSeed", "-r")]
    public string RandomSeed = string.Empty;

    [Argument("--dummyCount", "-d")]
    public int DummyCount = 0;

    [Argument("--additionalHashes", "-a")]
    public int AdditionalPasswordHashIterations = 0;

    [Argument("--compress", "-co")]
    public bool EnableCompression = false;

    public static object ParseImages(object? target, string value) => ImagePathParser.ParseImages(value);

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