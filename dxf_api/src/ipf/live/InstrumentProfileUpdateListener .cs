using System.Collections.Generic;

namespace com.dxfeed.ipf.live {

    /// <summary>
    /// Notifies about instrument profile changes.
    /// </summary>
    public interface InstrumentProfileUpdateListener {

        //TODO: check comments on unused classes from Java

        /// <summary>
        /// This method is invoked when a set of instrument profiles in the underlying 
        /// InstrumentProfileCollector changes.Each instance of the listeners receive the same 
        /// instance of instruments iterator on every invocation of this method.The instruments
        /// iterator used right here or stored and accessed from a different thread.
        ///
        /// Removal of instrument profile is represented by an InstrumentProfile instance with a
        /// InstrumentProfile#getType() equal to InstrumentProfileType#REMOVED.
        /// </summary>
        /// <param name="instruments">collection that represents pending instrument profile updates.</param>
        void InstrumentProfilesUpdated(ICollection<InstrumentProfile> instruments);

    }
}
