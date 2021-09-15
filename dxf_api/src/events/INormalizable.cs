using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// An interface that describes entities (events) that can be converted to normal form.
    /// Example: <c>TimeAndSale{symbol = AAPL&amp;Q, scope = Composite} -> TimeAndSale&amp;Q{symbol = AAPL, scope = Regional}</c> 
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface INormalizable<out T>
    {
        /// <summary>
        /// Converts the entity to normalized form
        /// </summary>
        /// <returns>The copy of the entity in the normalized form or current entity</returns>
        T Normalized();
    }
}