using OctoAwesome.Pooling;

namespace OctoAwesome.Notifications
{
    public abstract class Notification : IPoolElement
    {
<<<<<<< HEAD
        private IPool _pool;
        
        public uint SenderId { get; set; }

        public virtual bool Match<T>(T filter) => true;
=======
        public uint SenderId { get; set; }

        private IPool pool;

        public virtual bool Match<T>(T filter)
        {
            return true;
        }
>>>>>>> feature/performance

        public void Init(IPool pool)
        {
            _pool = pool;
            OnInit();
        }

        public void Release()
        {
            SenderId = 0;
            OnRelease();
<<<<<<< HEAD
            _pool.Push(this);
=======
            pool.Push(this);
>>>>>>> feature/performance
        }

        /// <summary>
        /// This method is called from the Init method. It's not needed to hold an seperate pool
        /// </summary>
<<<<<<< HEAD
        protected virtual void OnInit() { }
=======
        protected virtual void OnInit()
        {

        }
>>>>>>> feature/performance

        /// <summary>
        /// This is called from the Release method. Do not push this instance manualy to any pool 
        /// </summary>
<<<<<<< HEAD
        protected virtual void OnRelease() { }
=======
        protected virtual void OnRelease()
        {

        }
>>>>>>> feature/performance
    }
}
