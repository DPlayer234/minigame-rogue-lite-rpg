namespace SAE.RoguePG
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     An exception in relation to the project.
    /// </summary>
    [Serializable]
    public class RPGException : Exception
    {
        /// <summary>
        ///     The exception cause
        /// </summary>
        public Cause cause;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RPGException"/> class.
        /// </summary>
        public RPGException()
        {
            this.cause = Cause.Unknown;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RPGException"/> class.
        ///     Uses a cause to display an error message.
        /// </summary>
        public RPGException(Cause cause) : base(RPGException.ErrorMessages[cause])
        {
            this.cause = cause;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RPGException"/> class.
        ///     Uses a cause to display an error message and appends a custom message.
        /// </summary>
        public RPGException(Cause cause, string message) : base(RPGException.ErrorMessages[cause] + message)
        {
            this.cause = cause;
        }

        /// <summary>
        ///     Initializes the static fields and properites of the <see cref="RPGException"/> class.
        /// </summary>
        static RPGException()
        {
            RPGException.ErrorMessages = new Dictionary<Cause, string>
            {
                // Error Messages
                { Cause.Unknown, "Unknown error." },

                { Cause.DungeonNoPlayerSpawnPoint, "There is no player spawn point!" },
                { Cause.DungeonNoActiveInstance, "There is no active DungeonGenerator instance!" },
                { Cause.DungeonNoFloorTransition, "There is no floor transition set!" },

                { Cause.StatInvalid, "The given stat is invalid." },

                { Cause.MainManagerNoCamera, "There is no Main Camera set!" },
                { Cause.MainManagerNoActiveInstance, "There is no active MainManager instance!" },
                { Cause.BattleManagerNoActiveInstance, "There is no active BattleManager instance!" },

                { Cause.MiniMapNoInstance, "There is no MiniMap instance!" },

                { Cause.MenuNoPause, "There is no pause menu attached to the PauseMenu!" },
                { Cause.MenuMissingComponent, "The menu does not have all required components! Please check the setup." },

                { Cause.SpriteNoRootHierarchy, "This GameObject is lacking a Sprite Root/Hierarchy." },
                { Cause.SpriteNoBody, "This GameObject is lacking a Sprite Body." },

                { Cause.BattleDriverNotLeader, "This function can only be called on the leader of a party. (IsLeader)" },

                { Cause.HighlightNoLight, "There is no Light for this Highlight." },
                { Cause.HighlightOnGameObject, "The Highlight Component must be attached to a parenting GameObject." },
                { Cause.HighlightNotFound, "There was no Highlight found." },

                { Cause.StatusDisplayMissingComponent, "The StatusDisplay is missing a component! Please check the setup." },
                { Cause.StatusDisplayNoBattleDriver, "The StatusDisplay has no assigned BattleDriver!" },
        };
        }

        /// <summary>
        ///     Defines the exception cause
        /// </summary>
        /// This is the longest enum I've written so far.
        public enum Cause
        {
            Unknown,

            DungeonNoPlayerSpawnPoint,
            DungeonNoActiveInstance,
            DungeonNoFloorTransition,

            StatInvalid,

            MainManagerNoCamera,
            MainManagerNoActiveInstance,
            BattleManagerNoActiveInstance,

            MiniMapNoInstance,

            MenuNoPause,
            MenuMissingComponent,

            SpriteNoRootHierarchy,
            SpriteNoBody,

            BattleDriverNotLeader,

            HighlightNoLight,
            HighlightOnGameObject,
            HighlightNotFound,

            StatusDisplayMissingComponent,
            StatusDisplayNoBattleDriver
        }

        /// <summary>
        ///     Associates every <seealso cref="Cause"/> with an error message
        /// </summary>
        private static Dictionary<Cause, string> ErrorMessages { get; set; }
    }
}