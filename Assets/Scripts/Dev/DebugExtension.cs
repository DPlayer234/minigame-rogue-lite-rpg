using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SAE.RoguePG.Dev
{
    /// <summary>
    ///     Includes extension methods for debugging
    /// </summary>
    public static class DebugExtension
    {
        /// <summary>
        ///     Logs text to the console and includes the MonoBehaviour's object name
        /// </summary>
        /// <param name="mono">The instance of the MonoBehaviour</param>
        /// <param name="format">The formatted string</param>
        /// <param name="args">The arguments to the formatted string</param>
        public static void LogThisAndFormat(this MonoBehaviour mono, string format, params object[] args)
        {
            Debug.Log("[" + mono.name + "] " + string.Format(format, args));
        }
    }
}
