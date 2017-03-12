using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Collections.Generic;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class kTradeLight : Robot
    {
        private HeikenAshi _heikenAshi;
        private int kcounter;
        private bool is_position_open;
        private int dayotw;
        private int pday;
        private int countpositions;


        [Parameter("Contracts (EURO)", DefaultValue = 100000, MinValue = 1, MaxValue = 100000000)]
        public int ncontracts { get; set; }

        [Parameter("Take Profit (pips)", DefaultValue = 0, MinValue = 0, MaxValue = 100000000)]
        public int takeprofit { get; set; }

        [Parameter("Stop Loss (pips)", DefaultValue = 0, MinValue = 0, MaxValue = 100000000)]
        public int stoploss { get; set; }

        [Parameter("Daily Positions", DefaultValue = 0, MinValue = 0, MaxValue = 100)]
        public int dailypositions { get; set; }

        //

        protected override void OnStart()
        {
            Print("kTrade Light started");
            Print("Server time is {0}", Server.Time.AddHours(1));
            _heikenAshi = Indicators.GetIndicator<HeikenAshi>(1);
            kcounter = 0;
            countpositions = 0;
            pday = (int)Server.Time.DayOfWeek;
            is_position_open = false;
            Positions.Closed += PositionsOnClosed;
        }

        protected override void OnBar()
        {
            kcounter++;
            dayotw = (int)Server.Time.DayOfWeek;

            if (is_position_open == false)
            {

                if (dayotw != pday)
                {
                    countpositions = 0;
                }

                if (dailypositions == 0 || countpositions < dailypositions)
                {
                    pday = dayotw;
                    countpositions++;
                    Print("Opening position");
                    is_position_open = true;

                    if (_heikenAshi.xOpen.Last(0) < _heikenAshi.xClose.Last(0))
                    {
                        var result = ExecuteMarketOrder(TradeType.Buy, Symbol, ncontracts, "kTrade Light", stoploss, takeprofit);
                    }
                    else
                    {
                        var result = ExecuteMarketOrder(TradeType.Sell, Symbol, ncontracts, "kTrade Light", stoploss, takeprofit);
                    }
                }

            }

        }
        // end of on bar event
        protected override void OnTick()
        {
            // no onTick events
        }

        protected override void OnStop()
        {
            Print("kTrade Light Stopped");
        }

        private void PositionsOnClosed(PositionClosedEventArgs args)
        {
            var pos = args.Position;
            Print("Position closed with €{0} profit", pos.GrossProfit);
            is_position_open = false;
        }

    }
}
