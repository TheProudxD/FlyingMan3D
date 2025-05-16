using System.Threading.Tasks;
using UnityEngine;

namespace UnityUtils
{
    public static class AsyncOperationExtensions
    {
        /// <summary>
        /// Extension method that converts an AsyncOperation into a Task.
        /// </summary>
        /// <param name="asyncOperation">The AsyncOperation to convert.</param>
        /// <returns>A Task that represents the completion of the AsyncOperation.</returns>
        public static Task AsTask(this AsyncOperation asyncOperation)
        {
            var tcs = new TaskCompletionSource<bool>();
            asyncOperation.completed += a => tcs.SetResult(true);
            return tcs.Task;
        }


        public static Task<T> AsTask<T>(this ResourceRequest request) where T : Object
        {
            var tcs = new TaskCompletionSource<T>();

            request.completed += _ =>
            {
                if (request.asset == null)
                {
                    tcs.SetException(new System.Exception($"Failed to load resource of type {typeof(T)}"));
                }
                else
                {
                    tcs.SetResult((T)request.asset);
                }
            };

            return tcs.Task;
        }
    }
}