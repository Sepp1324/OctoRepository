using System;

namespace OctoAwesome.Rx
{
    public static class IObservableExtension
    {
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext) => observable.Subscribe(new Observer<T>(onNext));
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onException) => observable.Subscribe(new Observer<T>(onNext, onException));
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onException, Action onComplete) => observable.Subscribe(new Observer<T>(onNext, onException, onComplete));


        private class Observer<T> : IObserver<T>
        {
            private readonly Action<T> _onNext;
            private readonly Action<Exception> _onException;
            private readonly Action _onComplete;

            public Observer(Action<T> onNext = null, Action<Exception> onException = null, Action onComplete = null)
            {
               _onNext = onNext;
               _onException = onException;
               _onComplete = onComplete;
            }

            public void OnCompleted() => _onComplete?.Invoke();

            public void OnError(Exception error) => _onException?.Invoke(error);

            public void OnNext(T value) => _onNext?.Invoke(value);
        }
    }
}