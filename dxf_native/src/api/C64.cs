using System;
using System.Runtime.InteropServices;
using com.dxfeed.api.events;

namespace com.dxfeed.native.api {
	internal class C64 : C {
#if DEBUG
		private const string DXFEED_DLL = "DXFeedd_64.dll";
#else
		private const string DXFEED_DLL = "DXFeed_64.dll";
#endif

        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_initialize_logger")]
        private static extern int __dxf_initialize_logger(string file_name, bool rewrite_file, bool show_time_zone_info, bool verbose);
        internal override int dxf_initialize_logger(string file_name, bool rewrite_file, bool show_time_zone_info, bool verbose)
        {
            return __dxf_initialize_logger(file_name, rewrite_file, show_time_zone_info, verbose);
        }


        [DllImport(DXFEED_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_connection")]
        private static extern int __dxf_create_connection(
            string address,
            dxf_conn_termination_notifier_t notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection);
        internal override int dxf_create_connection(
            string address,
            dxf_conn_termination_notifier_t notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection)
        {
            return __dxf_create_connection(address, notifier, stcn, stdn, user_data, out connection);
        }


        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_close_connection")]
        private static extern int __dxf_close_connection(IntPtr connection);
        internal override int dxf_close_connection(IntPtr connection)
        {
            return __dxf_close_connection(connection);
        }


        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_subscription")]
        private static extern int __dxf_create_subscription(IntPtr connection, EventType event_types, out IntPtr subscription);
        internal override int dxf_create_subscription(IntPtr connection, EventType event_types, out IntPtr subscription)
        {
            return __dxf_create_subscription(connection, event_types, out subscription);
        }

        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_subscription_timed")]
        private static extern int __dxf_create_subscription_timed(IntPtr connection, EventType event_types, Int64 time, out IntPtr subscription);
        internal override int dxf_create_subscription_timed(IntPtr connection, EventType event_types, Int64 time, out IntPtr subscription)
        {
            return __dxf_create_subscription_timed(connection, event_types, time, out subscription);
        }



        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_close_subscription")]
        private static extern int __dxf_close_subscription(IntPtr subscription);
        internal override int dxf_close_subscription(IntPtr subscription)
        {
            return __dxf_close_subscription(subscription);
        }


        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_add_symbol")]
        private static extern int __dxf_add_symbol(IntPtr subscription, String symbol);
        internal override int dxf_add_symbol(IntPtr subscription, String symbol)
        {
            return __dxf_add_symbol(subscription, symbol);
        }


        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_add_symbols")]
        private static extern int __dxf_add_symbols(IntPtr subscription, string[] symbols, int count);
        internal override int dxf_add_symbols(IntPtr subscription, string[] symbols, int count)
        {
            return __dxf_add_symbols(subscription, symbols, count);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_add_candle_symbol")]
        private static extern int __dxf_add_candle_symbol(IntPtr subscription, IntPtr candle_attributes);
        internal override int dxf_add_candle_symbol(IntPtr subscription, IntPtr candle_attributes)
        {
            return __dxf_add_candle_symbol(subscription, candle_attributes);
        }


        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_remove_symbol")]
        private static extern int __dxf_remove_symbol(IntPtr subcription, string symbol);
        internal override int dxf_remove_symbol(IntPtr subcription, string symbol)
        {
            return __dxf_remove_symbol(subcription, symbol);
        }


        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_remove_symbols")]
        private static extern int __dxf_remove_symbols(IntPtr subscription, string[] symbols, int count);
        internal override int dxf_remove_symbols(IntPtr subscription, string[] symbols, int count)
        {
            return __dxf_remove_symbols(subscription, symbols, count);
        }


        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_get_symbols")]
        private static extern int __dxf_get_symbols(IntPtr subscription, out IntPtr symbols, out int count);
        internal override int dxf_get_symbols(IntPtr subscription, out IntPtr symbols, out int count)
        {
            return __dxf_get_symbols(subscription, out symbols, out count);
        }


        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_set_symbols")]
        private static extern int __dxf_set_symbols(IntPtr subscription, string[] symbols, int count);
        internal override int dxf_set_symbols(IntPtr subscription, string[] symbols, int count)
        {
            return __dxf_set_symbols(subscription, symbols, count);
        }


        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_clear_symbols")]
        private static extern int __dxf_clear_symbols(IntPtr subscription);
        internal override int dxf_clear_symbols(IntPtr subscription)
        {
            return __dxf_clear_symbols(subscription);
        }


        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_attach_event_listener")]
        private static extern int __dxf_attach_event_listener(IntPtr subscription, dxf_event_listener_t event_listener,
                                                             IntPtr user_data);
        internal override int dxf_attach_event_listener(IntPtr subscription, dxf_event_listener_t event_listener,
                                                             IntPtr user_data)
        {
            return __dxf_attach_event_listener(subscription, event_listener, user_data);
        }

        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_detach_event_listener")]
        private static extern int __dxf_detach_event_listener(IntPtr subscription, dxf_event_listener_t listener);
        internal override int dxf_detach_event_listener(IntPtr subscription, dxf_event_listener_t listener)
        {
            return __dxf_detach_event_listener(subscription, listener);
        }


        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_get_subscription_event_types")]
        private static extern int __dxf_get_subscription_event_types(IntPtr subscription, out int event_types);
        internal override int dxf_get_subscription_event_types(IntPtr subscription, out int event_types)
        {
            return __dxf_get_subscription_event_types(subscription, out event_types);
        }


        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_get_last_event")]
        private static extern int __dxf_get_last_event(IntPtr connection, int event_type, string symbol, IntPtr event_data);
        internal override int dxf_get_last_event(IntPtr connection, int event_type, string symbol, IntPtr event_data)
        {
            return __dxf_get_last_event(connection, event_type, symbol, event_data);
        }


        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_get_last_error")]
        private static extern int __dxf_get_last_error(out int error_code, out IntPtr error_descr);
        internal override int dxf_get_last_error(out int error_code, out IntPtr error_descr)
        {
            return __dxf_get_last_error(out error_code, out error_descr);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_set_order_source")]
        private static extern int __dxf_set_order_source(IntPtr subscription, byte[] source);
        internal override int dxf_set_order_source(IntPtr subscription, byte[] source)
        {
            return __dxf_set_order_source(subscription, source);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_add_order_source")]
        private static extern int __dxf_add_order_source(IntPtr subscription, byte[] source);
        internal override int dxf_add_order_source(IntPtr subscription, byte[] source)
        {
            return __dxf_add_order_source(subscription, source);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_candle_symbol_attributes")]
        private static extern int __dxf_create_candle_symbol_attributes(string base_symbol, char exchange_code, double period_value, int period_type,
                                                                        int price, int session, int alignment, out IntPtr candle_attributes);
        internal override int dxf_create_candle_symbol_attributes(string base_symbol, char exchange_code, double period_value, int period_type,
                                                                        int price, int session, int alignment, out IntPtr candle_attributes)
        {
            return __dxf_create_candle_symbol_attributes(base_symbol, exchange_code, period_value, period_type, price, session, alignment, out candle_attributes);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_delete_candle_symbol_attributes")]
        private static extern int __dxf_delete_candle_symbol_attributes(IntPtr candle_attributes);
        internal override int dxf_delete_candle_symbol_attributes(IntPtr candle_attributes)
        {
            return __dxf_delete_candle_symbol_attributes(candle_attributes);
        }
    }
}