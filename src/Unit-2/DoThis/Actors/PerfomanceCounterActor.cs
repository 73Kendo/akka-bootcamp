using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ChartApp.Actors
{
    /// <summary>
    /// Actor responsible for monitoring a specific <see cref="PerfomanceCounter"/>    
    /// </summary>
    public class PerfomanceCounterActor : UntypedActor
    {
        private readonly string _seriesName;
        private readonly Func<PerformanceCounter> _performanceCounterGenerator;
        private PerformanceCounter _counter;

        private readonly HashSet<IActorRef> _subscriptions;
        private readonly ICancelable _cancelPublishing;

        public PerfomanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator)
        {
            _seriesName = seriesName;
            _performanceCounterGenerator = performanceCounterGenerator;
            _subscriptions = new HashSet<IActorRef>();
            _cancelPublishing = new Cancelable(Context.System.Scheduler);
        }
        #region Actor lifecylce methods
        protected override void PreStart()
        {

            //create new instance of the performance counter
            _counter = _performanceCounterGenerator();
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromMilliseconds(250),
                TimeSpan.FromMilliseconds(250), 
                Self, 
                new GatherMerics(), 
                Self, 
                _cancelPublishing);            
        }

        protected override void PostStop()
        {
            try
            {
                //terminate tthe schedule task
                _cancelPublishing.Cancel(false);
                _counter.Dispose();
            }
            catch
            {
                // exteprion ??? don't care
            }
            finally
            {
                base.PostStop();
            }
        }
        #endregion

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GatherMerics msg:
                    //publish latest counter value to all subscribers
                    var metric = new Metric(_seriesName, _counter.NextValue());
                    foreach (var sub in _subscriptions)
                    {
                        sub.Tell(metric);
                    }
                    break;
                case SubscriberCounter sc:
                    //add subscription for this counter
                    //it's parent's job to filter by ounter types
                    _subscriptions.Add(sc.Subscriber);
                    break;
                case UnSubscriberCounter uc:
                    //remove a subscription from this counter
                    _subscriptions.Remove(uc.Subscriber);
                    break;                
            }
        }
    }
}
