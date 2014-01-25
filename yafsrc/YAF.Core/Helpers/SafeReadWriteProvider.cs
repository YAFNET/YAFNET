namespace YAF.Core.Helpers
{
    using System;
    using System.Threading;

    using YAF.Types.Interfaces;

    public class SafeReadWriteProvider<T> : IReadWriteProvider<T>
        where T : class
    {
        #region Fields

        private readonly Func<T> _create;

        private readonly ReaderWriterLockSlim _slimLock = new ReaderWriterLockSlim();

        private T _instance;

        #endregion

        #region Constructors and Destructors

        public SafeReadWriteProvider(Func<T> create)
        {
            this._create = create;
        }

        #endregion

        #region Public Properties

        public T Instance
        {
            get
            {
                T returnInstance = null;

                this._slimLock.EnterUpgradeableReadLock();
                try
                {
                    
                    returnInstance = this._instance;
                    if (returnInstance == null)
                    {
                        returnInstance = this._create();
                        // call this setter...
                        this.Instance = returnInstance;
                    }
                }
                finally
                {
                    this._slimLock.ExitUpgradeableReadLock();
                }

                return returnInstance;
            }
            set
            {
                this._slimLock.EnterWriteLock();

                try
                {
                    this._instance = value;
                }
                finally
                {
                    this._slimLock.ExitWriteLock();
                }
            }
        }

        #endregion
    }
}