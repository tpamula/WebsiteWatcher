using System.Threading;
using Xunit;

namespace WebsiteWatcher.Tests
{
    public class WatcherTests
    {
        [Fact]
        private void should_not_signal_when_word_found()
        {
            var websiteWatcher = new Model.Watcher(searchFor: "justTesting", checkIntervalInMs: 0,
                targetWebsite: "will not be used")
            {
                FakedWebsiteContent = "some content with a keyword: justTesting"
            };

            bool wordFound = false;
            websiteWatcher.WordNotFound += () => { wordFound = true; };
            websiteWatcher.Run();

            // Time could be faked but quick and dirty solution is preffered.
            Thread.Sleep(10);

            websiteWatcher.Stop();
            Assert.False(wordFound);
        }

        [Fact]
        private void should_signal_when_word_disappears()
        {
            var websiteWatcher = new Model.Watcher(searchFor: "justTesting", checkIntervalInMs: 0,
                targetWebsite: "will not be used")
            {
                FakedWebsiteContent = "some content without search keyword"
            };

            bool wordFound = false;
            websiteWatcher.WordNotFound += () => { wordFound = true; };
            websiteWatcher.Run();

            // Time could be faked but quick and dirty solution is preffered.
            Thread.Sleep(10);
            websiteWatcher.FakedWebsiteContent = "keyword: justTesting!!";
            Thread.Sleep(10);

            websiteWatcher.Stop();
            Assert.True(wordFound);
        }

        [Fact]
        private void should_signal_when_word_not_found()
        {
            var websiteWatcher = new Model.Watcher(searchFor: "justTesting", checkIntervalInMs: 0,
                targetWebsite: "will not be used")
            {
                FakedWebsiteContent = "some content without search keyword"
            };

            bool wordFound = false;
            websiteWatcher.WordNotFound += () => { wordFound = true; };
            websiteWatcher.Run();

            // Time could be faked but quick and dirty solution is preffered.
            Thread.Sleep(10);

            websiteWatcher.Stop();
            Assert.True(wordFound);
        }
    }
}