using Akka.Actor;

namespace WinTail
{
    public class ValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriteActor;

        public ValidatorActor(IActorRef consoleWriteActor)
        {
            _consoleWriteActor = consoleWriteActor;
        }
        protected override void OnReceive(object msg)
        {
            var message = msg as string;
            if (string.IsNullOrEmpty(message))
            {
                //signal that the user needs to supply an input
                _consoleWriteActor.Tell(new Messages.NullInputError("No input recived"));
            }
            else
            {
                var valid = IsValid(message);
                if (valid)
                {
                    //send sucess to console writer
                    _consoleWriteActor.Tell(new Messages.InputSuccess("Thank you. Message is valid"));
                }
                else
                {
                    //signal that input was bad
                    _consoleWriteActor.Tell(new Messages.ValidatonError("Invalid input had odd number of charachters"));
                }
                //tell sendr to contrinue doing its thing
                //(whathever may be, this actor dosn't care)               
            }
            Sender.Tell(new Messages.ContinueProcessing());
        }

        private bool IsValid(string message)
        {
            var valid = message.Length % 2 == 0;
            return valid;
        }
    }
}
