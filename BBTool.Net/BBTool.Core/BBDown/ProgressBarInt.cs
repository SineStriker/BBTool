using System;
using System.Text;
using System.Threading;

/**
 * From https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54
 */
namespace BBDown
{
    class ProgressBarInt : IDisposable, IProgress<int>
    {
        private const int blockCount = 40;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string animation = @"|/-\";

        private readonly Timer timer;

        private string currentText = string.Empty;
        private bool disposed = false;
        private int animationIndex = 0;

        private int minimum;
        private int maximum;
        private int currentValue;

        public ProgressBarInt(int min, int max)
        {
            minimum = min;
            maximum = max;

            timer = new Timer(TimerHandler);

            // A progress bar is only for temporary display in a console window.
            // If the console output is redirected to a file, draw nothing.
            // Otherwise, we'll end up with a lot of garbage in the target file.
            if (!Console.IsOutputRedirected)
            {
                ResetTimer();
            }
        }

        public void Report(int value)
        {
            // Make sure value is in [0..1] range
            value = Math.Max(minimum, Math.Min(maximum, value));
            Interlocked.Exchange(ref currentValue, value);
        }

        private void TimerHandler(object? state)
        {
            lock (timer)
            {
                if (disposed) return;

                double currentProgress = (double)(currentValue - minimum) / (maximum - minimum);
                int progressBlockCount = (int)(currentProgress * blockCount);
                string text = string.Format("                            [{0}{1}] {2} {3}/{4}",
                    new string('#', progressBlockCount), new string('-', blockCount - progressBlockCount),
                    animation[animationIndex++ % animation.Length],
                    currentValue,
                    maximum
                );
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            StringBuilder outputBuilder = new();
            outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text[commonPrefixLength..]);

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            currentText = text;
        }

        private void ResetTimer()
        {
            timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
        }

        public void Dispose()
        {
            lock (timer)
            {
                disposed = true;
                UpdateText(string.Empty);
            }
        }
    }
}