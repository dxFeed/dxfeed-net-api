#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using com.dxfeed.api;
using com.dxfeed.api.connection;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native;

namespace dxf_simple_order_book_sample
{
    public class OrderListener : IDxOrderSnapshotListener
    {
        private readonly int recordsPrintLimit;

        public OrderListener(int recordsPrintLimit)
        {
            this.recordsPrintLimit = recordsPrintLimit;
        }

        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "{3}: Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}}}", buf.EventType, buf.Symbol, buf.Size,
                DateTime.Now.ToString("o")));

            var book = buf.Select(o => new Offer
                {
                    Side = o.Side,
                    Price = o.Price,
                    Size = o.Size,
                    Timestamp = o.Time,
                    Sequence = o.Sequence,
                    Source = o.Source?.Name,
                    MarketMaker = o.MarketMaker
                })
                .ToList();

            Console.Write("Bids:\n");
            var bids = book.Where(o => o.Side == Side.Buy).OrderByDescending(o => o.Price).Take(recordsPrintLimit == 0 ? int.MaxValue : recordsPrintLimit);

            foreach (var o in bids)
                Console.WriteLine($"{o.Price} {o.Size}");

            Console.WriteLine();

            Console.Write("Asks:\n");
            var asks = book.Where(o => o.Side == Side.Sell).OrderBy(o => o.Price).Take(recordsPrintLimit == 0 ? int.MaxValue : recordsPrintLimit);
            foreach (var o in asks)
                Console.WriteLine($"{o.Price} {o.Size}");
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class Offer
        {
            public Side Side { get; set; }
            public double Price { get; set; }
            public DateTime Timestamp { get; set; }
            public long Size { get; set; }
            public long Sequence { get; set; }
            public string Source { get; set; }
            public string MarketMaker { get; set; }
        }
    }

    internal class InputParam<T>
    {
        private T value;

        private InputParam()
        {
            IsSet = false;
        }

        public InputParam(T defaultValue) : this()
        {
            value = defaultValue;
        }

        public bool IsSet { get; private set; }

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
                IsSet = true;
            }
        }
    }

    internal class Program
    {
        private const int DefaultRecordsPrintLimit = 10;
        private const int HostIndex = 0;
        private const int SymbolIndex = 1;

        private static bool TryParseDateTimeParam(string stringParam, InputParam<DateTime?> param)
        {
            DateTime dateTimeValue;

            if (!DateTime.TryParse(stringParam, out dateTimeValue))
            {
                return false;
            }

            param.Value = dateTimeValue;

            return true;
        }

        private static bool TryParseRecordsPrintLimitParam(string paramTagString, string paramString,
            InputParam<int> param)
        {
            if (!paramTagString.Equals("-l"))
            {
                return false;
            }

            int newRecordsPrintLimit;

            if (!int.TryParse(paramString, out newRecordsPrintLimit))
            {
                return false;
            }

            param.Value = newRecordsPrintLimit;

            return true;
        }

        private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
            InputParam<string> param)
        {
            if (!paramTagString.Equals(tag))
            {
                return false;
            }

            param.Value = paramString;

            return true;
        }

        private static void ConnectionStatusChangeHandler(IDxConnection connection, ConnectionStatus oldStatus,
            ConnectionStatus newStatus)
        {
            if (newStatus == ConnectionStatus.Connected)
            {
                Console.WriteLine("Connected to {0}", connection.ConnectedAddress);
            }
            else if (newStatus == ConnectionStatus.Authorized)
            {
                Console.WriteLine("Authorized");
            }
        }

        private static void DisconnectHandler(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        private static void ShowUsage()
        {
            Console.WriteLine(
                "Usage: dxf_simple_order_book_sample <host:port> <symbol> [<date>] <source> [-l <records_print_limit>] [-T <token>] [-p]\n" +
                "where\n" +
                "    host:port - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                "    symbol    - IBM, MSFT, AAPL, ...\n" +
                "    date      - The date of time series event in the format YYYY-MM-DD\n" +
                "    source    - Source for order events (default: NTV):\n" +
                "                NTV,NFX,ESPD,XNFI,ICE,ISE,DEA,DEX,BYX,BZX,BATE,CHIX,CEUX,BXTR,\n" +
                "                IST,BI20,ABE,FAIR,GLBX,ERIS,XEUR,CFE,C2OX,SMFE...\n" +
                "    -l <records_print_limit> - The number of displayed bids or asks in a book\n" +
                $"                               (0 - unlimited, default: {DefaultRecordsPrintLimit})\n" +
                "    -T <token>               - The authorization token\n" +
                "    -p                       - Enables the data transfer logging\n\n" +
                "examples:\n" +
                "  dxf_simple_order_book_sample demo.dxfeed.com:7300 IBM NTV\n" +
                "  dxf_simple_order_book_sample demo.dxfeed.com:7300 IBM 2020-03-31 NTV\n" +
                "  dxf_simple_order_book_sample demo.dxfeed.com:7300 IBM 2020-03-31 NTV -l 0\n");
        }

        private static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length > 9)
            {
                ShowUsage();

                return;
            }

            var address = args[HostIndex];
            var symbol = args[SymbolIndex];
            var source = new InputParam<string>(OrderSource.NTV.Name);
            var dateTime = new InputParam<DateTime?>(null);
            var recordsPrintLimit = new InputParam<int>(DefaultRecordsPrintLimit);
            var token = new InputParam<string>(null);
            var logDataTransferFlag = false;

            for (var i = SymbolIndex + 1; i < args.Length; i++)  {
                if (!dateTime.IsSet && TryParseDateTimeParam(args[i], dateTime)) {
                    continue;
                }

                if (!recordsPrintLimit.IsSet && i < args.Length - 1 &&
                    TryParseRecordsPrintLimitParam(args[i], args[i + 1], recordsPrintLimit))
                {
                    i++;

                    continue;
                }

                if (!token.IsSet && i < args.Length - 1 &&
                    TryParseTaggedStringParam("-T", args[i], args[i + 1], token)) {
                    i++;

                    continue;
                }

                if (logDataTransferFlag == false && args[i].Equals("-p")) {
                    logDataTransferFlag = true;
                    i++;

                    continue;
                }

                if (!source.IsSet) {
                    source.Value = args[i];
                }
            }

            Console.WriteLine(
                $"Connecting to {address} for [Order#{source.Value} (book)] on [{symbol}] ...");

            using (var con = token.IsSet
                ? new NativeConnection(address, token.Value, DisconnectHandler, ConnectionStatusChangeHandler)
                : new NativeConnection(address, DisconnectHandler, ConnectionStatusChangeHandler))
            {
                IDxSubscription s = null;
                try
                {
                    NativeTools.InitializeLogging("dxf_simple_order_book_sample.log", true, true, logDataTransferFlag);
                    s = con.CreateSnapshotSubscription(EventType.Order, dateTime.Value, new OrderListener(recordsPrintLimit.Value));
                    s.SetSource(source.Value);
                    s.AddSymbols(symbol);

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                }
                catch (DxException dxException)
                {
                    Console.WriteLine($"Native exception occured: {dxException.Message}");
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Exception occured: {exc.Message}");
                }
                finally
                {
                    s?.Dispose();
                }
            }
        }
    }
}