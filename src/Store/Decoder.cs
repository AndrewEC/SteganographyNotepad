namespace SteganographyNotepad.Store;

using System.IO;
using System.Text;
using System.Threading.Tasks;

using SteganographyApp.Common;
using SteganographyApp.Common.Data;
using SteganographyApp.Common.Injection;
using SteganographyApp.Common.IO;
using SteganographyApp.Common.IO.Content;

/// <summary>
/// Utility class to asynchronously decode text content from the cover images.
/// </summary>
internal static class Decoder
{
    /// <summary>
    /// Asynchronously decodes the text content from the cover images, specified within the arguments, and returns
    /// the UTF-8 result.
    /// </summary>
    /// <param name="arguments">The arguments used by the <see cref="ImageStore"/> to decode data from the
    /// cover images.</param>
    /// <returns>A UTF-8 encoded string read from the cover images.</returns>
    public static Task<string> DecodeTextFromImagesAsync(IInputArguments arguments)
        => Task.Run(() => DecodeData(arguments));

    private static string DecodeData(IInputArguments arguments)
    {
        var store = ServiceContainer.CreateImageStore(arguments);
        using (var storeStream = store.OpenStream(StreamMode.Read))
        {
            var contentChunkTable = ChunkTableReader.ReadContentChunkTable(storeStream);

            using (var memoryStream = new MemoryStream())
            {
                foreach (int chunkLength in contentChunkTable)
                {
                    string binary = storeStream.ReadContentChunkFromImage(chunkLength);
                    byte[] decoded = ServiceContainer.GetService<IDataEncoderUtil>()
                        .Decode(binary, arguments.Password, arguments.UseCompression, arguments.DummyCount, arguments.RandomSeed, arguments.AdditionalPasswordHashIterations);
                    memoryStream.Write(decoded);
                }

                memoryStream.Position = 0;
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}