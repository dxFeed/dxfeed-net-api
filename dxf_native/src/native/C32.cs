#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System;
using System.Runtime.InteropServices;
using com.dxfeed.api.connection;
using com.dxfeed.api.events;
using com.dxfeed.api.data;

namespace com.dxfeed.native.api
{
    internal class C32 : C
    {
#if DEBUG
        private const string DXFEED_DLL = "DXFeedd.dll";
#else
        private const string DXFEED_DLL = "DXFeed.dll";
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
            dxf_conn_status_notifier_t conn_status_notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection);
        internal override int dxf_create_connection(
            string address,
            dxf_conn_termination_notifier_t notifier,
            dxf_conn_status_notifier_t conn_status_notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection)
        {
            return __dxf_create_connection(address, notifier, conn_status_notifier, stcn, stdn, user_data, out connection);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_connection_auth_basic")]
        private static extern int __dxf_create_connection_auth_basic(
            string address,
            string user,
            string password,
            dxf_conn_termination_notifier_t notifier,
            dxf_conn_status_notifier_t conn_status_notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection);
        internal override int dxf_create_connection_auth_basic(
            string address,
            string user,
            string password,
            dxf_conn_termination_notifier_t notifier,
            dxf_conn_status_notifier_t conn_status_notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection)
        {
            return __dxf_create_connection_auth_basic(address, user, password, notifier, conn_status_notifier, stcn, stdn, user_data, out connection);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_connection_auth_bearer")]
        private static extern int __dxf_create_connection_auth_bearer(
            string address,
            string token,
            dxf_conn_termination_notifier_t notifier,
            dxf_conn_status_notifier_t conn_status_notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection);
        internal override int dxf_create_connection_auth_bearer(
            string address,
            string token,
            dxf_conn_termination_notifier_t notifier,
            dxf_conn_status_notifier_t conn_status_notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection)
        {
            return __dxf_create_connection_auth_bearer(address, token, notifier, conn_status_notifier, stcn, stdn, user_data, out connection);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_connection_auth_custom")]
        private static extern int __dxf_create_connection_auth_custom(
            string address,
            string authscheme,
            string authdata,
            dxf_conn_termination_notifier_t notifier,
            dxf_conn_status_notifier_t conn_status_notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection);
        internal override int dxf_create_connection_auth_custom(
            string address,
            string authscheme,
            string authdata,
            dxf_conn_termination_notifier_t notifier,
            dxf_conn_status_notifier_t conn_status_notifier,
            dxf_socket_thread_creation_notifier_t stcn,
            dxf_socket_thread_destruction_notifier_t stdn,
            IntPtr user_data,
            out IntPtr connection)
        {
            return __dxf_create_connection_auth_custom(address, authscheme, authdata, notifier, conn_status_notifier, stcn, stdn, user_data, out connection);
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

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_remove_candle_symbol")]
        private static extern int __dxf_remove_candle_symbol(IntPtr subscription, IntPtr candle_attributes);
        internal override int dxf_remove_candle_symbol(IntPtr subscription, IntPtr candle_attributes)
        {
            return __dxf_remove_candle_symbol(subscription, candle_attributes);
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

        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_attach_event_listener_v2")]
        private static extern int __dxf_attach_event_listener_v2(IntPtr subscription, dxf_event_listener_v2_t event_listener,
                                                             IntPtr user_data);
        internal override int dxf_attach_event_listener_v2(IntPtr subscription, dxf_event_listener_v2_t event_listener,
                                                             IntPtr user_data)
        {
            return __dxf_attach_event_listener_v2(subscription, event_listener, user_data);
        }

        [DllImport(DXFEED_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_detach_event_listener_v2")]
        private static extern int __dxf_detach_event_listener_v2(IntPtr subscription, dxf_event_listener_v2_t listener);
        internal override int dxf_detach_event_listener_v2(IntPtr subscription, dxf_event_listener_v2_t listener)
        {
            return __dxf_detach_event_listener_v2(subscription, listener);
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

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_snapshot")]
        private static extern int __dxf_create_snapshot(IntPtr connection, int event_id,
                                                        string symbol, byte[] source,
                                                        Int64 time, out IntPtr snapshot);
        internal override int dxf_create_snapshot(IntPtr connection, int event_id,
                                                  string symbol, byte[] source,
                                                  Int64 time, out IntPtr snapshot)
        {
            return __dxf_create_snapshot(connection, event_id, symbol, source, time, out snapshot);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_order_snapshot")]
        private static extern int __dxf_create_order_snapshot(IntPtr connection, string symbol,
                                                byte[] source, Int64 time, out IntPtr snapshot);
        internal override int dxf_create_order_snapshot(IntPtr connection, string symbol,
                                                byte[] source, Int64 time, out IntPtr snapshot)
        {
            return __dxf_create_order_snapshot(connection, symbol, source, time, out snapshot);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_candle_snapshot")]
        private static extern int __dxf_create_candle_snapshot(IntPtr connection,
                                                               IntPtr candle_attributes,
                                                               Int64 time, out IntPtr snapshot);
        internal override int dxf_create_candle_snapshot(IntPtr connection,
                                                         IntPtr candle_attributes,
                                                         Int64 time, out IntPtr snapshot)
        {
            return __dxf_create_candle_snapshot(connection, candle_attributes, time, out snapshot);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_close_snapshot")]
        private static extern int __dxf_close_snapshot(IntPtr snapshot);
        internal override int dxf_close_snapshot(IntPtr snapshot)
        {
            return __dxf_close_snapshot(snapshot);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_attach_snapshot_listener")]
        private static extern int __dxf_attach_snapshot_listener(IntPtr snapshot, dxf_snapshot_listener_t snapshotListener,
                                                           IntPtr userData);
        internal override int dxf_attach_snapshot_listener(IntPtr snapshot, dxf_snapshot_listener_t snapshotListener,
                                                           IntPtr userData)
        {
            return __dxf_attach_snapshot_listener(snapshot, snapshotListener, userData);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_detach_snapshot_listener")]
        private static extern int __dxf_detach_snapshot_listener(IntPtr snapshot, dxf_snapshot_listener_t snapshotListener);
        internal override int dxf_detach_snapshot_listener(IntPtr snapshot, dxf_snapshot_listener_t snapshotListener)
        {
            return __dxf_detach_snapshot_listener(snapshot, snapshotListener);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_get_snapshot_symbol")]
        private static extern int __dxf_get_snapshot_symbol(IntPtr snapshot, out IntPtr symbol);
        internal override int dxf_get_snapshot_symbol(IntPtr snapshot, out IntPtr symbol)
        {
            return __dxf_get_snapshot_symbol(snapshot, out symbol);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_write_raw_data")]
        private static extern int __dxf_write_raw_data(IntPtr connection, byte[] raw_file_name);
        internal override int dxf_write_raw_data(IntPtr connection, byte[] raw_file_name)
        {
            return __dxf_write_raw_data(connection, raw_file_name);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_get_connection_properties_snapshot")]
        private static extern int __dxf_get_connection_properties_snapshot(IntPtr connection, out IntPtr properties, out int count);
        internal override int dxf_get_connection_properties_snapshot(IntPtr connection, out IntPtr properties, out int count)
        {
            return __dxf_get_connection_properties_snapshot(connection, out properties, out count);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_free_connection_properties_snapshot")]
        private static extern int __dxf_free_connection_properties_snapshot(IntPtr properties, int count);
        internal override int dxf_free_connection_properties_snapshot(IntPtr properties, int count)
        {
            return __dxf_free_connection_properties_snapshot(properties, count);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_get_current_connected_address")]
        private static extern int __dxf_get_current_connected_address(IntPtr connection, out IntPtr address);
        internal override int dxf_get_current_connected_address(IntPtr connection, out IntPtr address)
        {
            return __dxf_get_current_connected_address(connection, out address);
        }
        
        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_get_current_connection_status")]
        private static extern int __dxf_get_current_connection_status(IntPtr connection, out ConnectionStatus status);

        internal override int dxf_get_current_connection_status(IntPtr connection, out ConnectionStatus status) 
        {
            return __dxf_get_current_connection_status(connection, out status);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_free")]
        private static extern int __dxf_free(IntPtr pointer);
        internal override int dxf_free(IntPtr pointer)
        {
            return __dxf_free(pointer);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_create_regional_book")]
        private static extern int __dxf_create_regional_book(IntPtr connection, string symbol, out IntPtr book);
        internal override int dxf_create_regional_book(IntPtr connection, string symbol, out IntPtr book)
        {
            return __dxf_create_regional_book(connection, symbol, out book);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_close_regional_book")]
        private static extern int __dxf_close_regional_book(IntPtr book);
        internal override int dxf_close_regional_book(IntPtr book)
        {
            return __dxf_close_regional_book(book);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_attach_regional_book_listener")]
        private static extern int __dxf_attach_regional_book_listener(IntPtr book, dxf_price_level_book_listener_t book_listener, IntPtr user_data);
        internal override int dxf_attach_regional_book_listener(IntPtr book, dxf_price_level_book_listener_t book_listener, IntPtr user_data)
        {
            return __dxf_attach_regional_book_listener(book, book_listener, user_data);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_detach_regional_book_listener")]
        private static extern int __dxf_detach_regional_book_listener(IntPtr book, dxf_price_level_book_listener_t book_listener);
        internal override int dxf_detach_regional_book_listener(IntPtr book, dxf_price_level_book_listener_t book_listener)
        {
            return __dxf_detach_regional_book_listener(book, book_listener);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_attach_regional_book_listener_v2")]
        private static extern int __dxf_attach_regional_book_listener_v2(IntPtr book, dxf_regional_quote_listener_t listener, IntPtr user_data);
        internal override int dxf_attach_regional_book_listener_v2(IntPtr book, dxf_regional_quote_listener_t listener, IntPtr user_data)
        {
            return __dxf_attach_regional_book_listener_v2(book, listener, user_data);
        }

        [DllImport(DXFEED_DLL, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dxf_detach_regional_book_listener_v2")]
        private static extern int __dxf_detach_regional_book_listener_v2(IntPtr book, dxf_regional_quote_listener_t listener);
        internal override int dxf_detach_regional_book_listener_v2(IntPtr book, dxf_regional_quote_listener_t listener)
        {
            return __dxf_detach_regional_book_listener_v2(book, listener);
        }
    }
}
