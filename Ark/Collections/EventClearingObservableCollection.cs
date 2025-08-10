using System.Collections.ObjectModel;
using System.Collections.Specialized;

#nullable enable

namespace Ark;

/// <summary>
/// Provides an <see cref="ObservableCollection{T}"/> that exposes a method to clear all
/// <see cref="INotifyCollectionChanged.CollectionChanged"/> subscriptions.
/// <para>+ Prevents memory leaks by explicitly removing event handlers.</para>
/// <para>- Not thread-safe; synchronize access in concurrent scenarios.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.objectmodel.observablecollection-1"/></para>
/// </summary>
/// <typeparam name="TValue">Type of elements contained in the collection.</typeparam>
/// <example>
    /// <code language="csharp">
    /// var coll = new EventClearingObservableCollection&lt;int&gt;();
/// coll.CollectionChanged += (s, e) => Console.WriteLine("changed");
/// coll.Add(1);
/// coll.ClearCollectionChangedEvents();
/// </code>
/// </example>
public class EventClearingObservableCollection<TValue> : ObservableCollection<TValue>, INotifyCollectionChanged, IDeepCloneable
{
        // need to explicitly implement with event accessor syntax in order to override in order to to clear
        // c# events are weird, they do not behave the same way as other c# things that are 'virtual',
        // a good article is here: https://medium.com/@unicorn_dev/virtual-events-in-c-something-went-wrong-c6f6f5fbe252
        // and https://stackoverflow.com/questions/2268065/c-sharp-language-design-explicit-interface-implementation-of-an-event
        private NotifyCollectionChangedEventHandler? _changed;

        /// <summary>
        /// Initializes a new empty instance of the collection.
        /// <para>+ Ready for item insertion.</para>
        /// <para>- Requires manual synchronization if shared across threads.</para>
        /// </summary>
        public EventClearingObservableCollection()
        {
        }

        /// <summary>
        /// Initializes the collection with the contents of the provided list.
        /// <para>+ Avoids enumerating when a <see cref="List{T}"/> is already available.</para>
        /// <para>- The list is copied; further changes to <paramref name="list"/> are not reflected.</para>
        /// </summary>
        /// <param name="list">Source list whose items populate the collection.</param>
        public EventClearingObservableCollection(List<TValue> list)
            : base(list)
        {
        }

        /// <summary>
        /// Initializes the collection with items from the supplied sequence.
        /// <para>+ Accepts any <see cref="IEnumerable{T}"/>.</para>
        /// <para>- Enumerates the sequence immediately.</para>
        /// </summary>
        /// <param name="collection">Sequence of items used to populate the collection.</param>
        public EventClearingObservableCollection(IEnumerable<TValue> collection)
            : base(collection)
        {
        }

        event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
        {
            add => _changed += value;
            remove => _changed -= value;
        }

        /// <summary>
        /// Removes all <see cref="INotifyCollectionChanged.CollectionChanged"/> event handlers.
        /// <para>+ Frees subscribers that no longer need notifications.</para>
        /// <para>- Subsequent changes will not notify previously attached listeners.</para>
        /// </summary>
        public void ClearCollectionChangedEvents() => _changed = null;

        /// <summary>
        /// Creates a deep clone of the collection and its items.
        /// <para>+ Utilises <see cref="DeepCloneHelper"/> to replicate items.</para>
        /// <para>- Requires items to implement <see cref="IDeepCloneable"/> for full fidelity.</para>
        /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone"/></para>
        /// </summary>
        /// <returns>A new <see cref="EventClearingObservableCollection{TValue}"/> containing cloned items.</returns>
        public object DeepClone()
        {
            var clone = new EventClearingObservableCollection<TValue>();
            DeepCloneHelper.CloneListItems<EventClearingObservableCollection<TValue>, TValue>(this, clone);

            return clone;
        }
    }

