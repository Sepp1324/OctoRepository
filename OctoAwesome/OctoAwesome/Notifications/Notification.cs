using OctoAwesome.Pooling;

namespace OctoAwesome.Notifications
{
    public abstract class Notification : IPoolElement
    {
        private IPool _pool;

        public uint SenderId { get; set; }

        public void Init(IPool pool)
        {
            _pool = pool;
            OnInit();
        }

        public void Release()
        {
            SenderId = 0;
            OnRelease();
            _pool.Push(this);
        }

        public virtual bool Match<T>(T filter)
        {
            return true;
        }

        /// <summary>
        ///     This method is called from the Init method. It's not needed to hold an seperate pool
        /// </summary>
        protected virtual void OnInit()
        {
        }

        /// <summary>
        ///     This is called from the Release method. Do not push this instance manualy to any pool
        /// </summary>
        protected virtual void OnRelease()
        {
        }
    }
}