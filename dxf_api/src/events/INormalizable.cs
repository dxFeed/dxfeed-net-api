using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace com.dxfeed.api.events
{
    public interface INormalizable<out T>
    {
        T Normalized();
    }
}