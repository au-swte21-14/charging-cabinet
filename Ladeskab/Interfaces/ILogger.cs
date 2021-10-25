#nullable enable
namespace Ladeskab.Interfaces
{
    public interface ILogger
    {
        public void WriteLine(string format, params object?[] arg);
    }
}