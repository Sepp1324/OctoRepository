using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{
    public class CountedScopeSemaphore : IDisposable
    {
        private readonly object countLockObject;

        private readonly object lockObject;
        private readonly ManualResetEventSlim mainLock;
        private readonly ManualResetEventSlim superLock;
        private int counter;

        public CountedScopeSemaphore()
        {
            mainLock = new ManualResetEventSlim(true);
            superLock = new ManualResetEventSlim(true);
            lockObject = new object();
            countLockObject = new object();
        }

        public void Dispose()
        {
            superLock.Dispose();
            mainLock.Dispose();
        }

        public SuperScope Wait()
        {
            lock (lockObject)
            {
                mainLock.Wait();
                superLock.Reset();
            }

            return new SuperScope(this);
        }

        public CountScope EnterScope()
        {
            lock (lockObject)
            {
                superLock.Wait();
                lock (countLockObject)
                {
                    counter++;
                    if (counter > 0)
                        mainLock.Reset();
                }
            }


            return new CountScope(this);
        }

        private void LeaveMainScope()
        {
            lock (countLockObject)
            {
                counter--;
                if (counter == 0)
                    mainLock.Set();
            }
        }

        private void LeaveSuperScope()
        {
            superLock.Set();
        }

        public readonly struct CountScope : IDisposable, IEquatable<CountScope>
        {
            public static CountScope Empty => new CountScope(null);

            private readonly CountedScopeSemaphore internalSemaphore;

            public CountScope(CountedScopeSemaphore countingSemaphore)
            {
                internalSemaphore = countingSemaphore;
            }

            public void Dispose()
            {
                internalSemaphore?.LeaveMainScope();
            }

            public override bool Equals(object obj)
            {
                return obj is CountScope scope
                       && Equals(scope);
            }

            public bool Equals(CountScope other)
            {
                return EqualityComparer<CountedScopeSemaphore>.Default.Equals(internalSemaphore,
                    other.internalSemaphore);
            }

            public override int GetHashCode()
            {
                return 37286538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(internalSemaphore);
            }

            public static bool operator ==(CountScope left, CountScope right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(CountScope left, CountScope right)
            {
                return !(left == right);
            }
        }

        public readonly struct SuperScope : IDisposable, IEquatable<SuperScope>
        {
            public static SuperScope Empty => new SuperScope(null);

            private readonly CountedScopeSemaphore internalSemaphore;

            public SuperScope(CountedScopeSemaphore semaphore)
            {
                internalSemaphore = semaphore;
            }

            public void Dispose()
            {
                internalSemaphore?.LeaveSuperScope();
            }

            public override bool Equals(object obj)
            {
                return obj is SuperScope scope && Equals(scope);
            }

            public bool Equals(SuperScope other)
            {
                return EqualityComparer<CountedScopeSemaphore>.Default.Equals(internalSemaphore,
                    other.internalSemaphore);
            }

            public override int GetHashCode()
            {
                return 37296538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(internalSemaphore);
            }

            public static bool operator ==(SuperScope left, SuperScope right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SuperScope left, SuperScope right)
            {
                return !(left == right);
            }
        }
    }
}