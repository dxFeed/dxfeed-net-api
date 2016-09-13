using com.dxfeed.api.events;
using com.dxfeed.native;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace com.dxfeed.samples.api {
    public class DXFeedConnect {
        static void Main(string[] args) {
            if (args.Length < 2) {
                string eventTypeNames = GetEventTypeNames(typeof(IDxMarketEvent));
                Console.Error.WriteLine("usage: DXFeedConnect <types> <symbols> [<time>]");
                Console.Error.WriteLine("where: <types>   is comma-separated list of dxfeed event type (" + eventTypeNames + ")");
                Console.Error.WriteLine("       <symbols> is comma-separated list of security symbols to get events for (e.g. \"IBM,C,SPX\")");
                Console.Error.WriteLine("                 for Candle event specify symbol with aggregation like in \"IBM{=15m}\"");
                Console.Error.WriteLine("       <time>    is a fromTime for time-series subscription");
                return;
            }
            string argTypes = args[0];
            string argSymbols = args[1];
            string argTime = args.Length > 2 ? args[2] : null;

            string[] symbols = parseSymbols(argSymbols);
            try {
                foreach (string type in argTypes.Split(',')) {
                    if (argTime != null)
                        connectTimeSeriesEvent(type, argTime, symbols);
                    else
                        connectEvent(type, symbols);
                }
                Thread.Sleep(int.MaxValue);
            } catch (Exception e) {
                Console.Error.WriteLine(e.StackTrace);
                Environment.Exit(1); // shutdown on any error
            }
        }

        private static string[] parseSymbols(string symbolList) {
            List<string> result = new List<string>();
            int parentheses = 0; // # of encountered parentheses of any type
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < symbolList.Length; i++) {
                char ch = symbolList[i];
                switch (ch) {
                    case '{':
                    case '(':
                    case '[':
                        parentheses++;
                        sb.Append(ch);
                        break;
                    case '}':
                    case ')':
                    case ']':
                        if (parentheses > 0)
                            parentheses--;
                        sb.Append(ch);
                        break;
                    case ',':
                        if (parentheses == 0) {
                            // not in parenthesis -- comma is a symbol list separator
                            result.Add(sb.ToString());
                            sb.Length = 0;
                        }
                        else
                            sb.Append(ch);
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            result.Add(sb.ToString());
            return result.ToArray();
        }

        private static void connectEvent(string type, params string[] symbols) {
      //      Class <?> eventType = findEventType(type, EventType);
            //DXFeedSubscription<Object> sub = DXFeed.getInstance().createSubscription(eventType);
      //      sub.addEventListener(new PrintListener<Object>());
            //sub.addSymbols(symbols);
        }

        private static void connectTimeSeriesEvent(string type, string fromTime, params string[] symbols)
        {
        //TODO:
        //      Class < TimeSeriesEvent <?>> eventType = findEventType(type, TimeSeriesEvent.class);
            //long from = TimeFormat.DEFAULT.parse(fromTime).getTime();
        //  DXFeedTimeSeriesSubscription<TimeSeriesEvent<?>> sub = DXFeed.getInstance().createTimeSeriesSubscription(eventType);
        //  sub.addEventListener(new PrintListener<TimeSeriesEvent<?>>());
            //sub.setFromTime(from);
            //sub.addSymbols(symbols);
        }

        private class PrintListener<E> : DXFeedEventListener<E> {
            public void EventsReceived(IList<E> events) {
            foreach (E e in events)
                Console.WriteLine(e);
            }
        }

        // ---- Utility methods to make this sample generic for use with any event type as specified on command line ----

        public static string GetEventTypeNames(Type baseClass) {
            StringBuilder sb = new StringBuilder();
            foreach (string s in GetEventTypesMap(baseClass).Keys) {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(s);
            }
            return sb.ToString();
        }


        public static Type FindEventType(string type, Type baseClass) {
            Type result;
            if (!GetEventTypesMap(baseClass).TryGetValue(type, out result))
                throw new ArgumentException("Cannot find " + baseClass.Name + " '" + type + "'");
            return result;
        }

        private static Dictionary<string, Type> GetEventTypesMap(Type baseClass) {
            Dictionary<string, Type> result = new Dictionary<string, Type>();
            foreach (Type eventType in DXEndpoint.GetInstance().GetEventTypes()) {
                if (!baseClass.IsAssignableFrom(eventType))
                    continue;
                EventTypeAttribute attr = Attribute.GetCustomAttribute(eventType, typeof(EventTypeAttribute)) as EventTypeAttribute;
                result[attr.EventName] = eventType;
            }
            return result;
        }

    }
}
