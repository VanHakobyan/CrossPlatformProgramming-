﻿/* 
Copyright 2015 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetify
{
   public static class BaseVMExtensions
   {
      #region CRUD

      /// <summary>
      /// Used in CRUD operations to add a new item to the list.
      /// </summary>
      /// <typeparam name="T">Property type.</typeparam>
      /// <param name="expression">Expression containing property name of the list.</param>
      /// <param name="item">List item to be added.</param>
      public static void AddList<T>(this BaseVM vm, Expression<Func<T>> expression, object item)
      {
         var propName = ((MemberExpression)expression.Body).Member.Name;
         vm.AddList(propName, item);
      }

      public static void AddList<T>(this BaseVM vm, string propName, T item) => vm.ChangedProperties[propName + "_add"] = item;

      /// <summary>
      /// Used in CRUD operations to update an existing item on the list.
      /// </summary>
      /// <typeparam name="T">Property type.</typeparam>
      /// <param name="expression">Expression containing property name of the list.</param>
      /// <param name="item">List item to be updated.</param>
      public static void UpdateList<T>(this BaseVM vm, Expression<Func<T>> expression, object item)
      {
         var propName = ((MemberExpression)expression.Body).Member.Name;
         vm.UpdateList(propName, item);
      }

      public static void UpdateList<T>(this BaseVM vm, string propName, T item) => vm.ChangedProperties[propName + "_update"] = item;

      /// <summary>
      /// Used in CRUD operations to remove an item from a list.
      /// </summary>
      /// <typeparam name="T">Property type.</typeparam>
      /// <param name="expression">Expression containing property name of the list.</param>
      /// <param name="itemKey">Identifies the list item to be removed.</param>
      public static void RemoveList<T>(this BaseVM vm, Expression<Func<T>> expression, object itemKey)
      {
         var propName = ((MemberExpression)expression.Body).Member.Name;
         vm.RemoveList(propName, itemKey);
      }

      public static void RemoveList<T>(this BaseVM vm, string propName, T itemKey) => vm.ChangedProperties[propName + "_remove"] = itemKey;

      #endregion

      /// <summary>
      /// Raises PropertyChanged event.
      /// </summary>
      /// <param name="source">Event source.</param>
      /// <param name="propertyName">Property that changed.</param>
      public static INotifyPropertyChanged Changed(this INotifyPropertyChanged source, string propertyName) =>
         RaiseEvent(source, nameof(INotifyPropertyChanged.PropertyChanged), new PropertyChangedEventArgs(propertyName));

      /// <summary>
      /// Raises RequestPushUpdates event.
      /// </summary>
      /// <param name="source">Event source.</param>
      /// <param name="propertyName">Property that changed.</param>
      public static void PushUpdates(this IPushUpdates source) => RaiseEvent(source, nameof(IPushUpdates.RequestPushUpdates), EventArgs.Empty);

      /// <summary>
      /// Raises event using reflection.
      /// </summary>
      /// <param name="source">Event source.</param>
      /// <param name="eventName">Event name.</param>
      /// <param name="eventArgs">Event arguments.</param>
      internal static T RaiseEvent<T, TEventArgs>(this T source, string eventName, TEventArgs eventArgs) where TEventArgs : EventArgs
      {
         var eventDelegate = (MulticastDelegate)source
            .GetType()
            .GetTypeInfo()
            .GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(source);

         foreach (var handler in eventDelegate?.GetInvocationList())
            handler.GetMethodInfo().Invoke(handler.Target, new object[] { source, eventArgs });

         return source;
      }
   }
}
