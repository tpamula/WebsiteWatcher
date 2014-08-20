using System.Threading;
using Xunit;

namespace WebsiteWatcher.Tests
{
    public class WatcherTests
    {
        [Fact]
        private void should_not_signal_when_word_found()
        {
            var watcherCore = new Watcher(searchFor: "justTesting", checkIntervalInMs: 10,
                targetWebsite: "will not be used")
            {
                MockedWebsiteContent = "some content with a keyword: justTesting"
            };

            bool wordFound = false;
            watcherCore.WordDisappeared += () => { wordFound = true; };
            watcherCore.Run();

            // Time should be faked but quick and dirty solution is preffered.
            Thread.Sleep(20);

            Assert.False(wordFound);
        }

        [Fact]
        private void should_signal_when_word_not_found()
        {
            var watcherCore = new Watcher(searchFor: "justTesting", checkIntervalInMs: 10,
                targetWebsite: "will not be used")
            {
                MockedWebsiteContent = "some content without search keyword"
            };

            bool wordFound = false;
            watcherCore.WordDisappeared += () => { wordFound = true; };
            watcherCore.Run();

            // Time should be faked but quick and dirty solution is preffered.
            Thread.Sleep(20);

            Assert.True(wordFound);
        }

        [Fact]
        private void should_signal_when_word_disappears()
        {
            var watcherCore = new Watcher(searchFor: "justTesting", checkIntervalInMs: 10,
                targetWebsite: "will not be used")
            {
                MockedWebsiteContent = "some content without search keyword"
            };

            bool wordFound = false;
            watcherCore.WordDisappeared += () => { wordFound = true; };
            watcherCore.Run();

            // Time should be faked but quick and dirty solution is preffered.
            Thread.Sleep(10);
            watcherCore.MockedWebsiteContent = "keyword: justTesting!!";
            Thread.Sleep(10);

            Assert.True(wordFound);
        }
    }
}