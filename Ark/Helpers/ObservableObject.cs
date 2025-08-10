using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

// ReSharper disable ExplicitCallerInfoArgument

namespace Ark
{
    /// <summary>
    /// A base class for objects of which the properties must be observable.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The observers used by the Observe method.
        /// </summary>
        private Dictionary<string, ConcurrentDictionary<Action, object>> _observers;

        #endregion Fields

        #region INotifyPropertyChanged

        /// <summary>
        /// Event raised when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #region Methods (Public RaisePropertyChanged)

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("Property name must be given.", nameof(propertyName));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            _observers?.GetValue(propertyName)?.ForEach(action => action.Key.Invoke());
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property that changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property that changed.</param>
        public virtual void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression);
            if (string.IsNullOrEmpty(propertyName)) return;

            RaisePropertyChanged(propertyName);
        }

        #endregion Methods (Public RaisePropertyChanged)

        #region Methods (Public Observer)

        /// <summary>
        /// Observe a property and calls back an action when changed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to observe.</typeparam>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="getValueFunction">The function to get the value of the observer property.</param>
        /// <param name="callback">The callback method to invoke when property changed.</param>
        public Action Observe<TProperty>(string propertyName, Func<TProperty> getValueFunction, Action<TProperty> callback)
        {
            _observers = _observers ?? new Dictionary<string, ConcurrentDictionary<Action, object>>();
            void Action() => callback(getValueFunction());

            if (_observers.ContainsKey(propertyName)) _observers[propertyName].TryAdd(Action, null);
            else _observers.Add(propertyName, new ConcurrentDictionary<Action, object> { [Action] = null });

            return Action;
        }

        /// <summary>
        /// Call first and observe a property and calls back an action when changed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to observe.</typeparam>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="getValueFunction">The function to get the value of the observer property.</param>
        /// <param name="callback">The callback method to invoke when property changed.</param>
        public Action CallAndObserve<TProperty>(string propertyName, Func<TProperty> getValueFunction, Action<TProperty> callback)
        {
            var action = Observe(propertyName, getValueFunction, callback);
            action();

            return action;
        }

        /// <summary>
        /// Call first asynchronously and observe a property and calls back an action when changed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to observe.</typeparam>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="getValueFunction">The function to get the value of the observer property.</param>
        /// <param name="callback">The callback method to invoke when property changed.</param>
        public async Task<Action> CallAndObserveAsync<TProperty>(string propertyName, Func<TProperty> getValueFunction, Func<TProperty, System.Threading.Tasks.Task> callback)
        {
            async void CallbackSync(TProperty prop) => await callback(prop);
            var action = Observe(propertyName, getValueFunction, CallbackSync);
            await callback(getValueFunction());

            return action;
        }

        /// <summary>
        /// Stops to observe a property (remove all actions).
        /// </summary>
        /// <param name="propertyName">The name of the property to stop to observe.</param>
        /// <returns>True if found and successfully removed, false otherwise.</returns>
        public bool StopObserve(string propertyName)
        {
            return _observers.Remove(propertyName);
        }

        /// <summary>
        /// Stops to observe only an action callback of a property.
        /// </summary>
        /// <param name="propertyName">The name of the property to stop to observe.</param>
        /// <param name="action">The action coming back from the Observe method.</param>
        /// <returns>True if found and successfully removed, false otherwise.</returns>
        public bool StopObserve(string propertyName, Action action)
        {
            var actions = _observers.GetValue(propertyName);
            // ReSharper disable once UnusedVariable
            return actions?.TryRemove(action, out object none) ?? false;
        }

        /// <summary>
        /// Stops to observe all the properties.
        /// </summary>
        public void StopObserveAll()
        {
            _observers?.Clear();
            _observers = null;
        }

        #endregion Methods (Public Observer)

        #region Methods (Protected Set)

        /// <summary>
        /// Assigns a new value to the property. Then, raises the PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="TProperty">The type of the property that changed.</typeparam>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <returns>True if the PropertyChanged event has been raised, false otherwise. The event is not raised if the old value is equal to the new value.</returns>
        protected bool Set<TProperty>(string propertyName, ref TProperty field, TProperty newValue)
        {
            if (Equals(field, newValue)) return false;

            field = newValue;
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="TProperty">The type of the property that changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change  occurred.</param>
        /// <returns>True if the PropertyChanged event has been raised, false otherwise. The event is not raised if the old value is equal to the new value.</returns>
        protected bool Set<TProperty>(Expression<Func<TProperty>> propertyExpression, ref TProperty field, TProperty newValue)
        {
            if (Equals(field, newValue)) return false;

            field = newValue;
            RaisePropertyChanged(propertyExpression);
            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>True if the PropertyChanged event has been raised, false otherwise. The event is not raised if the old value is equal to the new value.</returns>
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            return Set(propertyName, ref field, newValue);
        }

        /// <summary>
        /// Sets the property value with other property raise event when changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The field to update.</param>
        /// <param name="newValue">The new value to set in the property.</param>
        /// <param name="otherPropertyNameToRaise">The other property to set as changed with a raise event.</param>
        /// <param name="propertyName">The name of the property (automatic).</param>
        /// <returns></returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        protected bool Set<T>(ref T field, T newValue, string otherPropertyNameToRaise, [CallerMemberName] string propertyName = null)
        {
            return Set(ref field, newValue, new[] { otherPropertyNameToRaise }, propertyName);
        }

        /// <summary>
        /// Sets the property value with other properties raise event when changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The field to update.</param>
        /// <param name="newValue">The new value to set in the property.</param>
        /// <param name="otherPropertiesNamesToRaise">The other properties to set as changed with a raise event.</param>
        /// <param name="propertyName">The name of the property (automatic).</param>
        /// <returns></returns>
        protected bool Set<T>(ref T field, T newValue, string[] otherPropertiesNamesToRaise, [CallerMemberName] string propertyName = null)
        {
            var hasChanged = Set(ref field, newValue, propertyName);
            if (hasChanged) otherPropertiesNamesToRaise?.ForEach(RaisePropertyChanged);

            return hasChanged;
        }

        #endregion Methods (Protected Set)

        #region Methods (Protected Helpers)

        /// <summary>
        /// Extracts the name of a property from an expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">An expression returning the property's name.</param>
        /// <returns>The name of the property returned by the expression.</returns>
        /// <exception cref="ArgumentNullException">If the expression is null.</exception>
        /// <exception cref="ArgumentException">If the expression does not represent a property.</exception>
        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));

            var body = propertyExpression.Body as MemberExpression;
            if (body == null) throw new ArgumentException("Invalid argument", nameof(propertyExpression));

            var property = body.Member as PropertyInfo;
            if (property == null) throw new ArgumentException("Argument is not a property", nameof(propertyExpression));

            return property.Name;
        }

        #endregion Methods (Protected Helpers)
    }

    /// <summary>
    /// This class extends the ObservableObject class to add some convenient helpers for Observe methods.
    /// </summary>
    public static class ObservableObjectExtensibility
    {
        #region Methods (Public)

        /// <summary>
        /// Observes this object property and executes the callback if needed. 
        /// </summary>
        /// <typeparam name="TObject">The type of the observable object.</typeparam>
        /// <typeparam name="TProperty">The type of the property to observe.</typeparam>
        /// <param name="obj">This observable object.</param>
        /// <param name="property">The expression to get the property name/value.</param>
        /// <param name="callback">The callback to call when there is </param>
        /// <returns>The action to get the property value and executes the callback. This is useful to stop to observe only this action.</returns>
        public static Action Observe<TObject, TProperty>(this TObject obj, Expression<Func<TObject, TProperty>> property, Action<TProperty> callback)
            where TObject : ObservableObject
        {
            var propertyInfo = (property.Body as MemberExpression)?.Member as PropertyInfo;
            if (propertyInfo == null) throw new ArgumentException("The lambda expression 'property' should point to a valid Property");

            var propertyName = propertyInfo.Name;
            var getValueFunction = new Func<TProperty>(() => property.Compile()(obj));

            return obj.Observe(propertyName, getValueFunction, callback);
        }

        /// <summary>
        /// Call first and observes this object property and executes the callback if needed.
        /// </summary>
        /// <typeparam name="TObject">The type of the observable object.</typeparam>
        /// <typeparam name="TProperty">The type of the property to observe.</typeparam>
        /// <param name="obj">This observable object.</param>
        /// <param name="property">The expression to get the property name/value.</param>
        /// <param name="callback">The callback to call when there is </param>
        /// <returns>The action to get the property value and executes the callback. This is useful to stop to observe only this action.</returns>
        public static Action CallAndObserve<TObject, TProperty>(this TObject obj, Expression<Func<TObject, TProperty>> property, Action<TProperty> callback)
            where TObject : ObservableObject
        {
            var action = Observe(obj, property, callback);
            action();

            return action;
        }

        #endregion Methods (Public)
    }
}