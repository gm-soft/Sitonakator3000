using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WebsiteReleaseHelper.FormsControlWrappers
{
    public class ProgressBarWrapper
    {
        private readonly ProgressBar _progressBar;

        private readonly Timer _updateTimer;

        private const double TimerTickInterval = 50;

        private const double ProgressBarStep = 1;

        private readonly Dispatcher _dispatcher;

        public ProgressBarWrapper(ProgressBar progressBar, Dispatcher dispatcher)
        {
            _progressBar = progressBar;

            _dispatcher = dispatcher;

            _progressBar.Minimum = 0;

            _progressBar.Maximum = 100;

            _progressBar.Value = 0;

            _updateTimer = new Timer(TimerTickInterval);

            _updateTimer.Elapsed += UpdateTimerOnElapsed;
        }

        private async void UpdateTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            await _dispatcher.InvokeAsync(() =>
            {
                // учитывая, что работаем с числами с плавающей точкой,
                // на строгое равенство завязывать не будем и с толерантностью тоже связываться не будем.
                // оставим просто сравнение >=
                if (_progressBar.Value >= _progressBar.Maximum)
                    _progressBar.Value = 0;

                _progressBar.Value += ProgressBarStep;
            });
        }

        public void Start()
        {
            _updateTimer.Start();
        }

        public void Stop()
        {
            _updateTimer.Stop();

            RefreshProgressBar();
        }

        public async void RefreshProgressBar()
        {
            await _dispatcher.InvokeAsync(() =>
            {
                _progressBar.Value = _progressBar.Minimum;
            });
        }
    }
}