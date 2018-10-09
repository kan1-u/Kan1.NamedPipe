using System.IO;
using System.Threading.Tasks;

namespace Kan1.NamedPipe
{
    public interface IStreamFormatter
    {
        void Serialize<T>(Stream stream, T obj);
        T Deserialize<T>(Stream stream);
    }
}
