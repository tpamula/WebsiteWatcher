using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WebsiteWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

            var watcher = new Watcher(keyword, interval, targetWebsite);
            watcher.Run();
            watcher.WordDisappeared += () => Dispatcher.Invoke(ShowAlarm);

            ShowHeartbeats(watcher);
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

        private void ShowHeartbeats(Watcher watcher)
        {
            var lHeartbeatTimeStamp = new Label();
            GRoot.Children.Clear();
            GRoot.Children.Add(lHeartbeatTimeStamp);

            watcher.Hearbeat +=
                () => Dispatcher.Invoke(() =>
                {
                    lHeartbeatTimeStamp.Content = DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy");
                });
        }
    }
}