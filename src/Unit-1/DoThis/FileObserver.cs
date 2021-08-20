using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WinTail
{
    /// <summary>
    /// Turns <see cref="FileSystemWatcher"/> events about a specific file into messages for <see cref="TailActor"/>.
    /// </summary>
    public class FileObserver : IDisposable
    {
        private readonly IActorRef _tailActor;
        private readonly string _aboluteFilePath;
        private FileSystemWatcher _watcher;
        private readonly string _fileDir;
        private readonly string _fileNameOnly;

        public FileObserver(IActorRef tailActor, string aboluteFilePath)
        {
            _tailActor = tailActor;
            _aboluteFilePath = aboluteFilePath;
            _fileDir = Path.GetDirectoryName(aboluteFilePath);
            _fileNameOnly = Path.GetFileName(aboluteFilePath);
        }
        /// <summary>
        /// Begin monitoring file.
        /// </summary>
        public void Start()
        {
            //make watcher to observe our specific file
            _watcher = new FileSystemWatcher(_fileDir, _fileNameOnly);

            //watch our file for changes to the file name,
            //or new mesages being wrriten to file
            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            //assign callback for event types

            _watcher.Changed += OnFileChanged;
            _watcher.Error += OnFileError;

            // start watching
            _watcher.EnableRaisingEvents = true;
        }
        /// <summary>
        /// Callback for <see cref="FileSystemWatcher"/> file error events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileError(object sender, ErrorEventArgs e)
        {
            _tailActor.Tell(new TailActor.FileError(_fileNameOnly, e.GetException().Message));
        }

        /// <summary>
        /// Callback for <see cref="FileSystemWatcher"/> file change events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            //here we use a special ActorRefs.Nosender
            //since this event can hapen many times,
            //this is a litte microptimazation

            _tailActor.Tell(new TailActor.FileWriter(e.Name), ActorRefs.NoSender);
        }

        /// <summary>
        /// Stop monitoring
        /// </summary>
        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
