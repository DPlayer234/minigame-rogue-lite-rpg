using System;
using System.Collections.Generic;

namespace SAE.RoguePG.Exceptions
{
    /// <summary>
    ///     Thrown when there are any issues with the setup of a <see cref="Main.EntityDriver"/> or similar components
    /// </summary>
    [Serializable]
    public class DungeonGeneratorException : Exception
    {
        public DungeonGeneratorException() { }
        public DungeonGeneratorException(string message) : base(message) { }
        public DungeonGeneratorException(string message, Exception inner) : base(message, inner) { }
        protected DungeonGeneratorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}