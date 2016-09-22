/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;
using com.dxfeed.api.events;

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
        /// Helper method to check error codes 
        /// Throws NativeDxException if return_code != DX_OK
        /// </summary>
        /// <param name="returnCode"></param>
        /// <exception cref="NativeDxException"></exception>
        internal static void CheckOk(int returnCode)
        {
            if (returnCode != DX_OK)
                throw NativeDxException.Create();
        }

        /*
         *	Event listener prototype
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
        internal delegate void dxf_event_listener_v2_t(EventType event_type, IntPtr symbol, IntPtr data, int data_count, DxEventParams event_params, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dxf_conn_termination_notifier_t(IntPtr connection, IntPtr user_data);

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

        /*
         *	Initializes the internal logger.
         *  Various actions and events, including the errors, are being logged throughout the library. They may be stored
         *  into the file.
         *  
         *  param file_name - a full path to the file where the log is to be stored
         *  rewrite_file - a flag defining the file open mode; if it's nonzero then the log file will be rewritten
         *  show_timezone_info - a flag defining the time display option in the log file; if it's nonzero then
         *					 the time will be displayed with the timezone suffix
         *  verbose - a flag defining the logging mode; if it's nonzero then the verbose logging will be enabled
         */
        //DXFEED_API ERRORCODE dxf_initialize_logger (const char* file_name, int rewrite_file, int show_timezone_info, int verbose);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_initialize_logger(string file_name, bool rewrite_file, bool show_time_zone_info, bool verbose);

        /*
         *	Creates connection with the specified parameters.
         *
         *  address - "[host[:port],]host:port"
         *  notifier - the callback to inform the client side that the connection has stumbled upon and error and will go reconnecting
         *  stcn - the callback for informing the client side about the socket thread creation;
         *         may be set to NULL if no specific action is required to perform on the client side on a new thread creation
         *  shdn - the callback for informing the client side about the socket thread destruction;
         *         may be set to NULL if no specific action is required to perform on the client side on a thread destruction
         *  user_data - the user defined value passed to the termination notifier callback along with the connection handle; may be set
         *              to whatever value
         *  OUT connection - the handle of the created connection
         */
        // DXFEED_API ERRORCODE dxf_create_connection (const char* address,
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_create_connection(
            string address,
            dxf_conn_termination_notifier_t notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection);

        /*
         *	Closes a connection.
         *  connection - a handle of a previously created connection
         */
        //DXFEED_API ERRORCODE dxf_close_connection (dxf_connection_t connection);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_close_connection(IntPtr connection);

        /*
         *	Creates a subscription with the specified parameters.
         *	
         *  connection - a handle of a previously created connection which the subscription will be using
         *  event_types - a bitmask of the subscription event types. See 'dx_event_id_t' and 'DX_EVENT_BIT_MASK'
         *				for information on how to create an event type bitmask
         *  OUT subscription - a handle of the created subscription
         */
        //DXFEED_API ERRORCODE dxf_create_subscription (dxf_connection_t connection, int event_types,
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_create_subscription(IntPtr connection, EventType event_types, out IntPtr subscription);

        /*
         *	Creates a subscription with the specified parameters.

         *  connection - a handle of a previously created connection which the subscription will be using
         *  event_types - a bitmask of the subscription event types. See 'dx_event_id_t' and 'DX_EVENT_BIT_MASK'
         *                for information on how to create an event type bitmask
         *  time - time in the past (unix time in milliseconds)
         *  OUT subscription - a handle of the created subscription
         */
        internal abstract int dxf_create_subscription_timed(IntPtr connection, EventType event_types,
                                                            Int64 time, out IntPtr subscription);

        /*
         *	Closes a subscription.
         *  All the data associated with it will be freed.
         *  
         *  subscription - a handle of the subscription to close
         */
        //DXFEED_API ERRORCODE dxf_close_subscription (dxf_subscription_t subscription);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_close_subscription(IntPtr subscription);

        /*
         *	Adds a single symbol to the subscription.
         *	
         *  subscription - a handle of the subscription to which a symbol is added
         *  symbol - the symbol to add
         */
        //DXFEED_API ERRORCODE dxf_add_symbol (dxf_subscription_t subscription, dxf_const_string_t symbol);
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_add_symbol(IntPtr subscription, String symbol);

        /*
         *	Adds several symbols to the subscription.
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
         *	Adds a candle symbol to the subscription.
 
         *  subscription - a handle of the subscription to which a symbol is added
         *  candle_attributes - pointer to the candle struct
         */
        internal abstract int dxf_add_candle_symbol(IntPtr subscription, IntPtr candle_attributes);

        /*
         *	Remove a candle symbol from the subscription.

         *  subscription - a handle of the subscription from symbol will be removed
         *  candle_attributes - pointer to the candle struct
        */
        internal abstract int dxf_remove_candle_symbol(IntPtr subscription, IntPtr candle_attributes);

        /*
         *	Removes a single symbol from the subscription.
         *	
         *  subscription - a handle of the subscription from which a symbol is removed
         *  symbol - the symbol to remove
         */
        //DXFEED_API ERRORCODE dxf_remove_symbol (dxf_subscription_t subscription, dxf_const_string_t symbol);
        //[DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_remove_symbol(IntPtr subcription, string symbol);

        /*
         *	Removes several symbols from the subscription.
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
         *	Retrieves the list of symbols currently added to the subscription.
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
         *	Sets the symbols for the subscription.
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
         *	Removes all the symbols from the subscription.
         * 
         *  subscription - a handle of the subscription whose symbols are to be cleared
         */
        //DXFEED_API ERRORCODE dxf_clear_symbols (dxf_subscription_t subscription);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_clear_symbols(IntPtr subscription);

        /*
         *	Attaches a listener callback to the subscription.
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
         *	Detaches a listener from the subscription.
         *  No error occurs if it's attempted to detach a listener which wasn't previously attached.
         *
         *  subscription - a handle of the subscription from which a listener is to be detached
         *  event_listener - a listener callback function pointer
         */
        //DXFEED_API ERRORCODE dxf_detach_event_listener (dxf_subscription_t subscription, dxf_event_listener_t event_listener);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_detach_event_listener(IntPtr subscription, dxf_event_listener_t listener);

        /*
         *	Attaches a extended listener callback to the subscription.
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
         *	Detaches a extended listener from the subscription.
         *  No error occurs if it's attempted to detach a listener which wasn't previously attached.
         *
         *  subscription - a handle of the subscription from which a listener is to be detached
         *  event_listener - a listener callback function pointer
         */
        internal abstract int dxf_detach_event_listener_v2(IntPtr subscription, dxf_event_listener_v2_t listener);

        /*
         *	Retrieves the subscription event types.
         *
         *  subscription - a handle of the subscription whose event types are to be retrieved
         *  OUT event_types - a pointer to the variable to which the subscription event types bitmask is to be stored
         */
        //DXFEED_API ERRORCODE dxf_get_subscription_event_types (dxf_subscription_t subscription, OUT int* event_types);
        //[DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal abstract int dxf_get_subscription_event_types(IntPtr subscription, out int event_types);

        /*
         *	Retrieves the last event data of the specified symbol and type for the connection.
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
         *	Retrieves the last error info.
         *  The error is stored on per-thread basis. If the connection termination notifier callback was invoked, then
         *  to retrieve the connection's error code call this function right from the callback function.
         *  If an error occurred within the error storage subsystem itself, the function will return DXF_FAILURE.
         *
         *  OUT error_code - a pointer to the variable where the error code is to be stored
         *  OUT error_descr - a pointer to the variable where the human-friendly error description is to be stored;
         *					  may be NULL if the text representation of error is not required
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

        /*
         *	API that allows user to create candle symbol attributes
         *
         *  base_symbol - the symbols to add
         *  exchange_code - exchange attribute of this symbol
         *  period_value -  aggregation period value of this symbol
         *  period_type - aggregation period type of this symbol
         *  price - price type attribute of this symbol
         *  session - session attribute of this symbol
         *  alignment - alignment attribute of this symbol
         *  candle_attributes - pointer to the configured candle attributes struct
         */
        internal abstract int dxf_create_candle_symbol_attributes(string base_symbol,
                                                                 char exchange_code,
                                                                 double period_value,
                                                                 int period_type,
                                                                 int price,
                                                                 int session,
                                                                 int alignment,
                                                                 out IntPtr candle_attributes);

        /*
         *	Free memory allocated by dxf_initialize_candle_symbol_attributes(...) function

         *  candle_attributes - pointer to the candle attributes struct
         */
        internal abstract int dxf_delete_candle_symbol_attributes(IntPtr candle_attributes);

        /*
         *  Creates Order snapshot with the specified parameters.
         *
         *  If source is NULL string subscription on Order event will be performed. You can specify order 
         *  source for Order event by passing suffix: "BYX", "BZX", "DEA", "DEX", "ISE", "IST", "NTV".
         *  If source is equal to "COMPOSITE_BID" or "COMPOSITE_ASK" subscription on MarketMaker event will 
         *  be performed.
         *
         *  connection - a handle of a previously created connection which the subscription will be using
         *  symbol - the symbol to add
         *  source - order source for Order event with 4 symbols maximum length OR keyword which can be 
         *           one of COMPOSITE_BID or COMPOSITE_ASK
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
         *  Detaches a listener from the snapshot.
         *  No error occurs if it's attempted to detach a listener which wasn't previously attached.
         *
         *  snapshot - a handle of the snapshot to which a listener is to be detached
         *  snapshot_listener - a listener callback function pointer
         */
        internal abstract int dxf_detach_snapshot_listener(IntPtr snapshot, dxf_snapshot_listener_t snapshotListener);

        /*
         *  Retrieves the symbol currently added to the snapshot subscription.
         *  The memory for the resulting symbol is allocated internally, so no actions to free it are required.
         *
         *  snapshot - a handle of the snapshot to which a listener is to be detached
         *  OUT symbol - a pointer to the string object to which the symbol is to be stored
         */
        internal abstract int dxf_get_snapshot_symbol(IntPtr snapshot, out IntPtr symbol);
    }
}
