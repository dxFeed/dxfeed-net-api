using com.dxfeed.native.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.dxfeed.native {
    public class NativeTools {

        public static void InitializeLogging(string fileName, bool overwrite, bool verbose) {
            C.CheckOk(C.Instance.dxf_initialize_logger(fileName, overwrite, true, verbose));
        }

        public static Int64 DateToUnixTime(DateTime dateTime) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = (DateTime)dateTime - origin;
            return Convert.ToInt64(Math.Floor(diff.TotalMilliseconds));
        }

    }
}
