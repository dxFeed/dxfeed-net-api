using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.dxfeed.ipf.live {
    class InstrumentProfileUpdater {

        private object updaterLocker = new object();
        private Dictionary<int, InstrumentProfile> dictionaryByKey = null;
        private List<InstrumentProfile> buffer = null;

        public ICollection<InstrumentProfile> InstrumentProfiles {
            get {
                lock (updaterLocker) {
                    return buffer;
                }
            }
        }

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

        private int GetInstrumentProfileKey(InstrumentProfile ip) {
            return (ip.GetType() + ip.GetSymbol()).GetHashCode();
        }

    }
}
