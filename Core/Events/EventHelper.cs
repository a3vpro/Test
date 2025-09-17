using System;
using System.Threading.Tasks;
using VisionNet.Core.Exceptions;

namespace VisionNet.Core.Events
{
    /// <summary>
    /// Provides utility methods for advanced event handling, allowing custom actions for each subscriber and transformation of event arguments.
    /// </summary>
    public static class EventHelper
    {
        /// <summary>
        /// Invokes a custom action for each subscriber of an event, allowing individualized handling.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="eventHandler">The event handler that contains the list of subscribers.</param>
        /// <param name="customAction">The custom action to execute for each subscriber. It takes the current subscriber as an argument.</param>
        /// <param name="onException">An optional action that runs if an exception occurs during invocation of a subscriber. It takes the exception as an argument.</param>
        public static void InvokeForEachSubscriber<TEventArgs>(
            EventHandler<TEventArgs> eventHandler,
            Action<EventHandler<TEventArgs>> customAction,
            Action<Exception> onException = null)
            where TEventArgs : EventArgs
        {
            if (eventHandler == null)
                return;

            // Get the list of subscribers to the event
            var subscribers = eventHandler.GetInvocationList();
            foreach (EventHandler<TEventArgs> subscriber in subscribers)
                try
                {
                    // Execute the custom action for the current subscriber
                    customAction(subscriber);
                }
                catch (Exception ex)
                {
                    // If provided, execute the exception handling action
                    onException?.Invoke(ex);
                }
        }

        /// <summary>
        /// Invokes a custom action for each subscriber of an event, allowing individualized handling.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="eventHandler">The event handler that contains the list of subscribers.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event arguments to pass to each subscriber.</param>
        /// <param name="customAction">The custom action to execute for each subscriber. It takes the current subscriber, the sender, and the event arguments as arguments.</param>
        /// <param name="onException">An optional action that runs if an exception occurs during invocation of a subscriber. It takes the exception as an argument.</param>
        public static void InvokeForEachSubscriber<TEventArgs>(
            EventHandler<TEventArgs> eventHandler,
            object sender,
            TEventArgs args,
            Action<EventHandler<TEventArgs>, object, TEventArgs> customAction,
            Action<Exception> onException = null)
            where TEventArgs : EventArgs
        {
            if (eventHandler == null)
                return;

            // Get the list of subscribers to the event
            var subscribers = eventHandler.GetInvocationList();
            foreach (EventHandler<TEventArgs> subscriber in subscribers)
                try
                {
                    // Execute the custom action for the current subscriber
                    customAction(subscriber, sender, args);
                }
                catch (Exception ex)
                {
                    // If provided, execute the exception handling action
                    InvokeException(onException, ex);
                }
        }

        /// <summary>
        /// Invokes each subscriber of an event with transformed arguments provided by a function, allowing modification of the event arguments for each subscriber.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="eventHandler">The event handler that contains the list of subscribers.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The original event arguments.</param>
        /// <param name="getEventArgs">A function that transforms the event arguments. It takes the original arguments and returns the transformed arguments.</param>
        /// <param name="onException">An optional action that runs if an exception occurs during invocation of a subscriber. It takes the exception as an argument.</param>
        public static void InvokeWithCustomEventArgs<TEventArgs>(
            EventHandler<TEventArgs> eventHandler,
            object sender,
            Func<TEventArgs> getEventArgs,
            Action<Exception> onException = null)
            where TEventArgs : EventArgs
        {
            if (eventHandler == null)
                return;

            // Get the list of subscribers to the event
            var subscribers = eventHandler.GetInvocationList();
            foreach (EventHandler<TEventArgs> subscriber in subscribers)
                try
                {
                    // Transform the event args for this subscriber
                    TEventArgs eventArgs = getEventArgs();

                    // Invoke the subscriber with the transformed event args
                    subscriber.Invoke(sender, eventArgs);
                }
                catch (Exception ex)
                {
                    // If provided, execute the exception handling action
                    InvokeException(onException, ex);
                }
        }

