using System.Collections.ObjectModel;

#nullable enable
namespace Ark;

/// <summary>
/// Provides an insertion-ordered set that removes duplicates using
/// <see cref="System.Collections.ObjectModel.KeyedCollection{TKey,TItem}"/> as its backing store.
/// <para>+ Maintains item order while preventing duplicates.</para>
/// <para>- Replacement of existing items requires additional lookups.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.objectmodel.keyedcollection-2"/></para>
/// </summary>
/// <typeparam name="T">Type of elements stored in the set.</typeparam>
/// <example>
/// <code language="csharp">
/// var set = new OrderedHashSet&lt;int&gt;(keepOldest: false);
/// set.Add(1);
/// set.Add(1); // replaces previous item
/// </code>
/// </example>
public class OrderedHashSet<T> : KeyedCollection<T, T>
    where T : notnull
{
    #region Fields
    private readonly bool _keepOldest;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderedHashSet{T}"/> class.
    /// <para>+ Allows choosing whether the oldest or newest duplicate is kept.</para>
    /// <para>- Not thread-safe; external synchronization required.</para>
    /// </summary>
    /// <param name="keepOldest">
    /// When <c>true</c>, the first encountered item is preserved; otherwise newer items replace existing ones.
    /// </param>
    public OrderedHashSet(bool keepOldest = true) => _keepOldest = keepOldest;
    #endregion Constructors

    #region Overrides
    /// <inheritdoc />
    protected override void InsertItem(int index, T item)
    {
        if (Dictionary == null)
        {
            base.InsertItem(index, item);
        }
        else
        {
            var exists = Dictionary.ContainsKey(item);

            // if we want to keep the newest, then we need to remove the old item and add the new one
            if (exists == false)
            {
                base.InsertItem(index, item);
            }
            else if (_keepOldest == false)
            {
                if (Remove(item))
                {
                    index--;
                }

                base.InsertItem(index, item);
            }
        }
    }

    /// <inheritdoc />
    protected override T GetKeyForItem(T item) => item;
    #endregion Overrides
}

