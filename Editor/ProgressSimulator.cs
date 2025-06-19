using System;
using System.Threading;
using System.Threading.Tasks;

namespace HatchStudio.Editor.Localization
{
    public class ProgressSimulator
    {
        private readonly string[] messages;
        private readonly float intervalSeconds;
        private int currentIndex = 0;
        private bool isRunning = false;
        private CancellationTokenSource cts;

        public int CurrentIndex => currentIndex;
        public bool IsRunning => isRunning;

        public event Action OnStatusChanged;

        public ProgressSimulator(string[] statusMessages, float intervalSeconds = 0.5f)
        {
            messages = statusMessages;
            this.intervalSeconds = intervalSeconds;
        }

        public void Start()
        {
            if (isRunning)
                return;

            isRunning = true;
            cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (isRunning && currentIndex < messages.Length - 2) // să nu treacă de penultimul
                {
                    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cts.Token);
                    currentIndex++;
                    OnStatusChanged?.Invoke();
                }
            }, cts.Token);
        }

        public void Stop()
        {
            if (!isRunning)
                return;

            isRunning = false;
            cts.Cancel();
            currentIndex = messages.Length - 1; // Setăm direct pe "Applying response..."
            OnStatusChanged?.Invoke();
        }

        public string GetCurrentMessage()
        {
            return messages[currentIndex];
        }
    }
}