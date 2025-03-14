namespace SteganographyNotepad.Store;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using SteganographyNotepad.Models;

/// <summary>
/// Utility class to convert the <see cref="SettingsModel"/> to a string array
/// that mirrors how the CLI arguments would look if the SteganographyApp was
/// being executed from the command line.
/// </summary>
public static class Arguments
{
    private static readonly ImmutableDictionary<Func<SettingsModel, string?>, string> Builders = new Dictionary<Func<SettingsModel, string?>, string>()
    {
        { model => string.Join(",", model.CoverImages.Select(image => image.Path)), "-c " },
        { model => model.Password, " -p " },
        { model => model.RandomSeed, " -r " },
        { model => model.AdditionalHashes, " -a " },
        { model => model.DummyCount, " -d " },
    }.ToImmutableDictionary();

    /// <summary>
    /// Transforms the settings into a string array that replicates how the CLI
    /// arguments would look if the SteganographyApp would be run from the command line.
    /// </summary>
    /// <param name="settings">The settings to be converted into CLI arguments.</param>
    /// <returns>The CLI arguments in the form of a string array.</returns>
    public static string[] FormCliArguments(SettingsModel settings)
    {
        var builder = new StringBuilder();

        foreach (var entry in Builders)
        {
            string? value = entry.Key.Invoke(settings);
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            builder.Append(entry.Value).Append(value);
        }

        if (!string.IsNullOrEmpty(settings.IsCompressionEnabled))
        {
            builder.Append($" -co");
        }

        return builder.ToString().Split(" ");
    }
}
