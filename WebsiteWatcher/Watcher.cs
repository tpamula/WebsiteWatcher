using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteWatcher
{
    public class Watcher
    {
        private readonly string _searchFor;
        private readonly int _checkIntervalInMs;
        private readonly string _targetWebsite;
        private bool _searchAllowed;

        public Watcher(string searchFor, int checkIntervalInMs, string targetWebsite)
        {
            _searchFor = searchFor;
            _checkIntervalInMs = checkIntervalInMs;
            _targetWebsite = targetWebsite;
        }

        public event Action WordFound;

        protected virtual void OnWordFound()
        {
            Action handler = WordFound;
            if (handler != null) handler();
        }

        public event Action Hearbeat;

        protected virtual void OnHeartbeat()
        {
            Action handler = Hearbeat;
            if (handler != null) handler();
        }

        private Task _searchTask;

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

        public string MockedWebsiteContent { get; set; }

        private void PerformSearch()
        {
            while (_searchAllowed)
            {
                var webClient = new WebClient();
                string websiteContent = string.IsNullOrEmpty(MockedWebsiteContent)
                    ? webClient.DownloadString(_targetWebsite)
                    : MockedWebsiteContent;

                if (!websiteContent.Contains(_searchFor))
                {
                    OnWordFound();
                    break;
                }
                OnHeartbeat();

                Thread.Sleep(_checkIntervalInMs);
            }
        }
    }
}