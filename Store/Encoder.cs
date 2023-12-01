namespace SteganographyNotepad.Store;

using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

using SteganographyApp.Common;
using SteganographyApp.Common.Arguments;
using SteganographyApp.Common.Data;
using SteganographyApp.Common.Injection;
using SteganographyApp.Common.IO;

/// <summary>
/// Utility class to asynchronously encode the text content to the cover images.
/// </summary>
internal static class Encoder
{
    /// <summary>
    /// Encodes the text content, represented by the data argument, and writes the encoded
    /// content to the cover images.
    /// </summary>
    /// <param name="data">The data to be encoded and written to the cover images.</param>
    /// <param name="arguments">The arguments to customize the behaviour or the encoder.</param>
    /// <returns>The void task to execute the encoding/writing process.</returns>
    public static Task EncodeDataToImagesAsync(string data, IInputArguments arguments) => Task.Run(() => EncodeData(data, arguments));

    private static void EncodeData(string data, IInputArguments arguments)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        string binaryData = Injector.Provide<IDataEncoderUtil>()
            .Encode(dataBytes, arguments.Password, arguments.UseCompression, arguments.DummyCount, arguments.RandomSeed, arguments.AdditionalPasswordHashIterations);

        int startingPixel = Calculator.CalculateRequiredBitsForContentTable(1);

        int written = -1;
        var store = new ImageStore(arguments);
        using (var wrapper = store.CreateIOWrapper())
        {
            wrapper.SeekToPixel(startingPixel);
            written = wrapper.WriteContentChunkToImage(binaryData);
            wrapper.EncodeComplete();
        }

        if (written == -1)
        {
            return;
        }
        using (var writer = new ChunkTableWriter(store, arguments))
        {
            writer.WriteContentChunkTable(ImmutableArray.Create(written));
        }
    }
}