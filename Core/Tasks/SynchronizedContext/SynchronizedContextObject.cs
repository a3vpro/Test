using System;
using System.Threading;
using System.Threading.Tasks;

namespace VisionNet.Core.Tasks
{
    /// <summary>
    /// Provides a base implementation that captures a <see cref="SynchronizationContext"/> and offers helpers
    /// to marshal delegate execution back onto that original context to maintain thread affinity guarantees.
    /// </summary>
    public abstract class SynchronizedContextObject
    {
        private SynchronizationContext _syncContext;

        /// <summary>
        /// Captures the current <see cref="SynchronizationContext"/> so later operations can be marshalled back to it.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when there is no current synchronization context to capture.</exception>
        protected void CaptureContext()
        {
            _syncContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current es null. Asegúrate de que CaptureContext se llama desde un hilo con un contexto de sincronización válido.");
        }

        /// <summary>
        /// Executes the specified <paramref name="action"/> on the captured synchronization context, synchronously if already on that context, otherwise by posting it asynchronously.
        /// </summary>
        /// <param name="action">Delegate to execute on the captured synchronization context.</param>
        /// <exception cref="InvalidOperationException">Thrown when the context has not been captured prior to invocation.</exception>
        protected void InvokeInOriginalContext(Action action)
        {
            if (_syncContext == null)
                throw new InvalidOperationException("CaptureContext no ha sido llamado. Debes capturar el contexto antes de invocar métodos en él.");

            if (_syncContext == SynchronizationContext.Current)
            {
                // Ejecuta la acción directamente si ya estamos en el hilo correcto
                action();
            }
            else
            {
                // Postea la acción al hilo correcto sin esperar un resultado
                _syncContext.Post(_ => action(), null);
            }
        }

        /// <summary>
        /// Executes the specified <paramref name="func"/> on the captured synchronization context and returns its result,
        /// running inline when already on that context or posting and blocking for completion otherwise.
        /// </summary>
        /// <typeparam name="T">Type of the value returned by <paramref name="func"/>.</typeparam>
        /// <param name="func">Delegate whose result is required from the captured synchronization context.</param>
        /// <returns>The value produced by <paramref name="func"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the context has not been captured prior to invocation.</exception>
        protected T InvokeInOriginalContext<T>(Func<T> func)
        {
            if (_syncContext == null)
                throw new InvalidOperationException("CaptureContext no ha sido llamado. Debes capturar el contexto antes de invocar métodos en él.");

            if (_syncContext == SynchronizationContext.Current)
            {
                // Ejecuta la función directamente si ya estamos en el hilo correcto y devuelve el resultado
                return func();
            }
            else
            {
                // Utiliza un TaskCompletionSource para esperar el resultado de forma asincrónica
                var tcs = new TaskCompletionSource<T>();
                _syncContext.Post(_ =>
                {
                    try
                    {
                        // Ejecuta la función y establece el resultado
                        T result = func();
                        tcs.SetResult(result);
                    }
                    catch (Exception e)
                    {
                        // Maneja cualquier excepción que pueda ocurrir
                        tcs.SetException(e);
                    }
                }, null);

                // Espera y devuelve el resultado. Ten en cuenta que esto bloqueará el hilo si se espera el resultado inmediatamente.
                return tcs.Task.Result;
            }
        }
    }
}