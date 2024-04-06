// using System;
// using Mirror;
// using SquareHero.Hotfix.Map;
// using SquareHero.Hotfix.Model;
// using UnityCommon.Util;
// using UnityEngine;
//
// namespace SquareHero.Hotfix.Mirror
// {
//     public static class MessageHelper
//     {
//
//         public static void Initialize()
//         {
//             RegisteMessageWriter();
//             RegisteMessageReader();
//         }
//
//
//         public static void RegisteMessageWriter()
//         {
//             Writer<SpawnMessage>.write = (writer, message) =>
//             {
//                 writer.WriteUInt(message.netId);
//                 writer.WriteBool(message.isLocalPlayer);
//                 writer.WriteBool(message.isOwner);
//                 writer.WriteULong(message.sceneId);
//                 writer.WriteUInt(message.assetId);
//                 writer.WriteVector3(message.position);
//                 writer.WriteQuaternion(message.rotation);
//                 writer.WriteVector3(message.scale);
//                 writer.WriteArray(message.payload.ToArray());
//             };
//
//             Writer<SceneMessage>.write = (writer, msg) =>
//             {
//                 writer.WriteString(msg.sceneName);
//                 writer.WriteInt((int)msg.sceneOperation);
//                 writer.WriteBool(msg.customHandling);
//             };
//
//             Writer<PlayerGameData>.write = (writer, data) =>
//             {
//                 writer.WriteLong(data.UserId);
//                 writer.WriteString(data.UserName);
//                 writer.WriteInt(data.SkinId);
//                 writer.WriteInt(data.Track);
//                 writer.WriteInt(data.ConnectionId);
//                 writer.WriteArray(data.PropIds);
//                 writer.WriteBool(data.IsRobot);
//                 writer.WriteString(data.SkinName);
//             };
//             Writer<TileData>.write = (writer, data) =>
//             {
//                 writer.WriteInt(data.id);
//                 writer.WriteInt((int)data.tileType);
//                 writer.WriteInt((int)data.tilePosition);
//             };
//
//             Writer<GiftModel>.write = (writer, model) =>
//             {
//                 writer.WriteInt(model.Track);
//                 writer.WriteBool(model.NeedCreate);
//                 writer.WriteInt((int)model.RewardType);
//                 writer.WriteInt(model.CoinCount);
//                 writer.WriteInt(model.DiamondCount);
//             };
//
//             Writer<RoomInfo>.write = (writer, info) =>
//             {
//                 var json = JsonUtil.ToJson(info);
//                 writer.WriteString(json);
//             };
//         }
//
//         public static void RegisteMessageReader()
//         {
//             Reader<SpawnMessage>.read = reader =>
//             {
//                 SpawnMessage msg = new SpawnMessage();
//                 msg.netId = reader.ReadUInt();
//                 msg.isLocalPlayer = reader.ReadBool();
//                 msg.isOwner = reader.ReadBool();
//                 msg.sceneId = reader.ReadULong();
//                 msg.assetId = reader.ReadUInt();
//                 msg.position = reader.ReadVector3();
//                 msg.rotation = reader.ReadQuaternion();
//                 msg.scale = reader.ReadVector3();
//                 var readArray = reader.ReadArray<byte>();
//                 msg.payload = new ArraySegment<byte>(readArray);
//                 Debug.Log($"Read SpawnMessage : {msg.assetId}, {msg.sceneId}, {msg.netId}");
//                 return msg;
//             };
//
//             Reader<SceneMessage>.read = reader =>
//             {
//                 SceneMessage msg = new SceneMessage();
//
//                 msg.sceneName = reader.ReadString();
//                 msg.sceneOperation = (SceneOperation)reader.ReadInt();
//                 msg.customHandling = reader.ReadBool();
//                 return msg;
//             };
//
//             Reader<PlayerGameData>.read = reader =>
//             {
//                 PlayerGameData data = new PlayerGameData();
//
//                 data.UserId = reader.ReadLong();
//                 data.UserName = reader.ReadString();
//                 data.SkinId = reader.ReadInt();
//                 data.Track = reader.ReadInt();
//                 data.ConnectionId = reader.ReadInt();
//                 data.PropIds = reader.ReadArray<int>();
//                 data.IsRobot = reader.ReadBool();
//                 data.SkinName = reader.ReadString();
//                 return data;
//             };
//
//             Reader<TileData>.read = reader =>
//             {
//                 TileData data = new TileData();
//                 data.id = reader.ReadInt();
//                 data.tileType = (TileType)reader.ReadInt();
//                 data.tilePosition = (TilePosition)reader.ReadInt();
//                 return data;
//             };
//
//             Reader<GiftModel>.read = reader =>
//             {
//                 GiftModel model = new GiftModel();
//
//                 model.Track = reader.ReadInt();
//                 model.NeedCreate = reader.ReadBool();
//                 model.RewardType = (RewardType)reader.ReadInt();
//                 model.CoinCount = reader.ReadInt();
//                 model.DiamondCount = reader.ReadInt();
//
//                 return model;
//             };
//
//             Reader<RoomInfo>.read = reader =>
//             {
//                 var roomInfo = JsonUtil.FromJson<RoomInfo>(reader.ReadString());
//                 return roomInfo;
//             };
//         }
//
//
//
//     }
// }