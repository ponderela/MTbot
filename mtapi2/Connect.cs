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
using Newtonsoft.Json;

namespace mtapi2
{
    class Connect
    {
        readonly List<Action> _groupOrderCommands = new List<Action>();
        readonly MtApiClient _apiClient = new MtApiClient();
       // readonly TimerTradeMonitor _timerTradeMonitor;
        //readonly TimeframeTradeMonitor _timeframeTradeMonitor;
        int status = 0;

        String symbol = "GBPUSD";
        double volume = 0.02;
        int slippage = 1;
        double sl_volume = 0.010;
        double tp_volume = 0.020;

        /*status 1 = buy
         *status 2 = sell 
         */
        public Connect(int status)
        {
            //_timerTradeMonitor = new TimerTradeMonitor(_apiClient) { Interval = 10000 }; // 10 sec
            //_timeframeTradeMonitor = new TimeframeTradeMonitor(_apiClient);
            this.status = status;
            Connect_Click();           
        }

        public void Connect_Click()
        {
            //_timerTradeMonitor.Start();
            //_timeframeTradeMonitor.Start();
            _apiClient.BeginConnect(8222);
            apiClient_ConnectionStateChanged();
        }

        public void apiClient_ConnectionStateChanged()
        {
            while (true)
            {
                    var temp = _apiClient.ConnectionState;
                    if (temp.ToString() == "Connected")
                    {
                        Console.WriteLine("Connected at : {0}", DateTime.Now.ToString());
                        if(status == 1)
                        {
                            CloseAll();
                             buy();
                        }
                        if(status == 2)
                        {
                             CloseAll();
                            sell();
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Connecting");
                    }
                
                Thread.Sleep(3000);
            }
        }
        public async void buy()
        {
            var result = await Execute(() => _apiClient.SymbolInfoTick(symbol));
            double current = result.Ask;


             var ticket = await Execute(() => _apiClient.OrderSendBuy(symbol, volume, slippage, current - sl_volume, current + tp_volume));
            //var ticket = await Execute(() => _apiClient.OrderSendBuy(symbol, volume, slippage));
            Console.WriteLine("Perform BUY Order at {0}",DateTime.Now.ToString());
        }

        public async void sell()
        {
            var result = await Execute(() => _apiClient.SymbolInfoTick(symbol));
            double current = result.Bid;


            var ticket = await Execute(() => _apiClient.OrderSendSell(symbol, volume, slippage, current + sl_volume, current - tp_volume));
            //var ticket = await Execute(() => _apiClient.OrderSendSell(symbol, volume, slippage));
            Console.WriteLine("Perform SELL Order at {0}", DateTime.Now.ToString());
        }

        public async void CloseAll()
        {
            Thread.Sleep(10000);
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
        }
    }

