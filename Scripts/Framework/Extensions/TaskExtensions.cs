using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Extensions
{
    public static class TaskExtensions
    {
        public static async void FireAndForget(this Task task)
        {
            if (task == null)
                return;

            try
            {
                await task.ConfigureAwait(true);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
