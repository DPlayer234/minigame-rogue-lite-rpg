namespace SAE.RoguePG.Exceptions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Thrown when there are any issues with the setup of a <see cref="Main.EntityDriver"/> or similar components
    /// </summary>
    [Serializable]
    public class MenuException : Exception
    {
        public MenuException() { }
        public MenuException(string message) : base(message) { }
        public MenuException(string message, Exception inner) : base(message, inner) { }
        protected MenuException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}