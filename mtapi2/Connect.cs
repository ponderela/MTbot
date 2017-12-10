using MtApi;
using MtApi.Monitors;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MT4bot;
using System.Drawing;

namespace mtapi2
{
    class Connect
    {
        readonly List<Action> _groupOrderCommands = new List<Action>();
        readonly MtApiClient _apiClient = new MtApiClient();
        readonly TimerTradeMonitor _timerTradeMonitor;
        readonly TimeframeTradeMonitor _timeframeTradeMonitor;
        String previous = "";
        int counter = 0;

        public Connect()
        {
            //_timerTradeMonitor = new TimerTradeMonitor(_apiClient) { Interval = 10000 }; // 10 sec     

            //_timeframeTradeMonitor = new TimeframeTradeMonitor(_apiClient);

            Connect_Click();
        }

        private void Connect_Click()
        {

            //_timerTradeMonitor.Start();
            //_timeframeTradeMonitor.Start();
            _apiClient.BeginConnect(8222);
            apiClient_ConnectionStateChanged();

        }

        private void apiClient_ConnectionStateChanged()
        {
            while (true)
            {
                var temp = _apiClient.ConnectionState;
                if (temp.ToString() =="Connected")
                {
                    Console.WriteLine("Connected at : {0}",DateTime.Now.ToString());
                    Interval();

                }
                else
                {
                    Console.WriteLine("Connecting");
                }

                Thread.Sleep(10000);
            }
        }

        public async void buy()
        {
            String symbol = "GBPUSD";
            double volume = 0.01;
            int slippage = 1;
            double sl_volume = 0.001;
            double tp_volume = 0.002;

            var result = await Execute(() => _apiClient.SymbolInfoTick(symbol));
            double current = result.Ask;


            // var ticket = await Execute(() => _apiClient.OrderSendBuy(symbol, volume, slippage, current - sl_volume, current + tp_volume));
            var ticket = await Execute(() => _apiClient.OrderSendBuy(symbol, volume, slippage));
            Console.WriteLine("Perform BUY Order at {0}",DateTime.Now.ToString());
        }

        public async void sell()
        {
            String symbol = "GBPUSD";
            double volume = 0.01;
            int slippage = 1;
            double sl_volume = 0.001;
            double tp_volume = 0.002;

            var result = await Execute(() => _apiClient.SymbolInfoTick(symbol));
            double current = result.Bid;


            //var ticket = await Execute(() => _apiClient.OrderSendSell(symbol, volume, slippage, current + sl_volume, current - tp_volume));
            var ticket = await Execute(() => _apiClient.OrderSendSell(symbol, volume, slippage));
            Console.WriteLine("Perform SELL Order at {0}", DateTime.Now.ToString());
        }

        public async void CloseAll()
        {
            Console.WriteLine("Close Exist Order");
            var ticket = await Execute(() => _apiClient.OrderCloseAll());
        }

        private Task<TResult> Execute<TResult>(Func<TResult> func)
        {
            return Task.Factory.StartNew(() =>
            {
                var result = default(TResult);
                try
                {
                    result = func();
                }
                catch (MtConnectionException ex)
                {
                    Console.WriteLine("MtExecutionException: " + ex.Message);
                }
                catch (MtExecutionException ex)
                {
                    Console.WriteLine("MtExecutionException: " + ex.Message + "; ErrorCode = " + ex.ErrorCode);
                }

                return result;
            });
        }

        public void Interval()
        {
           int  topThreshold = 110;
           int botThreshold = 855;

            img bitmapClass = new img(topThreshold, botThreshold);
                bitmapClass.screenshot();
                using (Bitmap bmp = new Bitmap("screen.bmp"))
                {
                    var scanning = bitmapClass.scanning(bmp);
                    if (counter == 0)
                    {
                        previous = scanning;
                        Console.WriteLine("First Initate initial state :{0}", previous);
                    }
                    else
                    {
                        if (scanning == "G" && previous != "G")
                        {
                            Console.WriteLine("Statechange from {0} to {1} BUY Signal", previous, scanning);
                        //Console.Beep();
                        CloseAll();
                        Thread.Sleep(10000);
                        buy();
                            previous = scanning;
                        }
                        else if (scanning == "P" && previous != "P")
                        {
                            Console.WriteLine("Statechange from {0} to {1} SELL signal", previous, scanning);
                        //Console.Beep();
                        CloseAll();
                        Thread.Sleep(10000);
                        sell();
                            previous = scanning;
                        }
                    }
                }
                counter++;
            }
        }
    }

