using com.dxfeed.native.api;

namespace com.dxfeed.native
{
    public class NativeTools
    {
        public static void InitializeLogging(string fileName, bool overwrite, bool verbose)
        {
            C.CheckOk(C.Instance.dxf_initialize_logger(fileName, overwrite, true, verbose));
        }
    }
}
