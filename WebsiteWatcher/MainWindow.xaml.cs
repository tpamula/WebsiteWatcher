using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WebsiteWatcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BStartSearch_OnClick(object sender, RoutedEventArgs e)
        {
            string targetWebsite = TbTargetWebsite.Text;
            string keyword = TbKeyword.Text;
            int interval = Convert.ToInt32(TbInterval.Text);

            var watcher = new Model.Watcher(keyword, interval, targetWebsite);
            watcher.Run();
            watcher.WordNotFound += () => Dispatcher.Invoke(ShowAlarm);
            watcher.WordNotFound += () => Task.Factory.StartNew(RingAlarm);

            ShowHeartbeats(watcher);
        }

        private void RingAlarm()
        {
            while (true)
            {
                System.Media.SystemSounds.Exclamation.Play();
                Thread.Sleep(1000);
            }
        }

        private void ShowAlarm()
        {
            GRoot.Children.Clear();
            GRoot.ColumnDefinitions.Clear();
            GRoot.RowDefinitions.Clear();

            var storyboard = new Storyboard();
            var fillerAnimation = new ColorAnimation
            {
                From = Colors.Red,
                To = Colors.Green,
                Duration = new Duration(new TimeSpan(0, 0, 0, 1)),
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };

            storyboard.Children.Add(fillerAnimation);
            Storyboard.SetTargetProperty(fillerAnimation, new PropertyPath("Background.Color"));

            Activate();
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            storyboard.Begin((FrameworkElement)GRoot.Parent);
        }

        private void ShowHeartbeats(Model.Watcher watcher)
        {
            var lHeartbeatTimeStamp = new Label();
            GRoot.Children.Clear();
            GRoot.Children.Add(lHeartbeatTimeStamp);

            watcher.Heartbeat +=
                () => Dispatcher.Invoke(() =>
                {
                    lHeartbeatTimeStamp.Content = DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy");
                });
        }
    }
}