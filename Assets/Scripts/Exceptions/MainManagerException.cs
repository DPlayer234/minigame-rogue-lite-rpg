namespace SAE.RoguePG.Exceptions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Thrown when there are any issues with the setup of a <see cref="Main.MainManager"/>
    /// </summary>
    [Serializable]
    public class MainManagerException : Exception
    {
        public MainManagerException() { }
        public MainManagerException(string message) : base(message) { }
        public MainManagerException(string message, Exception inner) : base(message, inner) { }
        protected MainManagerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}