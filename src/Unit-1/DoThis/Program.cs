using System;
﻿using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            Props consoleWriterProps = Props.Create(() => new ConsoleWriterActor());
            IActorRef consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleAriaterActor");
            //Props validationActorProps = Props.Create(() => new ValidatorActor(consoleWriterActor)); // replace at Lesson4
            //IActorRef validationActor = MyActorSystem.ActorOf(validationActorProps, "validationActor"); 
           
            //make TailCoridantorActor
            Props tailCoordinatorProps = Props.Create(() => new TailCoordinatorActor());
            IActorRef tailCordinatorActor = MyActorSystem.ActorOf(tailCoordinatorProps, "tailCoordinatorActor");

            //pass TailCoordinatorActor to FileValidatorActor (just adding one extra arg
            Props fileValidatorProps = Props.Create(() => new FileValidatorActor(consoleWriterActor));
            IActorRef fileValidatorActor = MyActorSystem.ActorOf(fileValidatorProps, "validationActor");

            Props consoleReaderProps = Props.Create<ConsoleReaderActor>();
            IActorRef consoleReadActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReadActor");
            //var consoleWriteActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            //var consoleReadActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(consoleWriteActor)));
            consoleReadActor.Tell(ConsoleReaderActor.StartCommand);
            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }

    }
    #endregion
}
