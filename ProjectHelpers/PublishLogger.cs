using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectHelpers
{
    internal class PublishLogger : ILogger
    {
        public PublishLogger(IVsOutputWindowPane writer)
        {
            _writer = writer;
        }

        private IEventSource _eventSource;
        private IVsOutputWindowPane _writer;

        public LoggerVerbosity Verbosity { get; set; }

        public string Parameters { get; set; }

        public void Initialize(IEventSource eventSource)
        {
            _eventSource = eventSource;
            eventSource.ErrorRaised += ErrorRaised;
            eventSource.MessageRaised += MessageRaised;
            eventSource.WarningRaised += WarningRaised;
        }

        private void WarningRaised(object sender, BuildWarningEventArgs e)
        {
            _writer.OutputStringThreadSafe(e.Message);
            _writer.OutputStringThreadSafe(Environment.NewLine);
        }

        private void MessageRaised(object sender, BuildMessageEventArgs e)
        {
            _writer.OutputStringThreadSafe(e.Message);
            _writer.OutputStringThreadSafe(Environment.NewLine);
        }

        private void ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            _writer.OutputStringThreadSafe(e.Message);
            _writer.OutputStringThreadSafe(Environment.NewLine);
        }

        public void Shutdown()
        {
            _eventSource.ErrorRaised -= ErrorRaised;
            _eventSource.MessageRaised -= MessageRaised;
            _eventSource.WarningRaised -= WarningRaised;
        }
    }
}
