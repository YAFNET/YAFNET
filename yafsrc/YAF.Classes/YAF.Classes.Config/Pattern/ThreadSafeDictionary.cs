/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Classes.Pattern
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Threading;

  /// <summary>
  /// The thread safe dictionary.
  /// </summary>
  /// <typeparam name="TKey">
  /// </typeparam>
  /// <typeparam name="TValue">
  /// </typeparam>
  public interface IThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
  {
    /// <summary>
    /// Merge is similar to the SQL merge or upsert statement.  
    /// </summary>
    /// <param name="key">
    /// Key to lookup
    /// </param>
    /// <param name="newValue">
    /// New Value
    /// </param>
    void MergeSafe(TKey key, TValue newValue);

    /// <summary>
    /// This is a blind remove. Prevents the need to check for existence first.
    /// </summary>
    /// <param name="key">
    /// Key to Remove
    /// </param>
    void RemoveSafe(TKey key);
  }

  /// <summary>
  /// The thread safe dictionary.
  /// </summary>
  /// <typeparam name="TKey">
  /// </typeparam>
  /// <typeparam name="TValue">
  /// </typeparam>
  [Serializable]
  public class ThreadSafeDictionary<TKey, TValue> : IThreadSafeDictionary<TKey, TValue>
  {
    /// <summary>
    /// This is the internal dictionary that we are wrapping
    /// </summary>
    private IDictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

    /// <summary>
    /// The dictionary lock.
    /// </summary>
    private ReaderWriterLockSlim dictionaryLock = Locks.GetLockInstance(LockRecursionPolicy.NoRecursion); // setup the lock;

    #region IThreadSafeDictionary<TKey,TValue> MembersG

    /// <summary>
    /// This is a blind remove. Prevents the need to check for existence first.
    /// </summary>
    /// <param name="key">
    /// Key to remove
    /// </param>
    public void RemoveSafe(TKey key)
    {
      using (new ReadLock(this.dictionaryLock))
      {
        if (this.dict.ContainsKey(key))
        {
          using (new WriteLock(this.dictionaryLock))
          {
            this.dict.Remove(key);
          }
        }
      }
    }

    /// <summary>
    /// Merge does a blind remove, and then add.  Basically a blind Upsert.  
    /// </summary>
    /// <param name="key">
    /// Key to lookup
    /// </param>
    /// <param name="newValue">
    /// New Value
    /// </param>
    public void MergeSafe(TKey key, TValue newValue)
    {
      using (new WriteLock(this.dictionaryLock))
      {
        // take a writelock immediately since we will always be writing
        if (this.dict.ContainsKey(key))
        {
          this.dict.Remove(key);
        }


        this.dict.Add(key, newValue);
      }
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The remove.
    /// </returns>
    public virtual bool Remove(TKey key)
    {
      using (new WriteLock(this.dictionaryLock))
      {
        return this.dict.Remove(key);
      }
    }

    /// <summary>
    /// The contains key.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The contains key.
    /// </returns>
    public virtual bool ContainsKey(TKey key)
    {
      using (new ReadOnlyLock(this.dictionaryLock))
      {
        return this.dict.ContainsKey(key);
      }
    }

    /// <summary>
    /// The try get value.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The try get value.
    /// </returns>
    public virtual bool TryGetValue(TKey key, out TValue value)
    {
      using (new ReadOnlyLock(this.dictionaryLock))
      {
        return this.dict.TryGetValue(key, out value);
      }
    }

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    public virtual TValue this[TKey key]
    {
      get
      {
        using (new ReadOnlyLock(this.dictionaryLock))
        {
          return this.dict[key];
        }
      }

      set
      {
        using (new WriteLock(this.dictionaryLock))
        {
          this.dict[key] = value;
        }
      }
    }

    /// <summary>
    /// Gets Keys.
    /// </summary>
    public virtual ICollection<TKey> Keys
    {
      get
      {
        using (new ReadOnlyLock(this.dictionaryLock))
        {
          return new List<TKey>(this.dict.Keys);
        }
      }
    }

    /// <summary>
    /// Gets Values.
    /// </summary>
    public virtual ICollection<TValue> Values
    {
      get
      {
        using (new ReadOnlyLock(this.dictionaryLock))
        {
          return new List<TValue>(this.dict.Values);
        }
      }
    }

    /// <summary>
    /// The clear.
    /// </summary>
    public virtual void Clear()
    {
      using (new WriteLock(this.dictionaryLock))
      {
        this.dict.Clear();
      }
    }

    /// <summary>
    /// Gets Count.
    /// </summary>
    public virtual int Count
    {
      get
      {
        using (new ReadOnlyLock(this.dictionaryLock))
        {
          return this.dict.Count;
        }
      }
    }

    /// <summary>
    /// The contains.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// The contains.
    /// </returns>
    public virtual bool Contains(KeyValuePair<TKey, TValue> item)
    {
      using (new ReadOnlyLock(this.dictionaryLock))
      {
        return this.dict.Contains(item);
      }
    }

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    public virtual void Add(KeyValuePair<TKey, TValue> item)
    {
      using (new WriteLock(this.dictionaryLock))
      {
        this.dict.Add(item);
      }
    }

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public virtual void Add(TKey key, TValue value)
    {
      using (new WriteLock(this.dictionaryLock))
      {
        this.dict.Add(key, value);
      }
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// The remove.
    /// </returns>
    public virtual bool Remove(KeyValuePair<TKey, TValue> item)
    {
      using (new WriteLock(this.dictionaryLock))
      {
        return this.dict.Remove(item);
      }
    }

    /// <summary>
    /// The copy to.
    /// </summary>
    /// <param name="array">
    /// The array.
    /// </param>
    /// <param name="arrayIndex">
    /// The array index.
    /// </param>
    public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      using (new ReadOnlyLock(this.dictionaryLock))
      {
        this.dict.CopyTo(array, arrayIndex);
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsReadOnly.
    /// </summary>
    public virtual bool IsReadOnly
    {
      get
      {
        using (new ReadOnlyLock(this.dictionaryLock))
        {
          return this.dict.IsReadOnly;
        }
      }
    }

    /// <summary>
    /// The get enumerator.
    /// </summary>
    /// <returns>
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// </exception>
    public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      throw new NotSupportedException("Cannot enumerate a threadsafe dictionary.  Instead, enumerate the keys or values collection");
    }


    /// <summary>
    /// The i enumerable. get enumerator.
    /// </summary>
    /// <returns>
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// </exception>
    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotSupportedException("Cannot enumerate a threadsafe dictionary.  Instead, enumerate the keys or values collection");
    }

    #endregion
  }

  /// <summary>
  /// The locks.
  /// </summary>
  public static class Locks
  {
    /// <summary>
    /// The get read lock.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public static void GetReadLock(ReaderWriterLockSlim locks)
    {
      bool lockAcquired = false;
      while (!lockAcquired)
      {
        lockAcquired = locks.TryEnterUpgradeableReadLock(1);
      }
    }

    /// <summary>
    /// The get read only lock.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public static void GetReadOnlyLock(ReaderWriterLockSlim locks)
    {
      bool lockAcquired = false;
      while (!lockAcquired)
      {
        lockAcquired = locks.TryEnterReadLock(1);
      }
    }

    /// <summary>
    /// The get write lock.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public static void GetWriteLock(ReaderWriterLockSlim locks)
    {
      bool lockAcquired = false;
      while (!lockAcquired)
      {
        lockAcquired = locks.TryEnterWriteLock(1);
      }
    }

    /// <summary>
    /// The release read only lock.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public static void ReleaseReadOnlyLock(ReaderWriterLockSlim locks)
    {
      if (locks.IsReadLockHeld)
      {
        locks.ExitReadLock();
      }
    }

    /// <summary>
    /// The release read lock.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public static void ReleaseReadLock(ReaderWriterLockSlim locks)
    {
      if (locks.IsUpgradeableReadLockHeld)
      {
        locks.ExitUpgradeableReadLock();
      }
    }

    /// <summary>
    /// The release write lock.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public static void ReleaseWriteLock(ReaderWriterLockSlim locks)
    {
      if (locks.IsWriteLockHeld)
      {
        locks.ExitWriteLock();
      }
    }

    /// <summary>
    /// The release lock.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public static void ReleaseLock(ReaderWriterLockSlim locks)
    {
      ReleaseWriteLock(locks);
      ReleaseReadLock(locks);
      ReleaseReadOnlyLock(locks);
    }

    /// <summary>
    /// The get lock instance.
    /// </summary>
    /// <returns>
    /// </returns>
    public static ReaderWriterLockSlim GetLockInstance()
    {
      return GetLockInstance(LockRecursionPolicy.SupportsRecursion);
    }

    /// <summary>
    /// The get lock instance.
    /// </summary>
    /// <param name="recursionPolicy">
    /// The recursion policy.
    /// </param>
    /// <returns>
    /// </returns>
    public static ReaderWriterLockSlim GetLockInstance(LockRecursionPolicy recursionPolicy)
    {
      return new ReaderWriterLockSlim(recursionPolicy);
    }
  }

  /// <summary>
  /// The base lock.
  /// </summary>
  public abstract class BaseLock : IDisposable
  {
    /// <summary>
    /// The _ locks.
    /// </summary>
    protected ReaderWriterLockSlim _Locks;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseLock"/> class.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public BaseLock(ReaderWriterLockSlim locks)
    {
      this._Locks = locks;
    }

    #region IDisposable Members

    /// <summary>
    /// The dispose.
    /// </summary>
    public abstract void Dispose();

    #endregion
  }


  /// <summary>
  /// The read lock.
  /// </summary>
  public class ReadLock : BaseLock
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadLock"/> class.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public ReadLock(ReaderWriterLockSlim locks)
      : base(locks)
    {
      Locks.GetReadLock(this._Locks);
    }


    /// <summary>
    /// The dispose.
    /// </summary>
    public override void Dispose()
    {
      Locks.ReleaseReadLock(this._Locks);
    }
  }


  /// <summary>
  /// The read only lock.
  /// </summary>
  public class ReadOnlyLock : BaseLock
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyLock"/> class.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public ReadOnlyLock(ReaderWriterLockSlim locks)
      : base(locks)
    {
      Locks.GetReadOnlyLock(this._Locks);
    }


    /// <summary>
    /// The dispose.
    /// </summary>
    public override void Dispose()
    {
      Locks.ReleaseReadOnlyLock(this._Locks);
    }
  }

  /// <summary>
  /// The write lock.
  /// </summary>
  public class WriteLock : BaseLock
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="WriteLock"/> class.
    /// </summary>
    /// <param name="locks">
    /// The locks.
    /// </param>
    public WriteLock(ReaderWriterLockSlim locks)
      : base(locks)
    {
      Locks.GetWriteLock(this._Locks);
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public override void Dispose()
    {
      Locks.ReleaseWriteLock(this._Locks);
    }
  }
}