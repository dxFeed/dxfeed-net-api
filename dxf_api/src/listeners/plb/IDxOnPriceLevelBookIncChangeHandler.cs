using com.dxfeed.api.events;

namespace com.dxfeed.api.plb
{
    /// <summary>
    /// The OnPLBIncrementalChange listener interface
    /// </summary>
    public interface IDxOnPriceLevelBookIncChangeHandler
    {
        /// <summary>
        /// On "PLB has been updated" event received
        /// </summary>
        /// <param name="removals">The price level removals</param>
        /// <param name="additions">The price level additions</param>
        /// <param name="updates">The price level updates</param>
        void OnBookIncrementalChange(DxPriceLevelBook removals, DxPriceLevelBook additions, DxPriceLevelBook updates);
    }
}