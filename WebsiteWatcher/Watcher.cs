using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteWatcher
{
    public class Watcher
    {
        private readonly int _checkIntervalInMs;
        private readonly string _searchFor;
        private readonly string _targetWebsite;
        private volatile bool _searchAllowed;
        private Task _searchTask;

        public Watcher(string searchFor, int checkIntervalInMs, string targetWebsite)
        {
            _searchFor = searchFor;
            _checkIntervalInMs = checkIntervalInMs;
            _targetWebsite = targetWebsite;
        }

        public event Action Hearbeat;

        public event Action WordDisappeared;

        public string MockedWebsiteContent { get; set; }

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
            Action handler = Hearbeat;
            if (handler != null) handler();
        }

        protected virtual void OnWordDisappeared()
        {
            Stop();

            Action handler = WordDisappeared;
            if (handler != null) handler();
        }

        private void PerformSearch()
        {
            var webClient = new WebClient();

            while (_searchAllowed)
            {
                string websiteContent = string.IsNullOrEmpty(MockedWebsiteContent)
                    ? webClient.DownloadString(_targetWebsite)
                    : MockedWebsiteContent;

                if (!websiteContent.Contains(_searchFor))
                {
                    OnWordDisappeared();
                    break;
                }
                OnHeartbeat();

                Thread.Sleep(_checkIntervalInMs);
            }
        }
    }
}