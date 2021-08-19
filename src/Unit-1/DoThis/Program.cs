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
            Props validationActorProps = Props.Create(() => new ValidatorActor(consoleWriterActor));
            IActorRef validationActor = MyActorSystem.ActorOf(validationActorProps, "validationActor");
            Props consoleReaderProps = Props.Create<ConsoleReaderActor>(validationActor);
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
