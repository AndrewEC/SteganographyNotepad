namespace SteganographyNotepad.Store;

using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

using SteganographyApp.Common;
using SteganographyApp.Common.Arguments;
using SteganographyApp.Common.Data;
using SteganographyApp.Common.Injection;
using SteganographyApp.Common.IO;

internal static class Encoder
{
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