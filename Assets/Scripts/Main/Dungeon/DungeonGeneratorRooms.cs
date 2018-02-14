namespace SAE.RoguePG.Main.Dungeon
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Generates a floor when attached to a GameObject.
    ///     Rooms are generated on a grid.
    /// </summary>
    public partial class DungeonGenerator
    {
        /// <summary>
        ///     Lists possible offsets from one room to the next
        /// </summary>
        private static List<Vector2Int> roomOffsets = new List<Vector2Int>()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        /// <summary>
        ///     How many rooms were already generated?
        /// </summary>
        private int generatedRooms;

        /// <summary>
        ///     How many rooms does this floor have?
        /// </summary>
        private int totalFloorSize = -1;

        /// <summary>
        ///     How the floor is layed out.
        /// </summary>
        private Dictionary<Vector2Int, RoomType> floorLayout;

        /// <summary>
        ///     Position of the boss room in the grid
        /// </summary>
        private Vector2Int bossRoomPosition;

        /// <summary>
        ///     The wall object "blocking" the floor transition
        /// </summary>
        private GameObject floorTransitionBlockingWall;

        /// <summary> Parent transform for the dungeon rooms and co. </summary>
        private Transform roomParent;

        /// <summary>
        ///     How many rooms does this floor have?
        ///     Not necessarily equal to <seealso cref="DungeonGenerator.GetTotalFloorSize(int)"/>.
        /// </summary>
        private int TotalFloorSize
        {
            get
            {
                // Generate value on first demand
                return this.totalFloorSize < 0 ?
                    this.totalFloorSize = DungeonGenerator.GetTotalFloorSize(this.floorNumber) + Random.Range(-1, 1) :
                    this.totalFloorSize;
            }
        }

        /// <summary>
        ///     Defines common room types
        /// </summary>
        public enum RoomType
        {
            /// <summary> There is no room </summary>
            None = -1,

            /// <summary> The starting room </summary>
            Start,

            /// <summary> A room without any special features </summary>
            Common,

            /// <summary> The boss room, containing the boss and the path to the next floor </summary>
            Boss,

            /// <summary> A room with 'treasure' of some kind </summary>
            Treasure
        }

        /// <summary>
        ///     Defines the floor layout
        /// </summary>
        private void DefineLayout()
        {
            this.floorLayout = new Dictionary<Vector2Int, RoomType>();

            Vector2Int coordinate = new Vector2Int(0, 0);
            this.floorLayout[coordinate] = RoomType.Start;

            // Common Rooms
            for (int i = 0; i < this.TotalFloorSize; i++)
            {
                // Move room coordinate by 1 in any direction
                coordinate += DungeonGenerator.roomOffsets.GetRandomItem();

                // Skip iteration and repeat if the room was already set.
                if (this.floorLayout.ContainsKey(coordinate))
                {
                    --i;
                    continue;
                }

                this.floorLayout[coordinate] = RoomType.Common;
            }

            this.DefineSpecialRoomLayout();
        }

        /// <summary>
        ///     Defines the position of special rooms
        /// </summary>
        private void DefineSpecialRoomLayout()
        {
            // Gets any valid locations for 'special' room types (aka, empty and exactly one connection to other rooms)
            var validSpecialLocations = new List<Vector2Int>();

            foreach (KeyValuePair<Vector2Int, RoomType> item in this.floorLayout)
            {
                Vector2Int position = item.Key;

                foreach (Vector2Int offset in DungeonGenerator.roomOffsets)
                {
                    Vector2Int checkPosition = position + offset;

                    if (!validSpecialLocations.Contains(checkPosition) && this.IsValidSpecialRoomLocation(checkPosition))
                    {
                        validSpecialLocations.Add(checkPosition);
                    }
                }
            }

            this.bossRoomPosition = this.AddSpecialRoomToLayout(validSpecialLocations, RoomType.Boss);
        }

        /// <summary>
        ///     Adds a given special room to the layout
        /// </summary>
        /// <param name="validSpecialLocations">A list of valid locations for special rooms</param>
        /// <param name="roomType">The room type</param>
        /// <returns>The position of the special room</returns>
        private Vector2Int AddSpecialRoomToLayout(List<Vector2Int> validSpecialLocations, RoomType roomType)
        {
            Vector2Int roomPosition = validSpecialLocations.GetRandomItem();
            validSpecialLocations.Remove(roomPosition);

            this.floorLayout[roomPosition] = roomType;
            return roomPosition;
        }

        /// <summary>
        ///     Returns whether the given position is valid for a special room
        /// </summary>
        /// <param name="position">The position on the grid</param>
        /// <returns>Whether the given position is valid for a special room</returns>
        private bool IsValidSpecialRoomLocation(Vector2Int position)
        {
            int adjacent = 0;

            foreach (Vector2Int offset in DungeonGenerator.roomOffsets)
            {
                adjacent += this.floorLayout.ContainsKey(position + offset) ? 1 : 0;
            }

            return adjacent == 1;
        }

        /// <summary>
        ///     Spawns rooms where appropriate
        /// </summary>
        private void SpawnRooms()
        {
            // This is going to make assigning rooms easier.
            Dictionary<RoomType, GameObject[]> typeToPrefabs = new Dictionary<RoomType, GameObject[]>(4);
            typeToPrefabs[RoomType.Start] = this.parts.startingRooms;
            typeToPrefabs[RoomType.Common] = this.parts.commonRooms;
            typeToPrefabs[RoomType.Boss] = this.parts.bossRooms;
            typeToPrefabs[RoomType.Treasure] = this.parts.treasureRooms;

            foreach (KeyValuePair<Vector2Int, RoomType> item in this.floorLayout)
            {
                GameObject newRoom = Instantiate(typeToPrefabs[item.Value].GetRandomItem(), this.roomParent);

                newRoom.transform.position = new Vector3(
                    item.Key.x * this.roomSize.x,
                    0.0f,
                    item.Key.y * this.roomSize.y);

                this.limitedRangeBehaviours.AddRange(newRoom.GetComponentsInChildren<Light>());
            }
        }
    }
}