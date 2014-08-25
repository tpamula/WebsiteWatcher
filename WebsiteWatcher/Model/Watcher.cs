using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteWatcher.Model
{
    public class Watcher
    {
        private readonly int _checkIntervalInMs;
        private readonly string _searchFor;
        private readonly string _targetWebsite;
        private readonly WebClient _webClient = new WebClient();
        private volatile bool _searchAllowed;
        private Task _searchTask;

        public Watcher(string searchFor, int checkIntervalInMs, string targetWebsite)
        {
            _searchFor = searchFor;
            _checkIntervalInMs = checkIntervalInMs;
            _targetWebsite = targetWebsite;
        }

        /// <summary>
        /// Occurs every search iteration.
        /// </summary>
        public event Action Heartbeat;

        /// <summary>
        /// Class stops monitoring on this event.
        /// </summary>
        public event Action WordNotFound;

        /// <summary>
        /// Could be done with DI, a separate interface, a class with it's implementation,
        /// a mock... Hardcoded dirty faking fits the project requirements better.
        /// </summary>
        public string FakedWebsiteContent { get; set; }

        public void Run()
        {
            if (_searchTask != null) throw new Exception("Task already assigned.");

            _searchAllowed = true;
            _searchTask = new Task(PerformSearch);
            _searchTask.Start();
        }

        public void Stop()
        {
            _searchAllowed = false;
            _searchTask = null;
        }

        protected virtual void OnHeartbeat()
        {
            Action handler = Heartbeat;
            if (handler != null) handler();
        }

        protected virtual void OnWordNotFound()
        {
            Stop();

            Action handler = WordNotFound;
            if (handler != null) handler();
        }

        private string GetWebsiteContent()
        {
            return string.IsNullOrEmpty(FakedWebsiteContent)
                ? _webClient.DownloadString(_targetWebsite)
                : FakedWebsiteContent;
        }

        private void PerformSearch()
        {
            while (_searchAllowed)
            {
                string websiteContent;
                try
                {
                    websiteContent = GetWebsiteContent();
                }
                catch
                {
                    // Word is unavailable. Raise the event.
                    // Could happen when site becomes 404'd.
                    OnWordNotFound();
                    break;
                }

                if (!websiteContent.Contains(_searchFor))
                {
                    OnWordNotFound();
                    break;
                }
                OnHeartbeat();

                Thread.Sleep(_checkIntervalInMs);
            }
        }
    }
}