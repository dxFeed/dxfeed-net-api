#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.native.api;

namespace com.dxfeed.native
{
    /// <summary>
    /// The native API utility functions
    /// </summary>
    public static class NativeTools
    {
        /// <summary>
        /// Initializes the internal logger.
        ///
        /// Various actions and events, including the errors, are being logged throughout the framework. They may be
        /// stored into the file.
        /// </summary>
        /// <param name="fileName">A full path to the file where the log is to be stored</param>
        /// <param name="overwrite">A flag defining the file open mode; if it's true then the log file will be rewritten</param>
        /// <param name="verbose">A flag defining the logging mode; if it's true then the verbose logging will be enabled</param>
        /// <param name="logDataTransfer">A flag defining the logging mode; if it's nonzero then the data transfer
        /// (portions of received and sent data) logging will be enabled</param>
        public static void InitializeLogging(string fileName, bool overwrite, bool verbose,
            bool logDataTransfer = false)
        {
            C.CheckOk(C.Instance.dxf_initialize_logger_v2(fileName, overwrite, true, verbose, logDataTransfer));
        }

        /// <summary>
        /// Initializes the C-API configuration and loads a config (in TOML format) from a string
        /// <br/>
        /// For the successful application of the configuration, this function must be called before creating any connection
        /// <br/>
        /// The config file sample: [Sample](https://github.com/dxFeed/dxfeed-net-api/dxfeed-api-config.sample.toml)
        /// <br/>
        /// The TOML format specification: https://toml.io/en/v1.0.0-rc.2
        /// <br/>
        /// <remarks><!-- for doxygen --></remarks>
        /// <example>
        /// Example #1
        /// <code>
        /// NativeTools.LoadConfigFromString(@"
        /// network.heartbeatPeriod = 10
        /// network.heartbeatTimeout = 120
        /// ");
        /// </code>
        /// <br/>
        /// Example #2
        /// <code>
        /// NativeTools.LoadConfigFromString(@"
        /// [network]
        /// heartbeatPeriod = 10
        /// heartbeatTimeout = 120
        /// ");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="config">The config (in TOML format) string</param>
        public static void LoadConfigFromString(string config)
        {
            C.CheckOk(C.Instance.dxf_load_config_from_string(config));
        }
        
        /// <summary>
        /// Initializes the C-API configuration and loads a config (in TOML format) from a string
        /// For the successful application of the configuration, this function must be called before creating any connection
        ///
        /// The config file sample: [Sample](https://github.com/dxFeed/dxfeed-net-api/dxfeed-api-config.sample.toml)
        /// The TOML format specification: https://toml.io/en/v1.0.0-rc.2
        /// <br/>
        /// <remarks><!-- for doxygen --></remarks>
        /// <example>
        /// <code>
        /// NativeTools.LoadConfigFromFile("./dxfeed-api-config.toml");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="fileName">The config (in TOML format) file name</param>
        public static void LoadConfigFromFile(string fileName)
        {
            C.CheckOk(C.Instance.dxf_load_config_from_file(fileName));
        }
    }
}