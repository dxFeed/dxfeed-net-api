namespace com.dxfeed.api.events {
    /// <summary>
    /// Interface class for all market events. All market events are plain objects that
    /// extend this class. Market event classes are simple beans with setter and getter methods for their
    /// properties and minimal business logic.
    /// </summary>
    public interface IDxMarketEvent {

        /// <summary>
        /// Returns symbol of this event.
        /// </summary>
        string Symbol { get; }

    }
}
