using com.dxfeed.api.events;

namespace com.dxfeed.api.plb
{
    /// <summary>
    /// The OnPLBUpdate listener interface
    /// </summary>
    public interface IDxOnPriceLevelBookUpdateHandler
    {
        /// <summary>
        /// On "PLB has been updated" event received
        /// </summary>
        /// <param name="book">The updated PLB</param>
        void OnBookUpdate(DxPriceLevelBook book);
    }
}