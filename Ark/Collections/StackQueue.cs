namespace Ark;

#nullable enable

/// <summary>
/// Provides both stack and queue operations using a single <see cref="System.Collections.Generic.LinkedList{T}"/> storage.
/// <para>+ Supports LIFO and FIFO retrieval in O(1) time.</para>
/// <para>- Not thread-safe; synchronize access in multi-threaded scenarios.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.generic.linkedlist-1"/></para>
/// </summary>
/// <typeparam name="T">Type of elements stored in the collection.</typeparam>
/// <example>
/// <code language="csharp">
/// var sq = new StackQueue&lt;int&gt;();
/// sq.Push(1);
/// sq.Enqueue(2);
/// var fromStack = sq.Pop();     // 1
/// var fromQueue = sq.Dequeue(); // 2
/// </code>
/// </example>
public class StackQueue<T>
{
    #region Fields
    private readonly LinkedList<T?> _linkedList = new();
    #endregion Fields

    #region Properties
    /// <summary>
    /// Gets the number of elements contained in the collection.
    /// <para>+ Provides O(1) access to the current count.</para>
    /// <para>- Value may change during enumeration in multithreaded scenarios.</para>
    /// </summary>
    public int Count => _linkedList.Count;
    #endregion Properties

    #region Methods
    /// <summary>
    /// Removes all elements from the collection.
    /// <para>+ Leaves the instance ready for reuse without allocating a new list.</para>
    /// <para>- All references held by items are released.</para>
    /// </summary>
    public void Clear() => _linkedList.Clear();

    /// <summary>
    /// Inserts an item at the top of the stack.
    /// <para>+ O(1) operation suitable for frequent pushes.</para>
    /// <para>- Item also becomes the next dequeued element.</para>
    /// </summary>
    /// <param name="item">The item to push onto the stack.</param>
    public void Push(T? item) => _linkedList.AddFirst(item);

    /// <summary>
    /// Adds an item to the queue.
    /// <para>+ O(1) operation sharing storage with the stack.</para>
    /// <para>- Ordering semantics depend on prior pushes.</para>
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    public void Enqueue(T? item) => _linkedList.AddFirst(item);

    /// <summary>
    /// Removes and returns the item at the top of the stack.
    /// <para>+ Retrieves the most recently pushed item.</para>
    /// <para>- Throws <see cref="System.InvalidOperationException"/> if the collection is empty.</para>
    /// </summary>
    /// <returns>The last pushed item, serialized using the default formatting of <typeparamref name="T"/>.</returns>
    public T Pop()
    {
        var obj = default(T);
        if (_linkedList.First is not null)
        {
            obj = _linkedList.First.Value;
        }

        _linkedList.RemoveFirst();
        return obj!;
    }

    /// <summary>
    /// Removes and returns the item at the head of the queue.
    /// <para>+ Retrieves the oldest enqueued element.</para>
    /// <para>- Throws <see cref="System.InvalidOperationException"/> if the collection is empty.</para>
    /// </summary>
    /// <returns>The item dequeued, serialized using the default formatting of <typeparamref name="T"/>.</returns>
    public T Dequeue()
    {
        var obj = default(T);
        if (_linkedList.Last is not null)
        {
            obj = _linkedList.Last.Value;
        }

        _linkedList.RemoveLast();
        return obj!;
    }

    /// <summary>
    /// Returns the item at the top of the stack without removing it.
    /// <para>+ Allows inspection without mutation.</para>
    /// <para>- Returns <see langword="null"/> if the collection is empty.</para>
    /// </summary>
    /// <returns>The last pushed item or <see langword="null"/> if the collection is empty.</returns>
    public T? PeekStack() => _linkedList.First is not null ? _linkedList.First.Value : default;

    /// <summary>
    /// Returns the item at the head of the queue without removing it.
    /// <para>+ Useful for lookahead checks.</para>
    /// <para>- Returns <see langword="null"/> if the collection is empty.</para>
    /// </summary>
    /// <returns>The item at the tail or <see langword="null"/> if the collection is empty.</returns>
    public T? PeekQueue() => _linkedList.Last is not null ? _linkedList.Last.Value : default;
    #endregion Methods
}
