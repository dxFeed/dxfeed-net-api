#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    /// <summary>
    ///     Represents an indexed collection of up-to-date information about some
    ///     condition or state of an external entity that updates in real-time. For example,
    ///     <see cref="IDxOrder"/> represents an order to buy or to sell some market instrument
    ///     that is currently active on a market exchange and multiple
    ///     orders are active for each symbol at any given moment in time.
    ///     <see cref="IDxCandle"/> represent snapshots of aggregate information
    ///     about trading over a specific time period and there are multiple periods available.
    ///     The <see cref="IDxCandle"/> is also an example of <see cref="IDxTimeSeriesEvent"/> that
    ///     is a more specific event type.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///          Index for each event is available via <see cref="IDxIndexedEvent.Index"/> property.
    ///          Indexed events retain information about multiple events per symbol based on the
    ///          event index and are conflated based on the event index. Last indexed event for
    ///          each symbol and index is always delivered to event listeners on subscription, but
    ///          intermediate (next-to-last) events for each symbol+index pair are not queued
    ///          anywhere, they are simply discarded as stale events.
    ///          More recent events represent an up-to-date information about some external entity.
    ///     </para>
    ///     <para>
    ///          Event flags, transactions and snapshots.
    ///     </para>
    ///     <para>
    ///          Some indexed event sources provide a consistent view of a set of events for a
    ///          given symbol. Their updates may incorporate multiple changes that have to be
    ///          processed at the same time. The corresponding information is carried in
    ///          <see cref="IDxIndexedEvent.EventFlags"/> property.
    ///          Some indexed events, like <see cref="IDxOrder"/>, support multiple sources of
    ///          information for the same symbol. The event source is available via
    ///          <see cref="IDxIndexedEvent.Source"/> property.
    ///          The value of <see cref="IDxIndexedEvent.Source"/> property is always
    ///          <see cref="IndexedEventSource.DEFAULT"/> for time-series events and other
    ///          singe-sourced events like <see cref="IDxSeries"/>, that do not support this
    ///          feature.
    ///     </para>
    ///     <para>
    ///          The value of <see cref="IDxIndexedEvent.EventFlags"/> property has several
    ///          significant bits that are packed into an integer in the following way:
    ///     </para>
    ///     <para>31    ...          4    3    2    1    0</para>
    ///     <para>+----------------+----+----+----+----+----+</para>
    ///     <para>|                | SS | SE | SB | RE | TX |</para>
    ///     <para>+----------------+----+----+----+----+----+</para>
    ///     <para>
    ///          Each source updates its transactional state using these bits separately.
    ///          The state of each source has to be tracked separately in a map for each source.
    ///          However, event <see cref="IDxIndexedEvent.Index"/> is unique across the sources.
    ///          This is achieved by allocating an event-specific number of most significant bits
    ///          of <see cref="IDxIndexedEvent.Index"/> for use as a <see cref="IDxIndexedEvent.Source"/>
    ///          <see cref="IndexedEventSource.Id"/>.
    ///     </para>
    ///     <para>
    ///          <c>TX</c> (bit 0) - <see cref="EventFlag.TxPending"/> is an indicator of
    ///          pending transactional update. It can be retrieved from <c>eventFlags</c> with the
    ///          following piece of code:
    ///     </para>
    ///     <para>
    ///          <c>bool txPending = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.TxPending"/>) != 0;</c>
    ///     </para>
    ///     <para>
    ///          When <c>txPending</c> is <c>true</c> it means, that an ongoing transaction update
    ///          that spans multiple events is in process. All events with <c>txPending</c>
    ///          <c>true</c> shall be put into a separate pending list for each source id and
    ///          should be processed later when an event for this source id with <c>txPending</c>
    ///          <c>false</c> comes.
    ///     </para>
    ///     <para>
    ///          <c>RE</c> (bit 1) - <see cref="EventFlag.RemoveEvent"/> is used to indicate
    ///          that that the event with the corresponding index has to be removed.
    ///     </para>
    ///     <para>
    ///          <c>bool removeEvent = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.RemoveEvent"/>) != 0;</c>
    ///     </para>
    ///     <para>
    ///          <c>SB</c> (bit 2) - <see cref="EventFlag.SnapshotBegin"/> is used to
    ///          indicate when the loading of a snapshot starts.
    ///     </para>
    ///     <para>
    ///          <c>bool snapshotBegin = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.SnapshotBegin"/>) != 0;</c>
    ///     </para>
    ///     <para>
    ///          Snapshot load starts on new subscription and the first indexed event that arrives
    ///          for each non-zero source id on new subscription may have <c>snapshotBegin</c> set
    ///          to <c>true</c>. It means, that an ongoing snapshot consisting of multiple events
    ///          is incoming. All events for this source id shall be put into a separate pending
    ///          list for each source id.
    ///     </para>
    ///     <para>
    ///          <c>SE</c> (bit 3) - <see cref="EventFlag.SnapshotEnd"/> or
    ///          <c>SS</c> (bit 4) - <see cref="EventFlag.SnapshotSnip"/> are used to
    ///          indicate the end of a snapshot.
    ///     </para>
    ///     <para>
    ///          <c> bool snapshotEnd = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.SnapshotEnd"/>) != 0;</c>
    ///          <c> bool snapshotSnip = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.SnapshotSnip"/>) != 0;</c>
    ///     </para>
    ///     <para>
    ///          The last event of a snapshot is marked with either <c>snapshotEnd</c> or
    ///          <c>snapshotSnip</c>. At this time, all events from a pending list for the
    ///          corresponding source can be processed, unless <c>txPending</c> is also set to
    ///          <c>true</c>. In the later case, the processing shall be further delayed due to
    ///          ongoing transaction.
    ///     </para>
    ///     <para>
    ///          The difference between <c>snapshotEnd</c> and <c>snapshotSnip</c> is the
    ///          following. <c>snapshotEnd</c> indicates that the data source had sent all the
    ///          data pertaining to the subscription for the corresponding indexed event, while
    ///          <c>snapshotSnip</c> indicates that some limit on the amount of data was reached
    ///          and while there still might be more data available, it will not be provided.
    ///     </para>
    ///     <para>
    ///          When a snapshot is empty or consists of a single event, then the event can have
    ///          both <c>snapshotBegin</c> and <c>snapshotEnd</c> or <c>snapshotSnip</c> flags.
    ///          In case of an empty snapshot, <c>removeEvent</c> on this event is also set to
    ///          <c>true</c>.
    ///      </para>
    /// </remarks>
    public interface IDxIndexedEvent : IDxEventType
    {

        /// <summary>
        ///     Returns source of this event.
        /// </summary>
        /// <returns>Source of this event.</returns>
        IndexedEventSource Source { get; }

        /// <summary>
        ///    Gets or sets transactional event flags.
        ///    See "Event Flags" section from <see cref="IDxIndexedEvent"/>.
        /// </summary>
        EventFlag EventFlags { get; set; }

        /// <summary>
        ///     Gets unique per-symbol index of this event.
        /// </summary>
        long Index { get; }
    }

    /// <summary>
    ///     Represents an indexed collection of up-to-date information about some
    ///     condition or state of an external entity that updates in real-time. For example,
    ///     <see cref="IDxOrder"/> represents an order to buy or to sell some market instrument
    ///     that is currently active on a market exchange and multiple
    ///     orders are active for each symbol at any given moment in time.
    ///     <see cref="IDxCandle"/> represent snapshots of aggregate information
    ///     about trading over a specific time period and there are multiple periods available.
    ///     The <see cref="IDxCandle"/> is also an example of <see cref="IDxTimeSeriesEvent"/> that
    ///     is a more specific event type.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///          Index for each event is available via <see cref="IDxIndexedEvent.Index"/> property.
    ///          Indexed events retain information about multiple events per symbol based on the
    ///          event index and are conflated based on the event index. Last indexed event for
    ///          each symbol and index is always delivered to event listeners on subscription, but
    ///          intermediate (next-to-last) events for each symbol+index pair are not queued
    ///          anywhere, they are simply discarded as stale events.
    ///          More recent events represent an up-to-date information about some external entity.
    ///     </para>
    ///     <para>
    ///          Event flags, transactions and snapshots.
    ///     </para>
    ///     <para>
    ///          Some indexed event sources provide a consistent view of a set of events for a
    ///          given symbol. Their updates may incorporate multiple changes that have to be
    ///          processed at the same time. The corresponding information is carried in
    ///          <see cref="IDxIndexedEvent.EventFlags"/> property.
    ///          Some indexed events, like <see cref="IDxOrder"/>, support multiple sources of
    ///          information for the same symbol. The event source is available via
    ///          <see cref="IDxIndexedEvent.Source"/> property.
    ///          The value of <see cref="IDxIndexedEvent.Source"/> property is always
    ///          <see cref="IndexedEventSource.DEFAULT"/> for time-series events and other
    ///          singe-sourced events like <see cref="IDxSeries"/>, that do not support this
    ///          feature.
    ///     </para>
    ///     <para>
    ///          The value of <see cref="IDxIndexedEvent.EventFlags"/> property has several
    ///          significant bits that are packed into an integer in the following way:
    ///     </para>
    ///     <para>31    ...          4    3    2    1    0</para>
    ///     <para>+----------------+----+----+----+----+----+</para>
    ///     <para>|                | SS | SE | SB | RE | TX |</para>
    ///     <para>+----------------+----+----+----+----+----+</para>
    ///     <para>
    ///          Each source updates its transactional state using these bits separately.
    ///          The state of each source has to be tracked separately in a map for each source.
    ///          However, event <see cref="IDxIndexedEvent.Index"/> is unique across the sources.
    ///          This is achieved by allocating an event-specific number of most significant bits
    ///          of <see cref="IDxIndexedEvent.Index"/> for use as a <see cref="IDxIndexedEvent.Source"/>
    ///          <see cref="IndexedEventSource.Id"/>.
    ///     </para>
    ///     <para>
    ///          <c>TX</c> (bit 0) - <see cref="EventFlag.TxPending"/> is an indicator of
    ///          pending transactional update. It can be retrieved from <c>eventFlags</c> with the
    ///          following piece of code:
    ///     </para>
    ///     <para>
    ///          <c>bool txPending = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.TxPending"/>) != 0;</c>
    ///     </para>
    ///     <para>
    ///          When <c>txPending</c> is <c>true</c> it means, that an ongoing transaction update
    ///          that spans multiple events is in process. All events with <c>txPending</c>
    ///          <c>true</c> shall be put into a separate pending list for each source id and
    ///          should be processed later when an event for this source id with <c>txPending</c>
    ///          <c>false</c> comes.
    ///     </para>
    ///     <para>
    ///          <c>RE</c> (bit 1) - <see cref="EventFlag.RemoveEvent"/> is used to indicate
    ///          that that the event with the corresponding index has to be removed.
    ///     </para>
    ///     <para>
    ///          <c>bool removeEvent = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.RemoveEvent"/>) != 0;</c>
    ///     </para>
    ///     <para>
    ///          <c>SB</c> (bit 2) - <see cref="EventFlag.SnapshotBegin"/> is used to
    ///          indicate when the loading of a snapshot starts.
    ///     </para>
    ///     <para>
    ///          <c>bool snapshotBegin = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.SnapshotBegin"/>) != 0;</c>
    ///     </para>
    ///     <para>
    ///          Snapshot load starts on new subscription and the first indexed event that arrives
    ///          for each non-zero source id on new subscription may have <c>snapshotBegin</c> set
    ///          to <c>true</c>. It means, that an ongoing snapshot consisting of multiple events
    ///          is incoming. All events for this source id shall be put into a separate pending
    ///          list for each source id.
    ///     </para>
    ///     <para>
    ///          <c>SE</c> (bit 3) - <see cref="EventFlag.SnapshotEnd"/> or
    ///          <c>SS</c> (bit 4) - <see cref="EventFlag.SnapshotSnip"/> are used to
    ///          indicate the end of a snapshot.
    ///     </para>
    ///     <para>
    ///          <c> bool snapshotEnd = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.SnapshotEnd"/>) != 0;</c>
    ///          <c> bool snapshotSnip = (<see cref="IDxIndexedEvent.EventFlags"/> &amp; <see cref="EventFlag.SnapshotSnip"/>) != 0;</c>
    ///     </para>
    ///     <para>
    ///          The last event of a snapshot is marked with either <c>snapshotEnd</c> or
    ///          <c>snapshotSnip</c>. At this time, all events from a pending list for the
    ///          corresponding source can be processed, unless <c>txPending</c> is also set to
    ///          <c>true</c>. In the later case, the processing shall be further delayed due to
    ///          ongoing transaction.
    ///     </para>
    ///     <para>
    ///          The difference between <c>snapshotEnd</c> and <c>snapshotSnip</c> is the
    ///          following. <c>snapshotEnd</c> indicates that the data source had sent all the
    ///          data pertaining to the subscription for the corresponding indexed event, while
    ///          <c>snapshotSnip</c> indicates that some limit on the amount of data was reached
    ///          and while there still might be more data available, it will not be provided.
    ///     </para>
    ///     <para>
    ///          When a snapshot is empty or consists of a single event, then the event can have
    ///          both <c>snapshotBegin</c> and <c>snapshotEnd</c> or <c>snapshotSnip</c> flags.
    ///          In case of an empty snapshot, <c>removeEvent</c> on this event is also set to
    ///          <c>true</c>.
    ///      </para>
    /// </remarks>
    /// <typeparam name="T">Type of the event symbol for this event type.</typeparam>
    public interface IDxIndexedEvent<T> : IDxIndexedEvent, IDxEventType<T>
    {
        //Note: no extra fields
    }

}