        /// <summary>
        /// Invokes each subscriber of an event with transformed arguments provided by a function, allowing modification of the event arguments for each subscriber.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="eventHandler">The event handler that contains the list of subscribers.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The original event arguments.</param>
        /// <param name="getEventArgs">A function that transforms the event arguments. It takes the original arguments and returns the transformed arguments.</param>
        /// <param name="onException">An optional action that runs if an exception occurs during invocation of a subscriber. It takes the exception as an argument.</param>
        public static void InvokeWithCustomEventArgs<TEventArgs>(
            EventHandler<TEventArgs> eventHandler,
            object sender,
            TEventArgs args,
            Func<TEventArgs, TEventArgs> getEventArgs,
            Action<Exception> onException = null)
            where TEventArgs : EventArgs
        {
            if (eventHandler == null)
                return;

            // Get the list of subscribers to the event
            var subscribers = eventHandler.GetInvocationList();
            foreach (EventHandler<TEventArgs> subscriber in subscribers)
                try
                {
                    // Transform the event args for this subscriber
                    TEventArgs transformedEventArgs = getEventArgs(args);

                    // Invoke the subscriber with the transformed event args
                    subscriber.Invoke(sender, transformedEventArgs);
                }
                catch (Exception ex)
                {
                    // If provided, execute the exception handling action
                    InvokeException(onException, ex);
                }
        }

        /// <summary>
        /// Invokes each subscriber of an event with cloned arguments, allowing modification of the event arguments for each subscriber.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="eventHandler">The event handler that contains the list of subscribers.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The original event arguments.</param>
        /// <param name="onException">An optional action that runs if an exception occurs during invocation of a subscriber. It takes the exception as an argument.</param>
        public static void InvokeWithClonedEventArgs<TEventArgs>(
            EventHandler<TEventArgs> eventHandler,
            object sender,
            TEventArgs args,
            Action<Exception> onException = null)
            where TEventArgs : EventArgs, ICloneable
        {
            if (eventHandler == null)
                return;

            // Get the list of subscribers to the event
            var subscribers = eventHandler.GetInvocationList();
            foreach (EventHandler<TEventArgs> subscriber in subscribers)
                try
                {
                    // Clone the event args for this subscriber
                    TEventArgs clonedEventArgs = (TEventArgs)args.Clone();

                    // Invoke the subscriber with the cloned event args
                    subscriber.Invoke(sender, clonedEventArgs);
                }
                catch (Exception ex)
                {
                    // If provided, execute the exception handling action
                    InvokeException(onException, ex);
                }
        }

        /// <summary>
        /// Invokes each subscriber of an event asynchronously, handling exceptions optionally.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="eventHandler">The event handler that contains the list of subscribers.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event arguments to pass to each subscriber.</param>
        /// <param name="onException">An optional action that runs if an exception occurs during invocation of a subscriber. It takes the exception as an argument.</param>
        public static void InvokeAsynchronous<TEventArgs>(
            EventHandler<TEventArgs> eventHandler,
            object sender,
            TEventArgs args,
            Action<Exception> onException = null)
            where TEventArgs : EventArgs, ICloneable
        {
            if (eventHandler == null)
                return;

            // Get the list of subscribers to the event
            var subscribers = eventHandler.GetInvocationList();
            foreach (EventHandler<TEventArgs> subscriber in subscribers)
            {
                try
                {
                    // Clone the event args for this subscriber
                    TEventArgs clonedEventArgs = (TEventArgs)args.Clone();

                    // Invoke the subscriber asynchronously
                    Task.Run(() => subscriber.Invoke(sender, clonedEventArgs));
                }
                catch (Exception ex)
                {
                    // If provided, execute the exception handling action
                    InvokeException(onException, ex);
                }
            }
        }

        private static void InvokeException(Action<Exception> onException, Exception ex)
        {
            if (onException == null)
                ex.LogToConsole();
            else
                try
                {
                    onException.Invoke(ex);
                }
                catch
                {
                    ex.LogToConsole();
                }
        }
    }
}