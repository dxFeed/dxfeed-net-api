using com.dxfeed.api.events;

namespace com.dxfeed.api.plb
{
    /// <summary>
    /// The OnNewPLB listener interface
    /// </summary>
    public interface IDxOnNewPriceLevelBookHandler
    {
        /// <summary>
        /// On "PLB has been created|reset" event received
        /// </summary>
        /// <param name="book">The new PLB</param>
        void OnNewBook(DxPriceLevelBook book);
    }
}