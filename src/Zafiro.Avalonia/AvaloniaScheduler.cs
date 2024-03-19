using Avalonia.Threading;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Zafiro.Avalonia;

    public class AvaloniaScheduler : LocalScheduler
    {
        /// <summary>
        /// Users can schedule actions on the dispatcher thread while being on the correct thread already.
        /// We are optimizing this case by invoking user callback immediately which can lead to stack overflows in certain cases.
        /// To prevent this we are limiting amount of reentrant calls to <see cref="Schedule{TState}"/> before we will
        /// schedule on a dispatcher anyway.
        /// </summary>
        private const int MaxReentrantSchedules = 32;

        private int reentrancyGuard;

        /// <summary>
        /// The instance of the <see cref="AvaloniaScheduler"/>.
        /// </summary>
        public static readonly AvaloniaScheduler Instance = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaScheduler"/> class.
        /// </summary>
        private AvaloniaScheduler()
        {
        }

        /// <inheritdoc/>
        public override IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            IDisposable PostOnDispatcher()
            {
                var composite = new CompositeDisposable(2);

                var cancellation = new CancellationDisposable();

                Dispatcher.UIThread.Post(() =>
                {
                    if (!cancellation.Token.IsCancellationRequested)
                    {
                        composite.Add(action(this, state));
                    }
                }, DispatcherPriority.Background);

                composite.Add(cancellation);

                return composite;
            }

            if (dueTime == TimeSpan.Zero)
            {
                if (!Dispatcher.UIThread.CheckAccess())
                {
                    return PostOnDispatcher();
                }
                else
                {
                    if (reentrancyGuard >= MaxReentrantSchedules)
                    {
                        return PostOnDispatcher();
                    }

                    try
                    {
                        reentrancyGuard++;

                        return action(this, state);
                    }
                    finally
                    {
                        reentrancyGuard--;
                    }
                }
            }
            else
            {
                var composite = new CompositeDisposable(2);

                composite.Add(DispatcherTimer.RunOnce(() => composite.Add(action(this, state)), dueTime));

                return composite;
            }
        }
    }