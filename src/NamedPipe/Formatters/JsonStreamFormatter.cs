using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kan1.NamedPipe.Formatters
{
    public class JsonStreamFormatter : IStreamFormatter
    {
        private readonly Encoding encoding;
        private readonly bool leaveOpen;

        public JsonStreamFormatter(Encoding encoding, bool leaveOpen = true)
        {
            this.encoding = encoding;
            this.leaveOpen = leaveOpen;
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            using (var writer = new BinaryWriter(stream, encoding, leaveOpen))
            {
                writer.Write(JsonConvert.SerializeObject(obj));
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var reader = new BinaryReader(stream, encoding, leaveOpen))
            {
                return JsonConvert.DeserializeObject<T>(reader.ReadString());
            }
        }
    }
}
