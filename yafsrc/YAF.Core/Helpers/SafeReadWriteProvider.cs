namespace YAF.Core.Helpers;

using System;
using System.Threading;

/// <summary>
/// The safe read write provider.
/// </summary>
/// <typeparam name="T">
/// </typeparam>
public class SafeReadWriteProvider<T> : IReadWriteProvider<T>
    where T : class
{
    private readonly Func<T> _create;

    private readonly ReaderWriterLockSlim _slimLock = new();

    private T _instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeReadWriteProvider{T}"/> class.
    /// </summary>
    /// <param name="create">
    /// The create.
    /// </param>
    public SafeReadWriteProvider(Func<T> create)
    {
        this._create = create;
    }

    public T Instance
    {
        get
        {
            T returnInstance;

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
}