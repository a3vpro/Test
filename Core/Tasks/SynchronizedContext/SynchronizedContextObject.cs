using System;
using System.Threading;
using System.Threading.Tasks;

namespace VisionNet.Core.Tasks
{
    public abstract class SynchronizedContextObject
    {
        private SynchronizationContext _syncContext;

        protected void CaptureContext()
        {
            _syncContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current es null. Asegúrate de que CaptureContext se llama desde un hilo con un contexto de sincronización válido.");
        }

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