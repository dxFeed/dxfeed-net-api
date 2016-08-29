using System.Collections.Generic;

namespace com.dxfeed.ipf.live {

    /// <summary>
    /// This class tracks changes in instrument profile snapshot and filter
    /// bulk data. It is possible while live streaming connection was broken.
    /// Reconnection accompanied with repeated receiving of whole snapshot.
    /// This class allow to send user only changed data, i.e without duplicates.
    /// </summary>
    class InstrumentProfileUpdater {

        private object updaterLocker = new object();
        private Dictionary<int, InstrumentProfile> dictionaryByKey = null;
        private List<InstrumentProfile> buffer = null;

        /// <summary>
        /// Get full instrument profiles collection.
        /// </summary>
        public ICollection<InstrumentProfile> InstrumentProfiles {
            get {
                lock (updaterLocker) {
                    return buffer;
                }
            }
        }

        /// <summary>
        /// Update buffered collection and returns only changed data.
        /// </summary>
        /// <param name="instrumentProfiles">Updatet data.</param>
        /// <returns>Changed data.</returns>
        public ICollection<InstrumentProfile> Update(IList<InstrumentProfile> instrumentProfiles) {
            lock(updaterLocker) {
                if (buffer == null) {
                    buffer = new List<InstrumentProfile>(instrumentProfiles.Count);
                    dictionaryByKey = new Dictionary<int, InstrumentProfile>(instrumentProfiles.Count);
                    foreach (InstrumentProfile ip in instrumentProfiles) {
                        buffer.Add(ip);
                        dictionaryByKey[GetInstrumentProfileKey(ip)] = ip;
                    }
                    return buffer;
                } else {
                    List<InstrumentProfile> updateList = new List<InstrumentProfile>();
                    foreach (InstrumentProfile ip in instrumentProfiles) {
                        int ipKey = GetInstrumentProfileKey(ip);
                        if (dictionaryByKey.ContainsKey(ipKey)) {
                            if (ip.GetTypeName() == InstrumentProfileType.REMOVED.Name) {
                                //Remove instrument profile
                                buffer.Remove(dictionaryByKey[ipKey]);
                                dictionaryByKey.Remove(ipKey);
                                updateList.Add(ip);
                            } else if (!dictionaryByKey[ipKey].Equals(ip)) {
                                //Update instrument profile
                                int pos = buffer.IndexOf(dictionaryByKey[ipKey]);
                                buffer[pos] = ip;
                                dictionaryByKey[ipKey] = ip;
                                updateList.Add(ip);
                            }
                        } else {
                            //Add new instrument profile
                            buffer.Add(ip);
                            dictionaryByKey[ipKey] = ip;
                            updateList.Add(ip);
                        }
                    }
                    return updateList;
                }
            }
        }

        /// <summary>
        /// Make a instrument profile key for hashing inside this class.
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private int GetInstrumentProfileKey(InstrumentProfile ip) {
            return (ip.GetType() + ip.GetSymbol()).GetHashCode();
        }

    }
}
