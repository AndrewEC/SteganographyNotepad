namespace SteganographyNotepad.Store;

using System.IO;
using System.Text;
using System.Threading.Tasks;

using SteganographyApp.Common.Arguments;
using SteganographyApp.Common.Data;
using SteganographyApp.Common.IO;
using SteganographyApp.Common.Injection;

public static class Decoder
{
    public static Task<string> DecodeTextFromImagesAsync(IInputArguments arguments) => Task.Run(() => DecodeData(arguments));

    private static string DecodeData(IInputArguments arguments)
    {
        var store = new ImageStore(arguments);
        using (var chunkTableReader = new ChunkTableReader(store, arguments))
        {
            using (var wrapper = store.CreateIOWrapper())
            {
                var contentChunkTable = chunkTableReader.ReadContentChunkTable();

                using (var stream = new MemoryStream())
                {
                    foreach (int chunkLength in contentChunkTable)
                    {
                        string binary = wrapper.ReadContentChunkFromImage(chunkLength);
                        byte[] decoded = Injector.Provide<IDataEncoderUtil>()
                            .Decode(binary, arguments.Password, arguments.UseCompression, arguments.DummyCount, arguments.RandomSeed, arguments.AdditionalPasswordHashIterations);
                        stream.Write(decoded);
                    }
                    stream.Position = 0;
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }
    }
}