using System;
using System.Collections.Generic;

namespace SAE.RoguePG.Exceptions
{
    /// <summary>
    ///     Thrown when there are any issues with the setup of a <see cref="Main.EntityDriver"/> or similar components
    /// </summary>
    [Serializable]
    public class EntityDriverException : Exception
    {
        public EntityDriverException() { }
        public EntityDriverException(string message) : base(message) { }
        public EntityDriverException(string message, Exception inner) : base(message, inner) { }
        protected EntityDriverException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}