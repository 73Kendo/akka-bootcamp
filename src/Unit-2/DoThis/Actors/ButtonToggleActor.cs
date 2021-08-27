using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ChartApp.Actors
{
    /// <summary>
    /// Actor responsible for managing button toggles
    /// </summary>
    public class ButtonToggleActor : UntypedActor
    {
        #region Mesages
        /// <summary>
        /// Toggles this button on or off and sends an appropiate messages
        /// to the <see cref="PerformanceCounterCoordinatorActor"/>
        /// </summary>
        public class Toggle { }
        #endregion
        private readonly CounterType _myCounterType;
        private bool _isToggledOn;
        private readonly Button _myButton;
        private readonly IActorRef _coordinatorActor;

        public ButtonToggleActor(CounterType myCounterType,  Button myButton, IActorRef coordinatorActor, bool isToggledOn = false)
        {
            _myCounterType = myCounterType;
            _isToggledOn = isToggledOn;
            _myButton = myButton;
            _coordinatorActor = coordinatorActor;
        }

        protected override void OnReceive(object message)
        {
            if (message is Toggle && _isToggledOn)
            {
                //togle is curently on
                //stop watchig this counter
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.UnWatch(_myCounterType));
                FlipToggle();
            }
            else if (message is Toggle && !_isToggledOn)
            {
                //toggle is curently off
                //start watching this counter
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Watch(_myCounterType));
                FlipToggle();
            }
            else
            {
                Unhandled(message);
            }
        }

        private void FlipToggle()
        {
            //flip the toggle
            _isToggledOn = !_isToggledOn;
            //change the text of button
            _myButton.Text = string.Format("{0} {1}", _myCounterType.ToString().ToUpperInvariant(), _isToggledOn ? "ON" : "OFF");
        }
    }
}
