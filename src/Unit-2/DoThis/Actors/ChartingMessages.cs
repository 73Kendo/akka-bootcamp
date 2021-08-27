using Akka.Actor;

namespace ChartApp.Actors
{

    #region Reporting

    /// <summary>
    /// Signal used to indicate that it's time to sample all counters
    /// </summary>
    ///     
    public class GatherMerics { }
    /// <summary>
    /// Metric data at the time of sample
    /// </summary>
    public class Metric
    {
        public Metric(string series, float counterValue)
        {
            Series = series;
            CounterValue = counterValue;
        }

        public string Series { get; private set; }
        public float CounterValue { get; private set; }
    }
    #endregion

    #region Performance Counter Managment
    /// <summary>
    /// All types of counters suported by this examples
    /// </summary>
    public enum CounterType
    {
        Cpu,
        Memory,
        Disk
    }
    /// <summary>
    /// Enable a counter and begins publising values to <see cref="Subscriber"/>
    /// </summary>
    public class SubscriberCounter
    {
        public SubscriberCounter(CounterType counter, IActorRef subscriber)
        {
            Counter = counter;
            Subscriber = subscriber;
        }

        public CounterType Counter { get; private set; }
        public IActorRef Subscriber { get; private set; }
    }
    /// <summary>
    /// Unsubscribers <see cref="Subscriber"/> from reciving updates
    /// for a given counter
    /// </summary>
    public class UnSubscriberCounter
    {
        public UnSubscriberCounter(CounterType counter, IActorRef subscriber)
        {
            Counter = counter;
            Subscriber = subscriber;
        }

        public CounterType Counter { get; private set; }
        public IActorRef Subscriber { get; private set; }
    }
    #endregion
}
