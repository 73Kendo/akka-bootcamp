﻿using Akka.Actor;
using System.IO;

namespace WinTail
{
    /// <summary>
    /// Actor that validates user input and signals result to others
    /// </summary>
    public class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;
        ///private readonly IActorRef _tailCoordinatorActor;

        public FileValidatorActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
           // _tailCoordinatorActor = tailCoordinatorActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                //signal that the user needs to supply an input
                _consoleWriterActor.Tell(new Messages.NullInputError("Input was blank. Please try again.\n"));

                //tell sender to contiue doing its thing
                //(whatever that my be, this actor dosn't care)
                Sender.Tell(new Messages.ContinueProcessing());
            }
            else
            {
                var valid = IsFileUri(msg);
                if (valid)
                {
                    //signal successful input
                    _consoleWriterActor.Tell(new Messages.InputSuccess($"Starting processing for {msg}"));

                    //start coordinator
                    //_tailCoordinatorActor.Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                    Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor").Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                }
                else
                {
                    //signal taht input was bad
                    _consoleWriterActor.Tell(new Messages.ValidatonError($"{msg} is not an existing URI on disk"));

                    //tell sender to continue doing its thing 
                    //(whatever that may be , this actor dosn't care)
                    Sender.Tell(new Messages.ContinueProcessing());

                }
            }            
        }
        /// <summary>
        /// Check if file exost at path provided by user
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsFileUri(string path)
        {
            return File.Exists(path);
        }
    }
}
