#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Runtime.InteropServices;
using com.dxfeed.api.events;
using com.dxfeed.api.data;
using com.dxfeed.api.connection;

namespace com.dxfeed.native.api
{
    internal abstract class C
    {
        private static C instance;
        private static object syncRoot = new Object();

        protected C() { }

        internal static C Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            if (Environment.Is64BitProcess)
                                instance = new C64();
                            else
                                instance = new C32();
                        }
                    }
                }

                return instance;
            }
        }

        internal const int DX_OK = 1;
        internal const int DX_ERR = 0;


        /// <summary>
        ///   Helper method to check error codes
        ///   Throws NativeDxException if return_code != DX_OK
        /// </summary>
        /// <param name="returnCode"></param>
        /// <exception cref="NativeDxException"></exception>
        internal static void CheckOk(int returnCode)
        {
            if (returnCode != DX_OK)
                throw NativeDxException.Create();
        }

        /*
         *  Event listener prototype
         *
         *  event type here is a one-bit mask, not an integer
         *  from dx_eid_begin to dx_eid_count
         */
        /* -------------------------------------------------------------------------- */
        /*
         * typedef void (*dxf_event_listener_t) (int event_type, dxf_const_string_t symbol_name,
                                                 const dxf_event_data_t* data, int data_count,
                                                 void* user_data);
         */

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_event_listener_t(EventType event_type, IntPtr symbol, IntPtr data, int data_count, IntPtr user_data);

        /*
            typedef void (*dxf_event_listener_t) (int event_type, dxf_const_string_t symbol_name,
                                                  const dxf_event_data_t* data, int data_count,
                                                  const dxf_event_params_t* event_params, void* user_data);
        */
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_event_listener_v2_t(EventType event_type, IntPtr symbol, IntPtr data, int data_count, IntPtr event_params, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_conn_termination_notifier_t(IntPtr connection, IntPtr user_data);

        /*
          typedef void (*dxf_conn_status_notifier_t) (dxf_connection_t connection,
                                                      dxf_connection_status_t old_status,
                                                      dxf_connection_status_t new_status,
                                                      void* user_data);
         */
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_conn_status_notifier_t(IntPtr connection, ConnectionStatus old_status, ConnectionStatus new_status, IntPtr user_data);
        
        /// <summary>
        /// The callback type of a connection incoming heartbeat notification
        ///
        /// Called when a server heartbeat arrives and contains non empty payload
        /// </summary>
        /// <param name="connection">The connection handle</param>
        /// <param name="serverMillis">The server time in milliseconds (from the incoming heartbeat payload)</param>
        /// <param name="serverLagMark">The server's messages composing lag time in microseconds (from the incoming heartbeat payload)</param>
        /// <param name="connectionRtt">The calculated connection RTT in microseconds</param>
        /// <param name="userData">The user data passed to SetOnServerHeartbeatNotifier</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ConnectionOnServerHeartbeatNotifier(IntPtr connection, long serverMillis, int serverLagMark, int connectionRtt, IntPtr userData);
        
        
        /* the low level callback types, required in case some thread-specific initialization must be performed
           on the client side on the thread creation/destruction
         */

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int dxf_socket_thread_creation_notifier_t(IntPtr connection, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_socket_thread_destruction_notifier_t(IntPtr connection, IntPtr user_data);

        /// <summary>
        /// Snapshot listener prototype
        /// </summary>
        /// <param name="snapshotData">pointer to the received snapshot data</param>
        /// <param name="userData">pointer to user struct, use NULL by default</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_snapshot_listener_t(IntPtr snapshotData, IntPtr userData);

        /// <summary>
        /// Snapshot listener prototype
        /// </summary>
        /// <param name="snapshotData">pointer to the received snapshot data</param>
        /// <param name="new_snapshot">snapshot or update</param>
        /// <param name="userData">pointer to user struct, use NULL by default</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_snapshot_inc_listener_t(IntPtr snapshotData, int new_snapshot, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_price_level_book_listener_t(IntPtr price_level_book, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_price_level_book_inc_listener_t(IntPtr removals, IntPtr additions, IntPtr updates, IntPtr user_data);
         
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_regional_quote_listener_t(IntPtr symbol, IntPtr quote, int count, IntPtr user_data);

        /// <summary>
        /// Initializes the internal logger.
        ///
        /// Various actions and events, including the errors, are being logged throughout the framework. They may be stored into the file.
        ///
        /// C-API: DXFEED_API ERRORCODE dxf_initialize_logger(const char* file_name, int rewrite_file, int show_timezone_info, int verbose);
        /// </summary>
        /// <param name="file_name">A full path to the file where the log is to be stored</param>
        /// <param name="rewrite_file">A flag defining the file open mode; if it's nonzero then the log file will be rewritten</param>
        /// <param name="show_time_zone_info">A flag defining the time display option in the log file; if it's nonzero then the time will be displayed with the timezone suffix</param>
        /// <param name="verbose">A flag defining the logging mode; if it's nonzero then the verbose logging will be enabled</param>
        /// <returns>DXF_SUCCESS on successful logger initialization or DXF_FAILURE on error; link dxf_get_last_error can be used to retrieve the error code and description in case of failure;</returns>
        internal abstract int dxf_initialize_logger(string file_name, bool rewrite_file, bool show_time_zone_info, bool verbose);

        /// <summary>
        /// Initializes the internal logger with data transfer logging.
        ///
        /// Various actions and events, including the errors, are being logged throughout the framework. They may be stored into the file.
        ///
        /// C-API: DXFEED_API ERRORCODE dxf_initialize_logger_v2(const char* file_name, int rewrite_file, int show_timezone_info, int verbose, int log_data_transfer);
        /// </summary>
        /// <param name="file_name">A full path to the file where the log is to be stored</param>
        /// <param name="rewrite_file">A flag defining the file open mode; if it's nonzero then the log file will be rewritten</param>
        /// <param name="show_time_zone_info">A flag defining the time display option in the log file; if it's nonzero then the time will be displayed with the timezone suffix</param>
        /// <param name="verbose">A flag defining the logging mode; if it's nonzero then the verbose logging will be enabled</param>
        /// <param name="log_data_transfer">A flag defining the logging mode; if it's nonzero then the data transfer (portions of received and sent data) logging will be enabled</param>
        /// <returns>DXF_SUCCESS on successful logger initialization or DXF_FAILURE on error; link dxf_get_last_error can be used to retrieve the error code and description in case of failure;</returns>
        internal abstract int dxf_initialize_logger_v2(string file_name, bool rewrite_file, bool show_time_zone_info, bool verbose, bool log_data_transfer);

        /*
         *  Creates connection with the specified parameters.

         *  address - the single address: "host:port" or just "host"
         *            address with credentials: "host:port[username=xxx,password=yyy]"
         *            multiple addresses: "(host1:port1)(host2)(host3:port3[username=xxx,password=yyy])"
         *            the data from file: "/path/to/file" on *nix and "drive:\path\to\file" on Windows
         *  notifier - the callback to inform the client side that the connection has stumbled upon and error and will go reconnecting
         *  conn_status_notifier - the callback to inform the client side that the connection status has changed
         *  stcn - the callback for informing the client side about the socket thread creation;
                   may be set to NULL if no specific action is required to perform on the client side on a new thread creation
         *  shdn - the callback for informing the client side about the socket thread destruction;
                   may be set to NULL if no specific action is required to perform on the client side on a thread destruction
         *  user_data - the user defined value passed to the termination notifier callback along with the connection handle; may be set
                        to whatever value
         *  OUT connection - the handle of the created connection
         */
        // DXFEED_API ERRORCODE dxf_create_connection (const char* address,
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_create_connection(
            string address,
            dxf_conn_termination_notifier_t notifier,
            dxf_conn_status_notifier_t conn_status_notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection);

        /*
         *  Creates connection with the specified parameters and basic authorization.

         *  address - the single address: "host:port" or just "host"
         *            address with credentials: "host:port[username=xxx,password=yyy]"
         *            multiple addresses: "(host1:port1)(host2)(host3:port3[username=xxx,password=yyy])"
         *            the data from file: "/path/to/file" on *nix and "drive:\path\to\file" on Windows
         *  user - the user name;
         *  password - the user password;
         *  notifier - the callback to inform the client side that the connection has stumbled upon and error and will go reconnecting
         *  conn_status_notifier - the callback to inform the client side that the connection status has changed
         *  stcn - the callback for informing the client side about the socket thread creation;
                   may be set to NULL if no specific action is required to perform on the client side on a new thread creation
         *  shdn - the callback for informing the client side about the socket thread destruction;
                   may be set to NULL if no specific action is required to perform on the client side on a thread destruction;
         *  user_data - the user defined value passed to the termination notifier callback along with the connection handle; may be set
                        to whatever value;
         *  OUT connection - the handle of the created connection.
         *
         * Note: the user and password data from argument have a higher priority than address credentials.
         */
        internal abstract int dxf_create_connection_auth_basic(string address,
                                                               string user,
                                                               string password,
                                                               dxf_conn_termination_notifier_t notifier,
                                                               dxf_conn_status_notifier_t conn_status_notifier,
                                                               dxf_socket_thread_creation_notifier_t stcn,
                                                               dxf_socket_thread_destruction_notifier_t stdn,
                                                               IntPtr user_data,
                                                               out IntPtr connection);

        /*
         *  Creates connection with the specified parameters and token authorization.

         *  address - the single address: "host:port" or just "host"
         *            address with credentials: "host:port[username=xxx,password=yyy]"
         *            multiple addresses: "(host1:port1)(host2)(host3:port3[username=xxx,password=yyy])"
         *            the data from file: "/path/to/file" on *nix and "drive:\path\to\file" on Windows
         *  token - the authorization token;
         *  notifier - the callback to inform the client side that the connection has stumbled upon and error and will go reconnecting
         *  conn_status_notifier - the callback to inform the client side that the connection status has changed
         *  stcn - the callback for informing the client side about the socket thread creation;
                   may be set to NULL if no specific action is required to perform on the client side on a new thread creation
         *  shdn - the callback for informing the client side about the socket thread destruction;
                   may be set to NULL if no specific action is required to perform on the client side on a thread destruction;
         *  user_data - the user defined value passed to the termination notifier callback along with the connection handle; may be set
                        to whatever value;
         *  OUT connection - the handle of the created connection.
         *
         * Note: the token data from argument have a higher priority than address credentials.
         */
        internal abstract int dxf_create_connection_auth_bearer(string address,
                                                                string token,
                                                                dxf_conn_termination_notifier_t notifier,
                                                                dxf_conn_status_notifier_t conn_status_notifier,
                                                                dxf_socket_thread_creation_notifier_t stcn,
                                                                dxf_socket_thread_destruction_notifier_t stdn,
                                                                IntPtr user_data,
                                                                out IntPtr connection);

        /*
         *  Creates connection with the specified parameters and custom described authorization.

         *  address - the single address: "host:port" or just "host"
         *            address with credentials: "host:port[username=xxx,password=yyy]"
         *            multiple addresses: "(host1:port1)(host2)(host3:port3[username=xxx,password=yyy])"
         *            the data from file: "/path/to/file" on *nix and "drive:\path\to\file" on Windows
         *  authscheme - the authorization scheme;
         *  authdata - the authorization data;
         *  notifier - the callback to inform the client side that the connection has stumbled upon and error and will go reconnecting
         *  conn_status_notifier - the callback to inform the client side that the connection status has changed
         *  stcn - the callback for informing the client side about the socket thread creation;
                   may be set to NULL if no specific action is required to perform on the client side on a new thread creation
         *  shdn - the callback for informing the client side about the socket thread destruction;
                   may be set to NULL if no specific action is required to perform on the client side on a thread destruction;
         *  user_data - the user defined value passed to the termination notifier callback along with the connection handle; may be set
                        to whatever value;
         *  OUT connection - the handle of the created connection.
         *
         * Note: the authscheme and authdata from argument have a higher priority than address credentials.
         */
        internal abstract int dxf_create_connection_auth_custom(string address,
                                                                string authscheme,
                                                                string authdata,
                                                                dxf_conn_termination_notifier_t notifier,
                                                                dxf_conn_status_notifier_t conn_status_notifier,
                                                                dxf_socket_thread_creation_notifier_t stcn,
                                                                dxf_socket_thread_destruction_notifier_t stdn,
                                                                IntPtr user_data,
                                                                out IntPtr connection);

        /// <summary>
        /// Sets a server heartbeat notifier's callback to the connection.
        ///
        /// This notifier will be invoked when the new heartbeat arrives from a server and contains non empty payload
        ///
        /// `[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_set_on_server_heartbeat_notifier")]`
        /// </summary>
        /// <param name="connection">The handle of a previously created connection</param>
        /// <param name="notifier">The notifier callback function pointer</param>
        /// <param name="userData">The data to be passed to the callback function</param>
        /// <returns>
        /// DXF_SUCCESS (DX_OK) on successful logger initialization or DXF_FAILURE (DX_ERR) on error; link dxf_get_last_error
        /// can be used to retrieve the error code and description in case of failure;
        /// </returns>
        internal abstract int SetOnServerHeartbeatNotifier(IntPtr connection,
            ConnectionOnServerHeartbeatNotifier notifier, IntPtr userData);

        /*
         *  Closes a connection.
         *  connection - a handle of a previously created connection
         */
        //DXFEED_API ERRORCODE dxf_close_connection (dxf_connection_t connection);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_close_connection(IntPtr connection);

        /*
         *  Creates a subscription with the specified parameters.
         *
         *  connection - a handle of a previously created connection which the subscription will be using
         *  event_types - a bitmask of the subscription event types. See 'dx_event_id_t' and 'DX_EVENT_BIT_MASK'
         *              for information on how to create an event type bitmask
         *  OUT subscription - a handle of the created subscription
         */
        //DXFEED_API ERRORCODE dxf_create_subscription (dxf_connection_t connection, int event_types,
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_create_subscription(IntPtr connection, EventType event_types, out IntPtr subscription);

        /// <summary>
        /// Creates a subscription with the specified parameters and the subscription flags.
        /// </summary>
        /// <param name="connection">A handle of a previously created connection which the subscription will be using</param>
        /// <param name="event_types">A bitmask of the subscription event types. See: EventType</param>
        /// <param name="subscr_flags">A bitmask of the subscription event flags. See: EventSubscriptionFlag</param>
        /// <param name="subscription">A handle of the created subscription</param>
        /// <returns>DX_OK (1) on successful subscription creation or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure; a handle to
        /// newly created subscription is returned via <paramref name="subscription"/> out parameter</returns>
        internal abstract int dxf_create_subscription_with_flags(IntPtr connection, EventType event_types,
            EventSubscriptionFlag subscr_flags, out IntPtr subscription);
        
        /*
         *  Creates a timed subscription with the specified parameters.

         *  connection - a handle of a previously created connection which the subscription will be using
         *  event_types - a bitmask of the subscription event types. See 'dx_event_id_t' and 'DX_EVENT_BIT_MASK'
         *                for information on how to create an event type bitmask
         *  time - time in the past (unix time in milliseconds)
         *  OUT subscription - a handle of the created subscription
         */
        internal abstract int dxf_create_subscription_timed(IntPtr connection, EventType event_types,
                                                            Int64 time, out IntPtr subscription);
        
        /// <summary>
        /// Creates a timed subscription with the specified parameters and the subscription flags.
        /// </summary>
        /// <param name="connection">A handle of a previously created connection which the subscription will be using</param>
        /// <param name="event_types">A bitmask of the subscription event types. See: EventType</param>
        /// <param name="time">UTC time in the past (unix time in milliseconds)</param>
        /// <param name="subscr_flags">A bitmask of the subscription event flags. See: EventSubscriptionFlag</param>
        /// <param name="subscription">A handle of the created subscription</param>
        /// <returns>DX_OK (1) on successful subscription creation or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure; a handle to
        /// newly created subscription is returned via <paramref name="subscription"/> out parameter</returns>
        internal abstract int dxf_create_subscription_timed_with_flags(IntPtr connection, EventType event_types,
            Int64 time, EventSubscriptionFlag subscr_flags, out IntPtr subscription);

        /*
         *  Closes a subscription.
         *  All the data associated with it will be freed.
         *
         *  subscription - a handle of the subscription to close
         */
        //DXFEED_API ERRORCODE dxf_close_subscription (dxf_subscription_t subscription);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_close_subscription(IntPtr subscription);

        /*
         *  Adds a single symbol to the subscription.
         *
         *  subscription - a handle of the subscription to which a symbol is added
         *  symbol - the symbol to add
         */
        //DXFEED_API ERRORCODE dxf_add_symbol (dxf_subscription_t subscription, dxf_const_string_t symbol);
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_add_symbol(IntPtr subscription, String symbol);

        /*
         *  Adds several symbols to the subscription.
         *  No error occurs if the symbol is attempted to add for the second time.
         *
         *  subscription - a handle of the subscription to which the symbols are added
         *  symbols - the symbols to add
         *  symbol_count - a number of symbols
         */
        //DXFEED_API ERRORCODE dxf_add_symbols (dxf_subscription_t subscription, dxf_const_string_t* symbols, int symbol_count);
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_add_symbols(IntPtr subscription, string[] symbols, int count);

        /*
         *  Adds a candle symbol to the subscription.

         *  subscription - a handle of the subscription to which a symbol is added
         *  candle_attributes - pointer to the candle struct
         */
        internal abstract int dxf_add_candle_symbol(IntPtr subscription, IntPtr candle_attributes);

        /*
         *  Remove a candle symbol from the subscription.

         *  subscription - a handle of the subscription from symbol will be removed
         *  candle_attributes - pointer to the candle struct
        */
        internal abstract int dxf_remove_candle_symbol(IntPtr subscription, IntPtr candle_attributes);

        /*
         *  Removes a single symbol from the subscription.
         *
         *  subscription - a handle of the subscription from which a symbol is removed
         *  symbol - the symbol to remove
         */
        //DXFEED_API ERRORCODE dxf_remove_symbol (dxf_subscription_t subscription, dxf_const_string_t symbol);
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_remove_symbol(IntPtr subcription, string symbol);

        /*
         *  Removes several symbols from the subscription.
         *  No error occurs if it's attempted to remove symbols that weren't added beforehand.
         *
         *  subscription - a handle of the subscription to which the symbols are added
         *  symbols - the symbols to remove
         *  symbol_count - a number of symbols
         */
        //DXFEED_API ERRORCODE dxf_remove_symbols (dxf_subscription_t subscription, dxf_const_string_t* symbols, int symbol_count);
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_remove_symbols(IntPtr subscription, string[] symbols, int count);

        /*
         *  Retrieves the list of symbols currently added to the subscription.
         *  The memory for the resulting list is allocated internally, so no actions to free it are required.
         *  The symbol name buffer is guaranteed to be valid until either the subscription symbol list is changed or a new call
         *  of this function is performed.
         *
         *  subscription - a handle of the subscriptions whose symbols are to retrieve
         *  OUT symbols - a pointer to the string array object to which the symbol list is to be stored
         *  OUT symbol_count - a pointer to the variable to which the symbol count is to be stored
         */
        //DXFEED_API ERRORCODE dxf_get_symbols (dxf_subscription_t subscription, OUT dxf_const_string_t** symbols, OUT int* symbol_count);
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_get_symbols(IntPtr subscription, out IntPtr symbols, out int count);

        /*
         *  Sets the symbols for the subscription.
         *  The difference between this function and 'dxf_add_symbols' is that all the previously added symbols
         *  that do not belong to the symbol list passed to this function will be removed.
         *
         *  subscription - a handle of the subscription whose symbols are to be set
         *  symbols - the symbol list to set
         *  symbol_count - the symbol count
         */
        //DXFEED_API ERRORCODE dxf_set_symbols (dxf_subscription_t subscription, dxf_const_string_t* symbols, int symbol_count);
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_set_symbols(IntPtr subscription, string[] symbols, int count);

        /*
         *  Removes all the symbols from the subscription.
         *
         *  subscription - a handle of the subscription whose symbols are to be cleared
         */
        //DXFEED_API ERRORCODE dxf_clear_symbols (dxf_subscription_t subscription);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_clear_symbols(IntPtr subscription);

        /*
         *  Attaches a listener callback to the subscription.
         *  This callback will be invoked when the new event data for the subscription symbols arrives.
         *  No error occurs if it's attempted to attach the same listener twice or more.
         *
         *  subscription - a handle of the subscription to which a listener is to be attached
         *  event_listener - a listener callback function pointer
         */
        //DXFEED_API ERRORCODE dxf_attach_event_listener (dxf_subscription_t subscription, dxf_event_listener_t event_listener, void* user_data);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_attach_event_listener(IntPtr subscription, dxf_event_listener_t event_listener,
                                                        IntPtr user_data);

        /*
         *  Detaches a listener from the subscription.
         *  No error occurs if it's attempted to detach a listener which wasn't previously attached.
         *
         *  subscription - a handle of the subscription from which a listener is to be detached
         *  event_listener - a listener callback function pointer
         */
        //DXFEED_API ERRORCODE dxf_detach_event_listener (dxf_subscription_t subscription, dxf_event_listener_t event_listener);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_detach_event_listener(IntPtr subscription, dxf_event_listener_t listener);

        /*
         *  Attaches a extended listener callback to the subscription.
         *  This callback will be invoked when the new event data for the subscription symbols arrives.
         *  No error occurs if it's attempted to attach the same listener twice or more.
         *
         *  This listener differs with extend number of callback parameters.
         *
         *  subscription - a handle of the subscription to which a listener is to be attached
         *  event_listener - a listener callback function pointer
         *  user_data - if there isn't user data pass NULL
         */
        internal abstract int dxf_attach_event_listener_v2(IntPtr subscription, dxf_event_listener_v2_t event_listener,
                                                           IntPtr user_data);

        /*
         *  Detaches a extended listener from the subscription.
         *  No error occurs if it's attempted to detach a listener which wasn't previously attached.
         *
         *  subscription - a handle of the subscription from which a listener is to be detached
         *  event_listener - a listener callback function pointer
         */
        internal abstract int dxf_detach_event_listener_v2(IntPtr subscription, dxf_event_listener_v2_t listener);

        /*
         *  Retrieves the subscription event types.
         *
         *  subscription - a handle of the subscription whose event types are to be retrieved
         *  OUT event_types - a pointer to the variable to which the subscription event types bitmask is to be stored
         */
        //DXFEED_API ERRORCODE dxf_get_subscription_event_types (dxf_subscription_t subscription, OUT int* event_types);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_get_subscription_event_types(IntPtr subscription, out int event_types);

        /*
         *  Retrieves the last event data of the specified symbol and type for the connection.
         *
         *  connection - a handle of the connection whose data is to be retrieved
         *  event_type - an event type bitmask defining a single event type
         *  symbol - a symbol name
         *  OUT event_data - a pointer to the variable to which the last data buffer pointer is stored; if there was no
                                     data for this connection/event type/symbol, NULL will be stored
         */
        //DXFEED_API ERRORCODE dxf_get_last_event (dxf_connection_t connection, int event_type, dxf_const_string_t symbol, OUT const dxf_event_data_t* event_data);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_get_last_event(IntPtr connection, int event_type, string symbol, IntPtr event_data);

        /*
         *  Retrieves the last error info.
         *  The error is stored on per-thread basis. If the connection termination notifier callback was invoked, then
         *  to retrieve the connection's error code call this function right from the callback function.
         *  If an error occurred within the error storage subsystem itself, the function will return DXF_FAILURE.
         *
         *  OUT error_code - a pointer to the variable where the error code is to be stored
         *  OUT error_descr - a pointer to the variable where the human-friendly error description is to be stored;
         *                    may be NULL if the text representation of error is not required
         */
        //DXFEED_API ERRORCODE dxf_get_last_error (OUT int* error_code, OUT dxf_const_string_t* error_descr);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_get_last_error(out int error_code, out IntPtr error_descr);

        /*
         *  Clear current sources and add new one to subscription
         *  Warning: you must configure order source before dxf_add_symbols/dxf_add_symbol call
         *
         *  subscription - a handle of the subscription where source will be changed
         *  source - source of order to set, 4 symbols maximum length
         */
        internal abstract int dxf_set_order_source(IntPtr subscription, byte[] source);

        /*
         *  Add a new source to subscription
         *  Warning: you must configure order source before dxf_add_symbols/dxf_add_symbol call
         *
         *  subscription - a handle of the subscription where source will be changed
         *  source - source of order event to add, 4 symbols maximum length
         */
        internal abstract int dxf_add_order_source(IntPtr subscription, byte[] source);

        /// <summary>
        /// API that allows user to create candle symbol attributes
        /// </summary>
        /// <param name="base_symbol">The base symbol</param>
        /// <param name="exchange_code">Exchange attribute of this symbol (A-Z)</param>
        /// <param name="period_value">Aggregation period value of this symbol</param>
        /// <param name="period_type">Aggregation period type of this symbol</param>
        /// <param name="price">Price ("price" key) type attribute of this symbol</param>
        /// <param name="session">Session ("tho" key) attribute of this symbol</param>
        /// <param name="alignment">Alignment ("a" key) attribute of this symbol</param>
        /// <param name="price_level">Price level ("pl" key) attribute of this symbol. The candle price level defines 
        /// additional axis to split candles within particular price corridor in addition to candle period attribute
        /// with the default value NAN.</param>
        /// <param name="candle_attributes">Pointer to the configured candle attributes struct</param>
        /// <returns>DX_OK (1) if candle attributes have been created successfully or DX_ERR (0) on error.
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure;
        /// *candle_attributes* are returned via output parameter</returns>
        internal abstract int dxf_create_candle_symbol_attributes(string base_symbol,
                                                                 char exchange_code,
                                                                 double period_value,
                                                                 int period_type,
                                                                 int price,
                                                                 int session,
                                                                 int alignment,
                                                                 double price_level,
                                                                 out IntPtr candle_attributes);

        /*
         *  Free memory allocated by dxf_initialize_candle_symbol_attributes(...) function

         *  candle_attributes - pointer to the candle attributes struct
         */
        internal abstract int dxf_delete_candle_symbol_attributes(IntPtr candle_attributes);

        /*
         *  Creates snapshot with the specified parameters.
         *
         *  For Order or Candle events (dx_eid_order or dx_eid_candle) please use
         *  short form of this function: dxf_create_order_snapshot or dxf_create_candle_snapshot
         *  respectively.
         *
         *  For order events (event_id is 'dx_eid_order')
         *  If source is NULL string subscription on Order event will be performed. You can specify order
         *  source for Order event by passing suffix: "BYX", "BZX", "DEA", "DEX", "ISE", "IST", "NTV", ...
         *  If source is equal to "AGGREGATE_BID" or "AGGREGATE_ASK" subscription on MarketMaker event will
         *  be performed. For other events source parameter does not matter.
         *
         *  connection - a handle of a previously created connection which the subscription will be using
         *  event_id - single event id. Next events is supported: dxf_eid_order, dxf_eid_candle,
                       dx_eid_spread_order, dx_eid_time_and_sale.
         *  symbol - the symbol to add.
         *  source - order source for Order, which can be one of following: "NTV", "ntv", "NFX", "ESPD", "XNFI", "ICE",
         *           "ISE", "DEA", "DEX", "BYX", "BZX", "BATE", "CHIX", "CEUX", "BXTR", "IST", "BI20", "ABE", "FAIR",
         *           "GLBX", "glbx", "ERIS", "XEUR", "xeur", "CFE", "C2OX", "SMFE", "smfe", "iex", "MEMX", "memx"
         *           For MarketMaker subscription use "AGGREGATE_BID" or
         *           "AGGREGATE_ASK" keyword.
         *  time - time in the past (unix time in milliseconds).
         *  OUT snapshot - a handle of the created snapshot
         */
        internal abstract int dxf_create_snapshot(IntPtr connection, int event_id,
                                                 string symbol, byte[] source,
                                                 Int64 time, out IntPtr snapshot);

        /*
         *  Creates Order snapshot with the specified parameters.
         *
         *  If source is NULL string subscription on Order event will be performed. You can specify order
         *  source for Order event by passing suffix: "BYX", "BZX", "DEA", "DEX", "ISE", "IST", "NTV", ...
         *  If source is equal to "AGGREGATE_BID" or "AGGREGATE_ASK" subscription on MarketMaker event will
         *  be performed.
         *
         *  connection - a handle of a previously created connection which the subscription will be using
         *  symbol - the symbol to add
         *  source - order source for Order event with 4 symbols maximum length OR keyword which can be
         *           one of AGGREGATE_BID or AGGREGATE_ASK
         *  time - time in the past (unix time in milliseconds)
         *  OUT snapshot - a handle of the created snapshot
         */
        internal abstract int dxf_create_order_snapshot(IntPtr connection, string symbol, byte[] source,
                                                        Int64 time, out IntPtr snapshot);

        /*
         *  Creates Candle snapshot with the specified parameters.
         *
         *  connection - a handle of a previously created connection which the subscription will be using
         *  candle_attributes - object specified symbol attributes of candle
         *  time - time in the past (unix time in milliseconds)
         *  OUT snapshot - a handle of the created snapshot
         */
        internal abstract int dxf_create_candle_snapshot(IntPtr connection, IntPtr candle_attributes,
                                                         Int64 time, out IntPtr snapshot);

        /*
         *  Closes a snapshot.
         *  All the data associated with it will be freed.
         *
         *  snapshot - a handle of the snapshot to close
         */
        internal abstract int dxf_close_snapshot(IntPtr snapshot);

        /*
         *  Attaches a listener callback to the snapshot.
         *  This callback will be invoked when the new snapshot arrives or existing updates.
         *  No error occurs if it's attempted to attach the same listener twice or more.
         *
         *  snapshot - a handle of the snapshot to which a listener is to be attached
         *  snapshot_listener - a listener callback function pointer
        */
        internal abstract int dxf_attach_snapshot_listener(IntPtr snapshot, dxf_snapshot_listener_t snapshotListener,
                                                           IntPtr userData);

        /*
         *  Attaches a listener callback to the snapshot.
         *  This callback will be invoked when the new snapshot arrives or existing updates.
         *  No error occurs if it's attempted to attach the same listener twice or more.
         *
         *  snapshot - a handle of the snapshot to which a listener is to be attached
         *  snapshot_listener - a listener callback function pointer
        */
        internal abstract int dxf_attach_snapshot_inc_listener(IntPtr snapshot, dxf_snapshot_inc_listener_t snapshotListener,
                                                               IntPtr userData);

        /// <summary>
        /// Detaches a listener from the snapshot.
        /// </summary>
        /// <param name="snapshot">A handle of a snapshot from which the listener should be detached.</param>
        /// <param name="snapshotListener">A listener callback function pointer</param>
        /// <returns>DX_OK (1) on successful detaching or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure</returns>
        internal abstract int dxf_detach_snapshot_listener(IntPtr snapshot, dxf_snapshot_listener_t snapshotListener);
        
        /// <summary>
        /// Detaches an incremental listener from the snapshot.
        /// </summary>
        /// <param name="snapshot">A handle of a snapshot from which the listener should be detached.</param>
        /// <param name="snapshotListener">A listener callback function pointer</param>
        /// <returns>DX_OK (1) on successful detaching or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure</returns>
        internal abstract int dxf_detach_snapshot_inc_listener(IntPtr snapshot, dxf_snapshot_inc_listener_t snapshotListener);

        /*
         *  Retrieves the symbol currently added to the snapshot subscription.
         *  The memory for the resulting symbol is allocated internally, so no actions to free it are required.
         *
         *  snapshot - a handle of the snapshot to which a listener is to be detached
         *  OUT symbol - a pointer to the string object to which the symbol is to be stored
         */
        internal abstract int dxf_get_snapshot_symbol(IntPtr snapshot, out IntPtr symbol);

        /// <summary>
        /// Creates Price Level book with the specified parameters.
        /// </summary>
        /// <param name="connection">A handle of a previously created connection which the subscription will be using</param>
        /// <param name="symbol">The symbol to use</param>
        /// <param name="sources">Order sources for Order. Each element can be one of following:
        /// "NTV", "ntv", "NFX", "ESPD", "XNFI", "ICE", "ISE", "DEA", "DEX", "BYX", "BZX", "BATE", "CHIX",
        /// "CEUX", "BXTR", "IST", "BI20", "ABE", "FAIR", "GLBX", "glbx", "ERIS", "XEUR", "xeur", "CFE",
        /// "C2OX", "SMFE", "smfe", "iex", "MEMX", "memx"</param>
        /// <param name="sourcesCount">The number of sources. 0 - all sources</param>
        /// <param name="book">A handle of the created price level book</param>
        /// <returns>DX_OK (1) on successful PLB creation or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure</returns>
        internal abstract int CreatePriceLevelBook(IntPtr connection, string symbol, string[] sources, int sourcesCount, out IntPtr book);

        /// <summary>
        /// Closes a price level book.
        /// </summary>
        /// <param name="book">A handle of the price level book to close</param>
        /// <returns>DX_OK (1) if price level book has been successfully closed or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure</returns>
        internal abstract int ClosePriceLevelBook(IntPtr book);

        /// <summary>
        /// Attaches a listener callback to the price level book.
        /// </summary>
        /// <param name="book">A handle of the book to which a listener is to be attached</param>
        /// <param name="bookListener">A listener callback function pointer</param>
        /// <param name="userData">Data to be passed to the callback function</param>
        /// <returns>DX_OK (1) if listener has been successfully attached or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure</returns>
        internal abstract int AttachPriceLevelBookListener(IntPtr book, dxf_price_level_book_listener_t bookListener,
            IntPtr userData);

        /// <summary>
        /// Detaches a listener from the price level book.
        ///
        /// No error occurs if it's attempted to detach a listener which wasn't previously attached.
        /// </summary>
        /// <param name="book">A handle of the book to which a listener is to be detached</param>
        /// <param name="bookListener">A listener callback function pointer</param>
        /// <returns>DX_OK (1) if listener has been successfully detached or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure</returns>
        internal abstract int DetachPriceLevelBookListener(IntPtr book, dxf_price_level_book_listener_t bookListener);

        /// <summary>
        /// Creates Price Level book (v2) with the specified parameters.
        /// </summary>
        /// <param name="connection">A handle of a previously created connection which the subscription will be using</param>
        /// <param name="symbol">The symbol to use</param>
        /// <param name="source">Order source for Order (AAPL, NTV etc) or MarketMaker (AGGREGATE_ASK or AGGREGATE_BID)</param>
        /// <param name="levelsNumber">The PLB levels number (0 - all levels)</param>
        /// <param name="book">A handle of the created price level book</param>
        /// <returns>DX_OK (1) if price level book has been successfully created or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure</returns>
        internal abstract int CreatePriceLevelBook2(IntPtr connection, string symbol, string source, int levelsNumber, out IntPtr book);
        
        /// <summary>
        /// Closes a price level book (v2).
        /// </summary>
        /// <param name="book">A handle of the price level book to close</param>
        /// <returns>DX_OK (1) if price level book has been successfully closed or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure</returns>
        internal abstract int ClosePriceLevelBook2(IntPtr book);

        /// <summary>
        /// Sets the listener callbacks to the PLB.
        /// </summary>
        /// <param name="book">The handle of the PLB</param>
        /// <param name="onNewBookListener">The listener that will be called when a new book is created (for example, when trading starts)</param>
        /// <param name="onBookUpdateListener">The listener that will be called when the book is updated.
        /// In this case, all price levels will be passed (taking into account the maximum number of price levels)</param>
        /// <param name="onIncrementalChangeListener">The listener that will be called on incremental updates. All deletions, additions, and level updates will be passed. </param>
        /// <param name="userData">User data to be passed to listeners.</param>
        /// <returns>DX_OK (1) if callbacks have been successfully set or DX_ERR (0) on error;
        /// dxf_get_last_error can be used to retrieve the error code and description in case of failure</returns>
        internal abstract int SetPriceLevelBookListeners(IntPtr book,
         dxf_price_level_book_listener_t onNewBookListener, dxf_price_level_book_listener_t onBookUpdateListener,
         dxf_price_level_book_inc_listener_t onIncrementalChangeListener, IntPtr userData);
        
        /*
         *  Add dumping of incoming traffic into specific file
         *
         *  connection - a handle of a previously created connection which the subscription will be using
         *  raw_file_name - raw data file name
         */
        internal abstract int dxf_write_raw_data(IntPtr connection, byte[] raw_file_name);

        /*
        *  Retrieves the array of key-value pairs (properties) for specified connection. The memory for the resulting array
        *  is allocated during execution of the function and SHOULD be free by caller with dxf_free_connection_properties_snapshot
        *  function. So done because connection properties can be changed during reconnection. Returned array is a snapshot
        *  of properties at the moment of the call.
        *
        *  connection - a handle of a previously created connection
        *  OUT properties - pointer to store address of key-value pairs array
        *  OUT count - variable to store length of key-value pairs array
        */
        internal abstract int dxf_get_connection_properties_snapshot(IntPtr connection, out IntPtr properties, out int count);

        /*
        * Frees memory allocated during dxf_get_connection_properties_snapshot function execution
        *
        *  properties - pointer to the key-value pairs array
        *  count - length of key-value pairs array
        */
        internal abstract int dxf_free_connection_properties_snapshot(IntPtr properties, int count);

        /*
        *  Retrieves the null-terminated string with current connected address in format <host>:<port>. If (*address)
        *  points to NULL then connection is not connected (reconnection, no valid addresses, closed connection and others).
        *  The memory for the resulting string is allocated during execution of the function and SHOULD be free by caller
        *  with call of dxf_free function. So done because inner string with connected address can be free during reconnection.
        *
        *  connection - a handle of a previously created connection
        *  OUT address - pointer to store address of the null-terminated string with current connected address
        */
        internal abstract int dxf_get_current_connected_address(IntPtr connection, out IntPtr address);

        /*
        *  Retrieves the current connection status
        *
        *  connection - a handle of a previously created connection
        *  OUT status - connection status
        */
        internal abstract int dxf_get_current_connection_status(IntPtr connection, out ConnectionStatus status);

        /*
        * Frees memory allocated in API functions from this module
        *
        *  pointer - pointer to memory allocated earlier in some API function from this module
        */
        internal abstract int dxf_free(IntPtr pointer);

        /*
         *  Creates Regional book with the specified parameters.
         *  Regional book is like Price Level Book but uses regional data instead of full depth order book.
         *
         *  connection - a handle of a previously created connection which the subscription will be using
         *  symbol - the symbol to use
         *  OUT book - a handle of the created regional book
         */
        internal abstract int dxf_create_regional_book(IntPtr connection, string symbol, out IntPtr book);

        /*
         *  Closes a regional book.
         *  All the data associated with it will be freed.
         *
         *  book - a handle of the price level book to close
         */
        internal abstract int dxf_close_regional_book(IntPtr book);

        /*
         *  Attaches a listener callback to regional book.
         *  This callback will be invoked when price levels created from regional data change.
         *
         *  book - a handle of the book to which a listener is to be detached
         *  book_listener - a listener callback function pointer
         */
        internal abstract int dxf_attach_regional_book_listener(IntPtr book, dxf_price_level_book_listener_t book_listener, IntPtr user_data);

        /*
         *  Detaches a listener from the regional book.
         *  No error occurs if it's attempted to detach a listener which wasn't previously attached.
         *
         *  book - a handle of the book to which a listener is to be detached
         *  book_listener - a listener callback function pointer
         */
        internal abstract int dxf_detach_regional_book_listener(IntPtr book, dxf_price_level_book_listener_t book_listener);

        /*
        *  Attaches a listener callback to regional book.
        *  This callback will be invoked when new regional quotes are received.
        *
        *  book - a handle of the book to which a listener is to be detached
        *  listener - a listener callback function pointer
        */
        internal abstract int dxf_attach_regional_book_listener_v2(IntPtr book, dxf_regional_quote_listener_t listener, IntPtr user_data);
        /*
        *  Detaches a listener from the regional book.
        *  No error occurs if it's attempted to detach a listener which wasn't previously attached.
        *
        *  book - a handle of the book to which a listener is to be detached
        *  book_listener - a listener callback function pointer
        */
        internal abstract int dxf_detach_regional_book_listener_v2(IntPtr book, dxf_regional_quote_listener_t listener);
        

        /// <summary>
        /// Initializes the C-API configuration and loads a config (in TOML format) from a file
        /// 
        /// For the successful application of the configuration, this function must be called before creating any connection
        ///
        /// DXFEED_API ERRORCODE dxf_load_config_from_file(const char* file_name);
        ///
        /// The config file sample: [Sample](https://github.com/dxFeed/dxfeed-c-api/dxfeed-api-config.sample.toml)
        /// The TOML format specification: https://toml.io/en/v1.0.0-rc.2
        /// </summary>
        /// <param name="fileName">The config (in TOML format) file name</param>
        /// <returns>DXF_SUCCESS on successful logger initialization or DXF_FAILURE on error; dxf_get_last_error can
        /// be used to retrieve the error code and description in case of failure;</returns>
        internal abstract int dxf_load_config_from_file(string fileName);
        
        /// <summary>
        /// Initializes the C-API configuration and loads a config (in TOML format) from a string
        /// 
        /// For the successful application of the configuration, this function must be called before creating any connection
        ///
        /// DXFEED_API ERRORCODE dxf_load_config_from_wstring(dxf_const_string_t config);
        ///
        /// The config file sample: [Sample](https://github.com/dxFeed/dxfeed-c-api/dxfeed-api-config.sample.toml)
        /// The TOML format specification: https://toml.io/en/v1.0.0-rc.2
        /// </summary>
        /// <param name="config">The config (in TOML format) string</param>
        /// <returns>DXF_SUCCESS on successful logger initialization or DXF_FAILURE on error; dxf_get_last_error can
        /// be used to retrieve the error code and description in case of failure;</returns>
        internal abstract int dxf_load_config_from_string(string config);
        
    }
}
