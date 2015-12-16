using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

namespace LightSharp
{
    public sealed class HttpServerTask : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;
        private HttpServer httpServer;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("HttpServerTask::Run()");

            // Associate a cancellation handler with the background task. 
            taskInstance.Canceled += OnCanceled;

            // Get the deferral object from the task instance
            serviceDeferral = taskInstance.GetDeferral();

            httpServer = new HttpServer(8000);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ThreadPool.RunAsync(w => httpServer.StartServerAsync());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine("HttpServerTask::OnCanceled()");

            httpServer.Dispose();
            httpServer = null;

            serviceDeferral.Complete();
        }
    }
}
