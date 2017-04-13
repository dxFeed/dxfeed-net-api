using com.dxfeed.api.events;

namespace com.dxfeed.tests.tools.eventplayer
{
    /// <summary>
    ///     Stores data for one incoming event.
    /// </summary>
    public interface IPlayedEvent
    {
        EventParams Params { get; }
        object Data { get; }
    }

    /// <summary>
    ///     Stores data for one incoming event.
    /// </summary>
    /// <typeparam name="NE">Native event data, e.g. <see cref="DxTestOrder"/></typeparam>
    public interface IPlayedEvent<NE> : IPlayedEvent
    {
        new NE Data { get; }
    }
}
