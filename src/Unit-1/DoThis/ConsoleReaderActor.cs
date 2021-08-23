using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
        public const string StartCommand = "start";
        private IActorRef _validationActor;
        //remove unit 5
        //public ConsoleReaderActor(IActorRef validationActor)
        //{
        //    _validationActor = validationActor;
        //}

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                DoPrintInstructions();
            }
           
            GetAndValidateInput();
        }
        #region Internal methods
        private void DoPrintInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk.\n");


            //replace at Lesson4
            //Console.WriteLine("Write whatever you want into the console!");
            //Console.WriteLine("Some entries will pass validation, and some won't...\n\n");
            //Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }

        /// <summary>
        /// Reads input from console, validates it, then signals appropriate response
        /// (continue processing, error, success, etc...)
        /// </summary>
        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if (! string.IsNullOrEmpty(message) && String.Equals(message,ExitCommand,StringComparison.OrdinalIgnoreCase))
            {
                //if user typed ExitCommand, shutdown to entrie actor
                //syste,a (allows to process exit)
                Context.System.Terminate();
                return;
            }
            //else 
            //{
            //    _validationActor.Tell(message);
            //}


            // otherwise, just send the message off for validation
            Context.ActorSelection("akka://MyActorSystem/user/validationActor").Tell(message);
        }
        
    #endregion


}
}