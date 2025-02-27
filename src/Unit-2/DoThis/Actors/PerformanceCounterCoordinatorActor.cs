﻿using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartApp.Actors
{
    /// <summary>
    /// Actor responsible for translating UI calls into ActorSystem messages
    /// </summary>
    public class PerformanceCounterCoordinatorActor : ReceiveActor
    {
        #region Message types
        /// <summary>
        /// Subscribe the <see cref="ChartingActor"/> to
        /// updates <see cref="Counter"/>
        /// </summary>
        public class Watch
        {
            public Watch(CounterType counter)
            {
                Counter = counter;
            }

            public CounterType Counter { get; private set; }
        }
        /// <summary>
        /// Unsubscribe the <see cref="ChartingActor"/> to
        /// updates <see cref="Counter"/>
        /// </summary>
        public class UnWatch
        {
            public UnWatch(CounterType counter)
            {
                Counter = counter;
            }

            public CounterType Counter { get; private set; }
        }
        #endregion
        /// <summary>
        /// Methods for generating new instances of all <see cref="PerformanceCounter"/>
        /// we want to monitor
        /// </summary>
        private static readonly Dictionary<CounterType, Func<PerformanceCounter>> CounterGeneartors = new Dictionary<CounterType, Func<PerformanceCounter>>()
            {

            {CounterType.Cpu, () => new PerformanceCounter("Processor", "% Processor Time", "_Total", true)},
            {CounterType.Memory, () => new PerformanceCounter("Memory", "% Committed Bytes In Use", true)},
            {CounterType.Disk, () => new PerformanceCounter("LogicalDisk", "% Disk Time", "_Total", true)},
            };

        /// <summary>
        /// Methods for creating new <see cref="Series"/> with distinct colors and names
        /// corresponding to each <see cref="PerfomanceCounter"/>
        /// </summary>
        private  static readonly Dictionary<CounterType, Func<Series>> CounterSeries = new Dictionary<CounterType, Func<Series>>()
        {
            { CounterType.Cpu,()=> new Series(CounterType.Cpu.ToString())
                {ChartType=SeriesChartType.SplineArea,
                Color=Color.DarkGreen }
            },
            {
                CounterType.Memory,()=> new Series(CounterType.Memory.ToString())
                { ChartType=SeriesChartType.FastLine,
                Color=Color.MediumBlue}
            },
            {
                CounterType.Disk,()=>new Series(CounterType.Disk.ToString())
                {ChartType=SeriesChartType.SplineArea,
                Color=Color.DarkRed}
            }

        };
        private Dictionary<CounterType, IActorRef> _counterActors;
        private IActorRef _chartingActor;

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor) :
            this(chartingActor, new Dictionary<CounterType,IActorRef>())
        {

        }

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor,
            Dictionary<CounterType,IActorRef> counterActors)
        {

            _chartingActor = chartingActor;
            _counterActors = counterActors;
            Receive<Watch>(watch =>
            {
            if (!_counterActors.ContainsKey(watch.Counter))
            {
                //create a child actor to monitor this counter
                //if one dose't exist alredy
                var counterActor = Context.ActorOf(Props.Create(() => new PerfomanceCounterActor(watch.Counter.ToString(), CounterGeneartors[watch.Counter])));
                //add this counter actor to our index
                _counterActors[watch.Counter] = counterActor;
            }

            //register this series with the ChartingActor
            _chartingActor.Tell(new ChartingActor.AddSeries(CounterSeries[watch.Counter]()));

                // tell the counter actor to begin publishing its
                // statistic to the _chartingActor
                _counterActors[watch.Counter].Tell(new SubscriberCounter(watch.Counter, _chartingActor));
            });
            Receive<UnWatch>(unwatch =>
            {
                if (!_counterActors.ContainsKey(unwatch.Counter))
                {
                    return;
                }
                //unsubscribe the ChartingActor from reciving any more updates
                _counterActors[unwatch.Counter].Tell(new UnSubscriberCounter(unwatch.Counter, _chartingActor));
                //remove this series from the ChartingActor
                _chartingActor.Tell(new ChartingActor.RemoveSeries(unwatch.Counter.ToString()));

            });
        }
    }
}
