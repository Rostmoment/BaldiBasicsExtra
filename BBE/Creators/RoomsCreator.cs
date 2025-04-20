using BBE.Extensions;
using PlusLevelLoader;
using MTM101BaldAPI;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MTM101BaldAPI.Registers;
using HarmonyLib;
using System.Linq;
using System.IO;
using BBE.NPCs;
using BBE.Helpers;
using PlusLevelFormat;
using System;
using BBE.NPCs.Chess;
using BBE.Rooms;
using UnityEngine.Audio;
using BBE.CustomClasses;

namespace BBE.Creators
{
    class RoomsCreator
    {
        private static void AddSpecialRoomToFloor(WeightedRoomAsset room, params string[] floors)
        {
            floors.Do(x => FloorData.Get(x).specialRooms.Add(room));
        }
        private static void AddSpecialRoomToFloor(RoomAsset room, int F1, int F2, int F3, int END)
        {
            if (F1 > 0)
                FloorData.Get("F1").specialRooms.Add(new WeightedRoomAsset() { selection = room, weight = F1 });
            if (F2 > 0)
                FloorData.Get("F2").specialRooms.Add(new WeightedRoomAsset() { selection = room, weight = F2 });
            if (F3 > 0)
                FloorData.Get("F3").specialRooms.Add(new WeightedRoomAsset() { selection = room, weight = F3 });
            if (END > 0)
                FloorData.Get("END").specialRooms.Add(new WeightedRoomAsset() { selection = room, weight = END });
        }
        // Sorry PixelGuy, you update the api a lot, so for stability I copied the code
        public static List<RoomAsset> GetAssetsFromLevelAsset(LevelAsset lvlAsset, string roomname, int minRoomRange, int maxRoomRange, int maxItemValue, bool isOffLimits, RoomFunctionContainer existingContainer = null, bool isASecretRoom = false, Texture2D mapBg = null, bool keepTextures = true, bool squaredShape = false)
        {
            List<RoomAsset> list = new List<RoomAsset>();
            for (int i = minRoomRange; i < maxRoomRange; i++)
            {
                RoomAsset roomAsset = ScriptableObject.CreateInstance<RoomAsset>();
                roomAsset.activity = lvlAsset.rooms[i].activity.GetNew();
                roomAsset.basicObjects = new List<BasicObjectData>(lvlAsset.rooms[i].basicObjects);
                roomAsset.blockedWallCells = new List<IntVector2>(lvlAsset.rooms[i].blockedWallCells);
                roomAsset.category = lvlAsset.rooms[i].category;
                roomAsset.type = lvlAsset.rooms[i].type;
                bool flag = roomAsset.type == RoomType.Hall;
                IntVector2 intVector = default(IntVector2);
                IntVector2 intVector2 = new IntVector2(lvlAsset.levelSize.x, lvlAsset.levelSize.z);
                CellData[] tile = lvlAsset.tile;
                foreach (CellData cellData in tile)
                {
                    if (cellData.roomId == i && cellData.type != 16)
                    {
                        if (intVector2.x > cellData.pos.x)
                        {
                            intVector2.x = cellData.pos.x;
                        }

                        if (intVector2.z > cellData.pos.z)
                        {
                            intVector2.z = cellData.pos.z;
                        }

                        if (flag)
                        {
                            cellData.type = 0;
                        }
                    }
                }

                Vector3 worldPosOffset = new Vector3(intVector2.x * 10f, 0f, intVector2.z * 10f);
                tile = lvlAsset.tile;
                foreach (CellData cellData2 in tile)
                {
                    if (cellData2.roomId == i && cellData2.type != 16)
                    {
                        cellData2.pos -= intVector2;
                        roomAsset.cells.Add(cellData2);
                        if (intVector.x < cellData2.pos.x)
                        {
                            intVector.x = cellData2.pos.x;
                        }

                        if (intVector.z < cellData2.pos.z)
                        {
                            intVector.z = cellData2.pos.z;
                        }
                    }
                }

                List<IntVector2> collection = roomAsset.cells.ConvertAll((CellData x) => x.pos);
                roomAsset.color = lvlAsset.rooms[i].color;
                roomAsset.doorMats = lvlAsset.rooms[i].doorMats;
                roomAsset.entitySafeCells = new List<IntVector2>(collection);
                roomAsset.eventSafeCells = new List<IntVector2>(collection);
                roomAsset.forcedDoorPositions = new List<IntVector2>(lvlAsset.rooms[i].forcedDoorPositions);
                roomAsset.hasActivity = lvlAsset.rooms[i].hasActivity;
                roomAsset.itemList = new List<WeightedItemObject>(lvlAsset.rooms[i].itemList);
                roomAsset.items = new List<ItemData>(lvlAsset.rooms[i].items);
                roomAsset.keepTextures = keepTextures;
                roomAsset.ceilTex = lvlAsset.rooms[i].ceilTex;
                roomAsset.florTex = lvlAsset.rooms[i].florTex;
                roomAsset.wallTex = lvlAsset.rooms[i].wallTex;
                roomAsset.mapMaterial = lvlAsset.rooms[i].mapMaterial;
                roomAsset.maxItemValue = maxItemValue;
                roomAsset.offLimits = isOffLimits;
                roomAsset.basicObjects.ForEach(delegate (BasicObjectData x)
                {
                    x.position -= worldPosOffset;
                });
                for (int k = 0; k < roomAsset.blockedWallCells.Count; k++)
                {
                    roomAsset.blockedWallCells[k] -= intVector2;
                }

                roomAsset.items.ForEach(delegate (ItemData x)
                {
                    x.position -= new Vector2(worldPosOffset.x, worldPosOffset.z);
                });
                roomAsset.activity.position -= worldPosOffset;
                for (int l = 0; l < roomAsset.basicObjects.Count; l++)
                {
                    IntVector2 gridPosition = IntVector2.GetGridPosition(roomAsset.basicObjects[l].position);
                    Vector3 position = roomAsset.basicObjects[l].position;
                    switch (roomAsset.basicObjects[l].prefab.name)
                    {
                        case "lightSpotMarker":
                            roomAsset.basicObjects.RemoveAt(l--);
                            roomAsset.standardLightCells.Add(gridPosition);
                            break;
                        case "itemSpawnMarker":
                            roomAsset.basicObjects.RemoveAt(l--);
                            roomAsset.itemSpawnPoints.Add(new ItemSpawnPoint
                            {
                                weight = 50,
                                position = new Vector2(position.x, position.z)
                            });
                            break;
                        case "nonSafeCellMarker":
                            if (!flag)
                            {
                                roomAsset.entitySafeCells.Remove(gridPosition);
                                roomAsset.eventSafeCells.Remove(gridPosition);
                            }

                            roomAsset.basicObjects.RemoveAt(l--);
                            break;
                    }
                }

                for (int m = 0; m < roomAsset.basicObjects.Count; m++)
                {
                    IntVector2 pos3 = IntVector2.GetGridPosition(roomAsset.basicObjects[m].position);
                    if (!(roomAsset.basicObjects[m].prefab.name == "potentialDoorMarker"))
                    {
                        continue;
                    }

                    roomAsset.basicObjects.RemoveAt(m--);
                    if (!flag)
                    {
                        roomAsset.potentialDoorPositions.Add(pos3);
                        if (!roomAsset.basicObjects.Any((BasicObjectData x) => IntVector2.GetGridPosition(x.position) == pos3))
                        {
                            roomAsset.blockedWallCells.Remove(pos3);
                        }
                    }
                }

                for (int n = 0; n < roomAsset.basicObjects.Count; n++)
                {
                    IntVector2 pos2 = IntVector2.GetGridPosition(roomAsset.basicObjects[n].position);
                    if (!(roomAsset.basicObjects[n].prefab.name == "forcedDoorMarker"))
                    {
                        continue;
                    }

                    roomAsset.basicObjects.RemoveAt(n--);
                    if (!flag)
                    {
                        roomAsset.forcedDoorPositions.Add(pos2);
                        if (!roomAsset.basicObjects.Any((BasicObjectData x) => IntVector2.GetGridPosition(x.position) == pos2))
                        {
                            roomAsset.blockedWallCells.Remove(pos2);
                        }
                    }
                }

                roomAsset.requiredDoorPositions = new List<IntVector2>(lvlAsset.rooms[i].requiredDoorPositions);
                if (isASecretRoom)
                {
                    roomAsset.secretCells.AddRange(roomAsset.cells.Select((CellData x) => x.pos));
                }
                else
                {
                    roomAsset.secretCells = new List<IntVector2>(lvlAsset.rooms[i].secretCells);
                    for (int num = 0; num < roomAsset.secretCells.Count; num++)
                    {
                        roomAsset.secretCells[num] -= intVector2;
                    }
                }

                roomAsset.name = $"Room_{roomAsset.category}_{roomname}{((maxRoomRange >= 2) ? string.Empty : ((object)i))}";
                ((UnityEngine.Object)roomAsset).name = roomAsset.name;
                if (existingContainer != null)
                {
                    roomAsset.roomFunctionContainer = existingContainer;
                }
                else if (!flag)
                {
                    RoomFunctionContainer roomFunctionContainer = new GameObject(roomAsset.name + "FunctionContainer").AddComponent<RoomFunctionContainer>();
                    roomFunctionContainer.functions = new List<RoomFunction>();
                    roomFunctionContainer.gameObject.ConvertToPrefab(setActive: true);
                    roomAsset.roomFunctionContainer = roomFunctionContainer;
                    existingContainer = roomFunctionContainer;
                }

                if (mapBg != null)
                {
                    roomAsset.mapMaterial = new Material(roomAsset.mapMaterial);
                    roomAsset.mapMaterial.SetTexture("_MapBackground", mapBg);
                    roomAsset.mapMaterial.shaderKeywords = new string[] { "_KEYMAPSHOWBACKGROUND_ON" };
                    roomAsset.mapMaterial.name = roomAsset.name;
                }
                else if (flag)
                {
                    roomAsset.mapMaterial = null;
                }

                if (!flag && squaredShape && intVector.z > 0 && intVector.x > 0)
                {
                    for (int num2 = 0; num2 <= intVector.x; num2++)
                    {
                        for (int num3 = 0; num3 <= intVector.z; num3++)
                        {
                            IntVector2 pos = new IntVector2(num2, num3);
                            if (!roomAsset.cells.Any((CellData x) => x.pos == pos))
                            {
                                roomAsset.cells.Add(new CellData
                                {
                                    pos = pos
                                });
                                roomAsset.secretCells.Add(pos);
                            }
                        }
                    }
                }

                list.Add(roomAsset);
            }

            return list;
        }
        public static List<RoomAsset> CreateAssetsFromPath(string path, int maxItemValue, bool isOffLimits, BinaryReader reader, RoomFunctionContainer existingContainer = null, bool isAHallway = false, bool isASecretRoom = false, Texture2D mapBg = null, bool keepTextures = true, bool squaredShape = false)
        {
            List<RoomAsset> result = new List<RoomAsset>();
            LevelAsset levelAsset = CustomLevelLoader.LoadLevelAsset(reader.ReadLevel());
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            try
            {
                result = GetAssetsFromLevelAsset(levelAsset, fileNameWithoutExtension, (!isAHallway) ? 1 : 0, isAHallway ? 1 : levelAsset.rooms.Count, maxItemValue, isOffLimits, existingContainer, isASecretRoom, mapBg, keepTextures, squaredShape);
                return result;
            }
            catch (Exception exception)
            {
                BasePlugin.Logger.LogWarning("Failed to load a room coming from the cbld: " + path);
                BasePlugin.Logger.LogError(exception);
                return result;
            }
            finally
            {
                UnityEngine.Object.Destroy(levelAsset);
            }
        }
        public static RoomSettings RegisterRoom(string name, string color) =>
            RegisterRoom(name, AssetsHelper.ColorFromHex(color));
        public static RoomSettings RegisterRoom(string name, Color color)
        {
            RoomSettings settings = new RoomSettings(RoomCategory.Special, name.ToEnum<RoomType>(), color, BasePlugin.Asset.Get<StandardDoorMats>("ClassDoorSet"));
            PlusLevelLoaderPlugin.Instance.roomSettings.Add(name, settings);
            return settings;
        }
        public static RoomSettings RegisterRoom(string name, Texture2D openDoor, Texture2D closedDoor, string color, RoomFunctionContainer functionContainer = null) =>
            RegisterRoom(name, openDoor, closedDoor, AssetsHelper.ColorFromHex(color), functionContainer);
        public static RoomSettings RegisterRoom(string name, Texture2D openDoor, Texture2D closedDoor, Color color, RoomFunctionContainer functionContainer = null)
        {
            StandardDoorMats door = ObjectCreators.CreateDoorDataObject(name + "Door", openDoor, closedDoor);
            RoomSettings settings = new RoomSettings(name.ToEnum<RoomCategory>(), RoomType.Room, color, door);
            if (functionContainer != null )
                settings.container = functionContainer;
            PlusLevelLoaderPlugin.Instance.roomSettings.Add(name, settings);
            return settings;
        }
        public static WeightedRoomAsset[] GetAllFromPath(string folderPath, int weight, int maxItemValue, bool isOffLimits, RoomFunctionContainer existingContainer = null, bool isAHallway = false, bool isASecretRoom = false, Texture2D mapBg = null, bool keepTextures = true, bool squaredShape = false)
        {
            List<WeightedRoomAsset> result = new List<WeightedRoomAsset>();
            foreach (RoomAsset room in GetAllFromPath(folderPath, maxItemValue, isOffLimits, existingContainer, isAHallway, isASecretRoom, mapBg, keepTextures, squaredShape))
            {
                result.Add(new WeightedRoomAsset() { selection = room, weight = weight });
            }
            return result.ToArray();
        }
        public static RoomAsset[] GetAllFromPath(string folderPath, int maxItemValue, bool isOffLimits, RoomFunctionContainer existingContainer = null, bool isAHallway = false, bool isASecretRoom = false, Texture2D mapBg = null, bool keepTextures = true, bool squaredShape = false)
        {
            List<RoomAsset> result = new List<RoomAsset>();
            foreach (string file in Directory.GetFiles(AssetsHelper.ModPath+folderPath))
            {
                List<RoomAsset> list = CreateAssetsFromPath(file, maxItemValue, isOffLimits, new BinaryReader(File.OpenRead(file)), existingContainer, isAHallway, isASecretRoom, mapBg, keepTextures, squaredShape);
                foreach (RoomAsset room in list)
                    result.Add(room);
            }
            return result.ToArray();
        }
        public static void AddRooms<N>(params WeightedRoomAsset[] rooms) where N : NPC
        {
            foreach (NPCMetadata data in NPCMetaStorage.Instance.FindAll(x => x.value is N))
            {
                foreach (NPC npc in data.prefabs.Values)
                {
                    npc.potentialRoomAssets = npc.potentialRoomAssets.AddRangeToArray(rooms);
                }
            }
        }
        public static RoomFunctionContainer CreateContainer(string name = null)
        {
            if (BasePlugin.Asset.Exists<RoomFunctionContainer>(name, out RoomFunctionContainer res))
                return res;
            res = new GameObject(name).AddComponent<RoomFunctionContainer>();
            res.name = name;
            res.gameObject.ConvertToPrefab(true);
            if (!name.EmptyOrNull())
                BasePlugin.Asset.Add<RoomFunctionContainer>(name, res);
            return res;
        }
        public static void CreateRooms()
        {
            RoomSettings settings = null;
            settings = RegisterRoom("BBEOldLibrary", Color.cyan);
            settings.container = CreateContainer("OldLibraryFunction");
            settings.container.AddFunction<SilenceRoomFunction>().mixer = AssetsHelper.LoadAsset<AudioMixer>("Master");
            settings = RegisterRoom("BBEOldLibraryRNG", Color.cyan);
            settings.container = CreateContainer("OldLibraryFunctionRNG");
            settings.container.AddFunction<SilenceRoomFunction>().mixer = AssetsHelper.LoadAsset<AudioMixer>("Master");
            settings.container.AddFunction<LibraryMazeGenerator>();

            RegisterRoom("BBEArtRoom", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_ArtRoomDoorOpen.png"), AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_ArtRoomDoorClose.png"), "#ff5500");
            RegisterRoom("BBEJohnMusclesGym", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_JohnMusclesGymDoorOpen.png"), AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_JohnMusclesGymDoorClose.png"), "#403b3c");
            RegisterRoom("BBEChessClass", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_StockfishRoomDoorOpen.png"), AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_StockfishRoomDoorClose.png"), "#1c5836");

            AddRooms<MrPaint>(GetAllFromPath("Premades/Rooms/MrPaintRooms", 100, 10, false));

            foreach (RoomAsset room in GetAllFromPath("Premades/Rooms/OldLibrary", 0, false, existingContainer: CreateContainer("OldLibraryFunctionRNG")))
            {
                room.posterChance = -1;
                room.AddFunction<SpecialRoomSwingingDoorsBuilder>().swingDoorPre = Prefabs.SwingDoor;
                AddSpecialRoomToFloor(room, 0, 33, 33, 33);
            }
        }
    }
}
