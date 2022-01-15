using System.Diagnostics;
using Perfy.Processes;

namespace Perfy.Misc;

public interface IWriter
{
    void WriteStart(Process process);
    void Write(Cache data);
    void WriteEnd(Cache data);
}
