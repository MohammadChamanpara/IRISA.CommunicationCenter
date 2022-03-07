using IRISA.CommunicationCenter.Library.Logging;
using System;
using System.Threading.Tasks;

namespace IRISA.CommunicationCenter.Library.Tasks
{
    public class BackgroundTimer
    {
        private DateTime _startTime;
        private readonly ILogger _logger;

        public bool IsRunning => _started;
        private bool _started;
        public int Interval { get; set; }
        public int? AliveTime { get; set; }
        public string PersianDescription { get; set; }

        public event Action DoWork;
        public event Action Started;
        public event Action Stopped;

        public BackgroundTimer(ILogger logger)
        {
            AliveTime = null;
            _started = false;
            Interval = 1000;
            _startTime = DateTime.Now;
            _logger = logger;
        }

        public void Start()
        {
            _startTime = DateTime.Now;
            if (_started)
                return;
            _started = true;
            Started?.Invoke();
            _ = Task.Run(() => KeepRunning());
        }

        private async Task KeepRunning()
        {
            while (_started)
            {
                try
                {
                    if (AliveTime.HasValue)
                        if ((DateTime.Now - _startTime).TotalSeconds >= AliveTime)
                            Stop();

                    DoWork?.Invoke();

                }
                catch (Exception exception)
                {
                    _logger.LogException(exception, $"بروز خطا در {PersianDescription}. ");
                }
                finally
                {
                    await Task.Delay(Interval);
                }
            }
            Stop();
        }

        public void Stop()
        {
            if (!_started)
                return;

            _started = false;
            Stopped?.Invoke();
        }
    }
}
