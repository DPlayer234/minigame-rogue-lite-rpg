//-----------------------------------------------------------------------
// <copyright file="DungeonGeneratorRooms.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Main.Dungeon
{
    using System.Collections;
    using System.Collections.Generic;
    using DPlay.RoguePG.Extension;
    using DPlay.RoguePG.Main.BattleDriver;
    using DPlay.RoguePG.Main.Camera;
    using DPlay.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Generates a floor when attached to a GameObject.
    ///     Rooms are generated on a grid.
    /// </summary>
    public partial class DungeonGenerator
    {
        /// <summary>
        ///     Amount of Rooms: |RoomCountMultiplier * floorNumber + RoomCountAdd|
        /// </summary>
        private const float RoomCountMultiplier = 1.33f;

        /// <summary>
        ///     Amount of Rooms: |RoomCountMultiplier * floorNumber + RoomCountAdd|
        /// </summary>
        private const float RoomCountAdd = 3.0f;

        /// <summary>
        ///     Variation to the relative floor size.
        /// </summary>
        private const float RoomCountVariation = 0.1f;

        /// <summary>
        ///     The amount of treasure rooms in relation to regular rooms.
        /// </summary>
        private const float TreasureRoomMultiplier = 0.05f;

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
        ///     Assigns every <seealso cref="RoomType"/> to
        ///     an array <seealso cref="GameObject"/> with the matching rooms.
        /// </summary>
        private Dictionary<RoomType, GameObject[]> typeToPrefabs;

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
        ///     The wall object "blocking" the floor transition
        /// </summary>
        private GameObject floorTransitionBlockingWall;

        /// <summary> Parent transform for the dungeon rooms and co. </summary>
        private Transform roomParent;

        /// <summary>
        ///     Gets the size of this floor.
        ///     The first access will add a little variation.
        /// </summary>
        private int TotalFloorSize
        {
            get
            {
                // Generate value on first demand
                return this.totalFloorSize < 0 ?
                    this.totalFloorSize =
                        DungeonGenerator.GetTotalFloorSize(
                            this.floorNumber,
                            Random.Range(
                                1.0f - DungeonGenerator.RoomCountVariation,
                                1.0f + DungeonGenerator.RoomCountVariation)) :
                    this.totalFloorSize;
            }
        }

        /// <summary>
        ///     Gets the size of a floor in <paramref name="floorNumber"/> without variation
        /// </summary>
        /// <param name="floorNumber">The floor number</param>
        /// <returns>The floor size</returns>
        public static int GetTotalFloorSize(int floorNumber)
        {
            return DungeonGenerator.GetTotalFloorSize(floorNumber, 1.0f);
        }

        /// <summary>
        ///     Gets the size of a floor in <paramref name="floorNumber"/> without variation
        /// </summary>
        /// <param name="floorNumber">The floor number</param>
        /// <param name="multiplier">Size multiplier</param>
        /// <returns>The floor size</returns>
        public static int GetTotalFloorSize(int floorNumber, float multiplier)
        {
            return Mathf.FloorToInt(
                (floorNumber * DungeonGenerator.RoomCountMultiplier + DungeonGenerator.RoomCountAdd) * multiplier);
        }

        /// <summary>
        ///     Defines the floor layout
        /// </summary>
        private void DefineLayout()
        {
            this.floorLayout = new Dictionary<Vector2Int, RoomType>();

            Vector2Int coordinate = Vector2Int.zero;
            this.floorLayout[Vector2Int.zero] = RoomType.Start;

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

            this.AddSpecialRoomToLayout(validSpecialLocations, RoomType.Boss);

            for (int i = 0; i < Mathf.Max(1, (int)(this.TotalFloorSize * DungeonGenerator.TreasureRoomMultiplier)); i++)
            {
                this.AddSpecialRoomToLayout(validSpecialLocations, RoomType.Treasure);
            }
        }

        /// <summary>
        ///     Adds a given special room to the layout
        /// </summary>
        /// <param name="validSpecialLocations">A list of valid locations for special rooms</param>
        /// <param name="roomType">The room type</param>
        /// <returns>The position of the special room</returns>
        private void AddSpecialRoomToLayout(List<Vector2Int> validSpecialLocations, RoomType roomType)
        {
            Vector2Int roomPosition = validSpecialLocations.GetRandomItem();
            validSpecialLocations.Remove(roomPosition);

            this.floorLayout[roomPosition] = roomType;
        }

        /// <summary>
        ///     Returns whether the given position is valid for a special room
        /// </summary>
        /// <param name="position">The position on the grid</param>
        /// <returns>Whether the given position is valid for a special room</returns>
        private bool IsValidSpecialRoomLocation(Vector2Int position)
        {
            if (this.floorLayout.ContainsKey(position)) return false;

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
            this.typeToPrefabs = new Dictionary<RoomType, GameObject[]>(4);
            this.typeToPrefabs[RoomType.Start] = this.parts.startingRooms;
            this.typeToPrefabs[RoomType.Common] = this.parts.commonRooms;
            this.typeToPrefabs[RoomType.Boss] = this.parts.bossRooms;
            this.typeToPrefabs[RoomType.Treasure] = this.parts.treasureRooms;

            int index = 0;
            foreach (KeyValuePair<Vector2Int, RoomType> roomData in this.floorLayout)
            {
                GameObject room = this.SpawnRoom(roomData.Key, roomData.Value);

                room.name = room.name + "#" + index++;
            }
        }

        /// <summary>
        ///     Spawns a single room and its walls
        /// </summary>
        /// <param name="position">The position of the room on the grid</param>
        /// <param name="roomType">The type of the room</param>
        private GameObject SpawnRoom(Vector2Int position, RoomType roomType)
        {
            GameObject newRoom = MonoBehaviour.Instantiate(this.typeToPrefabs[roomType].GetRandomItem(), this.roomParent);

            // Set room position
            newRoom.transform.position = new Vector3(
                position.x * this.roomSize.x,
                0.0f,
                position.y * this.roomSize.y);

            List<GameObject> walls = this.AddWalls(newRoom, position);

            // Define the floor transition position
            if (roomType == RoomType.Boss)
            {
                this.floorTransitionBlockingWall = walls.GetRandomItem();

                // Unparent it, so it doesn't cause any trouble later on
                this.floorTransitionBlockingWall.transform.parent = this.roomParent;
            }

            ActivityHandler.Add(newRoom.GetComponentsInChildren<Light>());

            return newRoom;
        }

        /// <summary>
        ///     Adds walls to a room at the given position.
        /// </summary>
        /// <param name="room">The room</param>
        /// <param name="position">The position on the grid</param>
        /// <returns>A list of walls</returns>
        private List<GameObject> AddWalls(GameObject room, Vector2Int position)
        {
            // Spawns walls
            List<GameObject> walls = new List<GameObject>(4);

            foreach (Vector2Int offset in DungeonGenerator.roomOffsets)
            {
                if (!this.floorLayout.ContainsKey(position + offset))
                {
                    GameObject newWall = MonoBehaviour.Instantiate(this.parts.wall, room.transform);

                    newWall.transform.localPosition = Vector3.zero;
                    newWall.transform.forward = new Vector3(
                        offset.x,
                        0.0f,
                        offset.y);

                    walls.Add(newWall);
                }
            }

            return walls;
        }

        /// <summary>
        ///     Applies the dungeon design to all rooms
        /// </summary>
        private void ApplyDesign()
        {
            this.ApplyDesign(this.roomParent);
        }

        /// <summary>
        ///     Applies the dungeon design to <paramref name="transform"/> and its children
        /// </summary>
        /// <param name="transform">The transform parent</param>
        private void ApplyDesign(Transform transform)
        {
            this.design = this.designs.GetRandomItem();

            foreach (Light light in transform.GetComponentsInChildren<Light>())
            {
                if (light.type == LightType.Point)
                {
                    light.intensity = this.design.pointLightIntensity;
                }
            }

            foreach (MeshRenderer meshRenderer in transform.GetComponentsInChildren<MeshRenderer>())
            {
                foreach (var tagMaterialPair in this.design.materials)
                {
                    try
                    {
                        if (meshRenderer.CompareTag(tagMaterialPair.tag))
                        {
                            meshRenderer.material = tagMaterialPair.material;
                            break;
                        }
                    }
                    catch (System.Exception e)
                    {
                        // Might throw an error due to an invalid tag.
                        // Log that but do not interrupt program flow.
                        Debug.LogError(e);
                    }
                }
            }
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
}