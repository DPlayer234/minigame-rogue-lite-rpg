namespace SAE.RoguePG.Exceptions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Thrown when there are any issues with the setup of a <see cref="UI.StatusDisplayException"/>
    /// </summary>
    [Serializable]
    public class StatusDisplayException : Exception
    {
        public StatusDisplayException() { }
        public StatusDisplayException(string message) : base(message) { }
        public StatusDisplayException(string message, Exception inner) : base(message, inner) { }
        protected StatusDisplayException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}