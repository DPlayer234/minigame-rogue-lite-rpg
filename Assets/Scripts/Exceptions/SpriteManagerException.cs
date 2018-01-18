﻿using System;
using System.Collections.Generic;

namespace SAE.RoguePG.Exceptions
{
    /// <summary>
    ///     Thrown when there are any issues with the setup of a <see cref="Main.SpriteManager3D"/>
    /// </summary>
    [Serializable]
    public class SpriteManagerException : Exception
    {
        public SpriteManagerException() { }
        public SpriteManagerException(string message) : base(message) { }
        public SpriteManagerException(string message, Exception inner) : base(message, inner) { }
        protected SpriteManagerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}