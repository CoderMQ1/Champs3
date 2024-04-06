using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Cinemachine.dll",
		"DOTween.dll",
		"LitJson.dll",
		"Mirror.dll",
		"ProtoBuf-Net.dll",
		"QFramework.CoreKit.dll",
		"QFramework.dll",
		"System.Core.dll",
		"System.dll",
		"UIKit.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.UI.dll",
		"YooAsset.dll",
		"com.unity-common.core.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// DG.Tweening.Core.DOGetter<UnityEngine.Color>
	// DG.Tweening.Core.DOGetter<UnityEngine.Vector3>
	// DG.Tweening.Core.DOGetter<float>
	// DG.Tweening.Core.DOSetter<UnityEngine.Color>
	// DG.Tweening.Core.DOSetter<UnityEngine.Vector3>
	// DG.Tweening.Core.DOSetter<float>
	// Mirror.Pool<object>
	// Mirror.Reader<Mirror.AddPlayerMessage>
	// Mirror.Reader<Mirror.ChangeOwnerMessage>
	// Mirror.Reader<Mirror.CommandMessage>
	// Mirror.Reader<Mirror.EntityStateMessage>
	// Mirror.Reader<Mirror.NetworkBehaviourSyncVar>
	// Mirror.Reader<Mirror.NetworkPingMessage>
	// Mirror.Reader<Mirror.NetworkPongMessage>
	// Mirror.Reader<Mirror.NotReadyMessage>
	// Mirror.Reader<Mirror.ObjectDestroyMessage>
	// Mirror.Reader<Mirror.ObjectHideMessage>
	// Mirror.Reader<Mirror.ObjectSpawnFinishedMessage>
	// Mirror.Reader<Mirror.ObjectSpawnStartedMessage>
	// Mirror.Reader<Mirror.ReadyMessage>
	// Mirror.Reader<Mirror.RpcMessage>
	// Mirror.Reader<Mirror.SceneMessage>
	// Mirror.Reader<Mirror.SpawnMessage>
	// Mirror.Reader<Mirror.TimeSnapshotMessage>
	// Mirror.Reader<SquareHero.Hotfix.Model.GiftModel>
	// Mirror.Reader<SquareHero.Hotfix.RoleAttribute>
	// Mirror.Reader<System.ArraySegment<byte>>
	// Mirror.Reader<System.DateTime>
	// Mirror.Reader<System.Decimal>
	// Mirror.Reader<System.Guid>
	// Mirror.Reader<System.Nullable<System.DateTime>>
	// Mirror.Reader<System.Nullable<System.Decimal>>
	// Mirror.Reader<System.Nullable<System.Guid>>
	// Mirror.Reader<System.Nullable<UnityEngine.Color32>>
	// Mirror.Reader<System.Nullable<UnityEngine.Color>>
	// Mirror.Reader<System.Nullable<UnityEngine.Matrix4x4>>
	// Mirror.Reader<System.Nullable<UnityEngine.Plane>>
	// Mirror.Reader<System.Nullable<UnityEngine.Quaternion>>
	// Mirror.Reader<System.Nullable<UnityEngine.Ray>>
	// Mirror.Reader<System.Nullable<UnityEngine.Rect>>
	// Mirror.Reader<System.Nullable<UnityEngine.Vector2>>
	// Mirror.Reader<System.Nullable<UnityEngine.Vector2Int>>
	// Mirror.Reader<System.Nullable<UnityEngine.Vector3>>
	// Mirror.Reader<System.Nullable<UnityEngine.Vector3Int>>
	// Mirror.Reader<System.Nullable<UnityEngine.Vector4>>
	// Mirror.Reader<System.Nullable<byte>>
	// Mirror.Reader<System.Nullable<double>>
	// Mirror.Reader<System.Nullable<float>>
	// Mirror.Reader<System.Nullable<int>>
	// Mirror.Reader<System.Nullable<long>>
	// Mirror.Reader<System.Nullable<sbyte>>
	// Mirror.Reader<System.Nullable<short>>
	// Mirror.Reader<System.Nullable<uint>>
	// Mirror.Reader<System.Nullable<ulong>>
	// Mirror.Reader<System.Nullable<ushort>>
	// Mirror.Reader<UnityEngine.Color32>
	// Mirror.Reader<UnityEngine.Color>
	// Mirror.Reader<UnityEngine.Matrix4x4>
	// Mirror.Reader<UnityEngine.Plane>
	// Mirror.Reader<UnityEngine.Quaternion>
	// Mirror.Reader<UnityEngine.Ray>
	// Mirror.Reader<UnityEngine.Rect>
	// Mirror.Reader<UnityEngine.Vector2>
	// Mirror.Reader<UnityEngine.Vector2Int>
	// Mirror.Reader<UnityEngine.Vector3>
	// Mirror.Reader<UnityEngine.Vector3Int>
	// Mirror.Reader<UnityEngine.Vector4>
	// Mirror.Reader<byte>
	// Mirror.Reader<double>
	// Mirror.Reader<float>
	// Mirror.Reader<int>
	// Mirror.Reader<long>
	// Mirror.Reader<object>
	// Mirror.Reader<sbyte>
	// Mirror.Reader<short>
	// Mirror.Reader<uint>
	// Mirror.Reader<ulong>
	// Mirror.Reader<ushort>
	// Mirror.Writer<Mirror.AddPlayerMessage>
	// Mirror.Writer<Mirror.ChangeOwnerMessage>
	// Mirror.Writer<Mirror.CommandMessage>
	// Mirror.Writer<Mirror.EntityStateMessage>
	// Mirror.Writer<Mirror.NetworkPingMessage>
	// Mirror.Writer<Mirror.NetworkPongMessage>
	// Mirror.Writer<Mirror.NotReadyMessage>
	// Mirror.Writer<Mirror.ObjectDestroyMessage>
	// Mirror.Writer<Mirror.ObjectHideMessage>
	// Mirror.Writer<Mirror.ObjectSpawnFinishedMessage>
	// Mirror.Writer<Mirror.ObjectSpawnStartedMessage>
	// Mirror.Writer<Mirror.ReadyMessage>
	// Mirror.Writer<Mirror.RpcMessage>
	// Mirror.Writer<Mirror.SceneMessage>
	// Mirror.Writer<Mirror.SpawnMessage>
	// Mirror.Writer<Mirror.TimeSnapshotMessage>
	// Mirror.Writer<SquareHero.Hotfix.Model.GiftModel>
	// Mirror.Writer<SquareHero.Hotfix.RoleAttribute>
	// Mirror.Writer<System.ArraySegment<byte>>
	// Mirror.Writer<System.DateTime>
	// Mirror.Writer<System.Decimal>
	// Mirror.Writer<System.Guid>
	// Mirror.Writer<System.Nullable<System.DateTime>>
	// Mirror.Writer<System.Nullable<System.Decimal>>
	// Mirror.Writer<System.Nullable<System.Guid>>
	// Mirror.Writer<System.Nullable<UnityEngine.Color32>>
	// Mirror.Writer<System.Nullable<UnityEngine.Color>>
	// Mirror.Writer<System.Nullable<UnityEngine.Matrix4x4>>
	// Mirror.Writer<System.Nullable<UnityEngine.Plane>>
	// Mirror.Writer<System.Nullable<UnityEngine.Quaternion>>
	// Mirror.Writer<System.Nullable<UnityEngine.Ray>>
	// Mirror.Writer<System.Nullable<UnityEngine.Rect>>
	// Mirror.Writer<System.Nullable<UnityEngine.Vector2>>
	// Mirror.Writer<System.Nullable<UnityEngine.Vector2Int>>
	// Mirror.Writer<System.Nullable<UnityEngine.Vector3>>
	// Mirror.Writer<System.Nullable<UnityEngine.Vector3Int>>
	// Mirror.Writer<System.Nullable<UnityEngine.Vector4>>
	// Mirror.Writer<System.Nullable<byte>>
	// Mirror.Writer<System.Nullable<double>>
	// Mirror.Writer<System.Nullable<float>>
	// Mirror.Writer<System.Nullable<int>>
	// Mirror.Writer<System.Nullable<long>>
	// Mirror.Writer<System.Nullable<sbyte>>
	// Mirror.Writer<System.Nullable<short>>
	// Mirror.Writer<System.Nullable<uint>>
	// Mirror.Writer<System.Nullable<ulong>>
	// Mirror.Writer<System.Nullable<ushort>>
	// Mirror.Writer<UnityEngine.Color32>
	// Mirror.Writer<UnityEngine.Color>
	// Mirror.Writer<UnityEngine.Matrix4x4>
	// Mirror.Writer<UnityEngine.Plane>
	// Mirror.Writer<UnityEngine.Quaternion>
	// Mirror.Writer<UnityEngine.Ray>
	// Mirror.Writer<UnityEngine.Rect>
	// Mirror.Writer<UnityEngine.Vector2>
	// Mirror.Writer<UnityEngine.Vector2Int>
	// Mirror.Writer<UnityEngine.Vector3>
	// Mirror.Writer<UnityEngine.Vector3Int>
	// Mirror.Writer<UnityEngine.Vector4>
	// Mirror.Writer<byte>
	// Mirror.Writer<double>
	// Mirror.Writer<float>
	// Mirror.Writer<int>
	// Mirror.Writer<long>
	// Mirror.Writer<object>
	// Mirror.Writer<sbyte>
	// Mirror.Writer<short>
	// Mirror.Writer<uint>
	// Mirror.Writer<ulong>
	// Mirror.Writer<ushort>
	// QFramework.Architecture.<>c<object>
	// QFramework.Architecture<object>
	// QFramework.BindableProperty.<>c<int>
	// QFramework.BindableProperty<int>
	// QFramework.BindablePropertyUnRegister<int>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.CreateRobot>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.GameReadyedOnClient>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.OnClientGameOver>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.OnClientStartGame>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.OnServerGameOver>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.GameEvents.OnServerStartGame>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.MapEvents.OnServerFinishCreateMap>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.PlayerEvents.GetCoin>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.PlayerEvents.JoinGame>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerInitialized>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.PlayerEvents.OnPropAffectEnd>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.PlayerEvents.OnPropUse>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets>
	// QFramework.EasyEvent.<>c<SquareHero.Hotfix.SystemEvents.UpdatePlayerProps>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.CreateRobot>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.GameReadyedOnClient>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.OnClientGameOver>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.OnClientStartGame>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.OnServerGameOver>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.GameEvents.OnServerStartGame>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.MapEvents.OnServerFinishCreateMap>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.PlayerEvents.GetCoin>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.PlayerEvents.JoinGame>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerInitialized>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.PlayerEvents.OnPropAffectEnd>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.PlayerEvents.OnPropUse>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<SquareHero.Hotfix.SystemEvents.UpdatePlayerProps>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.CreateRobot>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.GameReadyedOnClient>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.OnClientGameOver>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.OnClientStartGame>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.OnServerGameOver>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.GameEvents.OnServerStartGame>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.MapEvents.OnServerFinishCreateMap>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.PlayerEvents.GetCoin>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.PlayerEvents.JoinGame>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerInitialized>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.PlayerEvents.OnPropAffectEnd>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.PlayerEvents.OnPropUse>
	// QFramework.EasyEvent<SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived>
	// QFramework.EasyEvent<SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets>
	// QFramework.EasyEvent<SquareHero.Hotfix.SystemEvents.UpdatePlayerProps>
	// QFramework.IObjectFactory<object>
	// QFramework.MonoSingleton<object>
	// QFramework.MonoSingletonProperty<object>
	// QFramework.Pool<object>
	// QFramework.SafeObjectPool<object>
	// QFramework.Singleton<object>
	// QFramework.SingletonProperty<object>
	// QFramework.UIKit.<>c__DisplayClass6_0<object>
	// QFramework.UIKitTable<object>
	// System.Action<Mirror.NetworkDiagnostics.MessageInfo>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.CreateRobot>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.GameReadyedOnClient>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.OnClientGameOver>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.OnClientStartGame>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.OnServerGameOver>
	// System.Action<SquareHero.Hotfix.Events.GameEvents.OnServerStartGame>
	// System.Action<SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap>
	// System.Action<SquareHero.Hotfix.Events.MapEvents.OnServerFinishCreateMap>
	// System.Action<SquareHero.Hotfix.Events.PlayerEvents.GetCoin>
	// System.Action<SquareHero.Hotfix.Events.PlayerEvents.JoinGame>
	// System.Action<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerInitialized>
	// System.Action<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode>
	// System.Action<SquareHero.Hotfix.Events.PlayerEvents.OnPropAffectEnd>
	// System.Action<SquareHero.Hotfix.Events.PlayerEvents.OnPropUse>
	// System.Action<SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived>
	// System.Action<SquareHero.Hotfix.Model.GiftModel>
	// System.Action<SquareHero.Hotfix.PlayerAsset>
	// System.Action<SquareHero.Hotfix.PropsData>
	// System.Action<SquareHero.Hotfix.RoleAttribute>
	// System.Action<SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets>
	// System.Action<SquareHero.Hotfix.SystemEvents.UpdatePlayerProps>
	// System.Action<System.DateTime>
	// System.Action<UnityEngine.EventSystems.RaycastResult>
	// System.Action<double>
	// System.Action<int,System.IntPtr,int>
	// System.Action<int,int>
	// System.Action<int,object>
	// System.Action<int>
	// System.Action<long>
	// System.Action<object,Mirror.AddPlayerMessage>
	// System.Action<object,Mirror.ChangeOwnerMessage>
	// System.Action<object,Mirror.CommandMessage>
	// System.Action<object,Mirror.EntityStateMessage>
	// System.Action<object,Mirror.NetworkPingMessage>
	// System.Action<object,Mirror.NetworkPongMessage>
	// System.Action<object,Mirror.NotReadyMessage>
	// System.Action<object,Mirror.ObjectDestroyMessage>
	// System.Action<object,Mirror.ObjectHideMessage>
	// System.Action<object,Mirror.ObjectSpawnFinishedMessage>
	// System.Action<object,Mirror.ObjectSpawnStartedMessage>
	// System.Action<object,Mirror.ReadyMessage>
	// System.Action<object,Mirror.RpcMessage>
	// System.Action<object,Mirror.SceneMessage>
	// System.Action<object,Mirror.SpawnMessage>
	// System.Action<object,Mirror.TimeSnapshotMessage>
	// System.Action<object,SquareHero.Hotfix.Model.GiftModel>
	// System.Action<object,SquareHero.Hotfix.RoleAttribute>
	// System.Action<object,System.ArraySegment<byte>>
	// System.Action<object,System.DateTime>
	// System.Action<object,System.Decimal>
	// System.Action<object,System.Guid>
	// System.Action<object,System.Nullable<System.DateTime>>
	// System.Action<object,System.Nullable<System.Decimal>>
	// System.Action<object,System.Nullable<System.Guid>>
	// System.Action<object,System.Nullable<UnityEngine.Color32>>
	// System.Action<object,System.Nullable<UnityEngine.Color>>
	// System.Action<object,System.Nullable<UnityEngine.Matrix4x4>>
	// System.Action<object,System.Nullable<UnityEngine.Plane>>
	// System.Action<object,System.Nullable<UnityEngine.Quaternion>>
	// System.Action<object,System.Nullable<UnityEngine.Ray>>
	// System.Action<object,System.Nullable<UnityEngine.Rect>>
	// System.Action<object,System.Nullable<UnityEngine.Vector2>>
	// System.Action<object,System.Nullable<UnityEngine.Vector2Int>>
	// System.Action<object,System.Nullable<UnityEngine.Vector3>>
	// System.Action<object,System.Nullable<UnityEngine.Vector3Int>>
	// System.Action<object,System.Nullable<UnityEngine.Vector4>>
	// System.Action<object,System.Nullable<byte>>
	// System.Action<object,System.Nullable<double>>
	// System.Action<object,System.Nullable<float>>
	// System.Action<object,System.Nullable<int>>
	// System.Action<object,System.Nullable<long>>
	// System.Action<object,System.Nullable<sbyte>>
	// System.Action<object,System.Nullable<short>>
	// System.Action<object,System.Nullable<uint>>
	// System.Action<object,System.Nullable<ulong>>
	// System.Action<object,System.Nullable<ushort>>
	// System.Action<object,UnityEngine.Color32>
	// System.Action<object,UnityEngine.Color>
	// System.Action<object,UnityEngine.Matrix4x4>
	// System.Action<object,UnityEngine.Plane>
	// System.Action<object,UnityEngine.Quaternion>
	// System.Action<object,UnityEngine.Ray>
	// System.Action<object,UnityEngine.Rect>
	// System.Action<object,UnityEngine.Vector2>
	// System.Action<object,UnityEngine.Vector2Int>
	// System.Action<object,UnityEngine.Vector3>
	// System.Action<object,UnityEngine.Vector3Int>
	// System.Action<object,UnityEngine.Vector4>
	// System.Action<object,byte>
	// System.Action<object,double>
	// System.Action<object,float>
	// System.Action<object,int>
	// System.Action<object,long>
	// System.Action<object,object>
	// System.Action<object,sbyte>
	// System.Action<object,short>
	// System.Action<object,uint>
	// System.Action<object,ulong>
	// System.Action<object,ushort>
	// System.Action<object>
	// System.Action<ushort,System.ArraySegment<byte>>
	// System.ArraySegment.Enumerator<byte>
	// System.ArraySegment<byte>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<SquareHero.Hotfix.Net.Message>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<SquareHero.Hotfix.Net.Message>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<SquareHero.Hotfix.Net.Message>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Generic.ArraySortHelper<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.ArraySortHelper<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.ArraySortHelper<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.ArraySortHelper<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<long>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.Comparer<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.Comparer<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.Comparer<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.Comparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<double>
	// System.Collections.Generic.Comparer<float>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.ComparisonComparer<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.ComparisonComparer<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.ComparisonComparer<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.ComparisonComparer<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ComparisonComparer<byte>
	// System.Collections.Generic.ComparisonComparer<double>
	// System.Collections.Generic.ComparisonComparer<float>
	// System.Collections.Generic.ComparisonComparer<int>
	// System.Collections.Generic.ComparisonComparer<long>
	// System.Collections.Generic.ComparisonComparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,float>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,float>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<ushort,object>
	// System.Collections.Generic.Dictionary<int,float>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<ushort,object>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<ushort>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.ICollection<SquareHero.Hotfix.Net.Message>
	// System.Collections.Generic.ICollection<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.ICollection<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.ICollection<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.ICollection<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<long>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.IComparer<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.IComparer<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.IComparer<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.IComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<long>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.IEnumerable<SquareHero.Hotfix.Net.Message>
	// System.Collections.Generic.IEnumerable<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.IEnumerable<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.IEnumerable<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerable<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.IEnumerator<SquareHero.Hotfix.Net.Message>
	// System.Collections.Generic.IEnumerator<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.IEnumerator<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.IEnumerator<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerator<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<long>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<ushort>
	// System.Collections.Generic.IList<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.IList<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.IList<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.IList<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.IList<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IList<UnityEngine.UIVertex>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<long>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<int,float>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<ushort,object>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.List.Enumerator<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.List.Enumerator<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.List.Enumerator<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.List.Enumerator<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.List<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.List<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.List<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.Generic.ObjectComparer<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.Generic.ObjectComparer<SquareHero.Hotfix.PropsData>
	// System.Collections.Generic.ObjectComparer<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.Generic.ObjectComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<double>
	// System.Collections.Generic.ObjectComparer<float>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<ushort>
	// System.Collections.Generic.Queue.Enumerator<System.UIntPtr>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<System.UIntPtr>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<SquareHero.Hotfix.Model.GiftModel>
	// System.Collections.ObjectModel.ReadOnlyCollection<SquareHero.Hotfix.PlayerAsset>
	// System.Collections.ObjectModel.ReadOnlyCollection<SquareHero.Hotfix.PropsData>
	// System.Collections.ObjectModel.ReadOnlyCollection<SquareHero.Hotfix.RoleAttribute>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<long>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<SquareHero.Hotfix.Model.GiftModel>
	// System.Comparison<SquareHero.Hotfix.PlayerAsset>
	// System.Comparison<SquareHero.Hotfix.PropsData>
	// System.Comparison<SquareHero.Hotfix.RoleAttribute>
	// System.Comparison<UnityEngine.EventSystems.RaycastResult>
	// System.Comparison<byte>
	// System.Comparison<double>
	// System.Comparison<float>
	// System.Comparison<int>
	// System.Comparison<long>
	// System.Comparison<object>
	// System.Converter<object,object>
	// System.Func<double,double>
	// System.Func<double>
	// System.Func<float,byte>
	// System.Func<float>
	// System.Func<int,object>
	// System.Func<int>
	// System.Func<object,Mirror.AddPlayerMessage>
	// System.Func<object,Mirror.ChangeOwnerMessage>
	// System.Func<object,Mirror.CommandMessage>
	// System.Func<object,Mirror.EntityStateMessage>
	// System.Func<object,Mirror.NetworkBehaviourSyncVar>
	// System.Func<object,Mirror.NetworkPingMessage>
	// System.Func<object,Mirror.NetworkPongMessage>
	// System.Func<object,Mirror.NotReadyMessage>
	// System.Func<object,Mirror.ObjectDestroyMessage>
	// System.Func<object,Mirror.ObjectHideMessage>
	// System.Func<object,Mirror.ObjectSpawnFinishedMessage>
	// System.Func<object,Mirror.ObjectSpawnStartedMessage>
	// System.Func<object,Mirror.ReadyMessage>
	// System.Func<object,Mirror.RpcMessage>
	// System.Func<object,Mirror.SceneMessage>
	// System.Func<object,Mirror.SpawnMessage>
	// System.Func<object,Mirror.TimeSnapshotMessage>
	// System.Func<object,SquareHero.Hotfix.Model.GiftModel>
	// System.Func<object,SquareHero.Hotfix.RoleAttribute>
	// System.Func<object,System.ArraySegment<byte>>
	// System.Func<object,System.DateTime>
	// System.Func<object,System.Decimal>
	// System.Func<object,System.Guid>
	// System.Func<object,System.Nullable<System.DateTime>>
	// System.Func<object,System.Nullable<System.Decimal>>
	// System.Func<object,System.Nullable<System.Guid>>
	// System.Func<object,System.Nullable<UnityEngine.Color32>>
	// System.Func<object,System.Nullable<UnityEngine.Color>>
	// System.Func<object,System.Nullable<UnityEngine.Matrix4x4>>
	// System.Func<object,System.Nullable<UnityEngine.Plane>>
	// System.Func<object,System.Nullable<UnityEngine.Quaternion>>
	// System.Func<object,System.Nullable<UnityEngine.Ray>>
	// System.Func<object,System.Nullable<UnityEngine.Rect>>
	// System.Func<object,System.Nullable<UnityEngine.Vector2>>
	// System.Func<object,System.Nullable<UnityEngine.Vector2Int>>
	// System.Func<object,System.Nullable<UnityEngine.Vector3>>
	// System.Func<object,System.Nullable<UnityEngine.Vector3Int>>
	// System.Func<object,System.Nullable<UnityEngine.Vector4>>
	// System.Func<object,System.Nullable<byte>>
	// System.Func<object,System.Nullable<double>>
	// System.Func<object,System.Nullable<float>>
	// System.Func<object,System.Nullable<int>>
	// System.Func<object,System.Nullable<long>>
	// System.Func<object,System.Nullable<sbyte>>
	// System.Func<object,System.Nullable<short>>
	// System.Func<object,System.Nullable<uint>>
	// System.Func<object,System.Nullable<ulong>>
	// System.Func<object,System.Nullable<ushort>>
	// System.Func<object,UnityEngine.Color32>
	// System.Func<object,UnityEngine.Color>
	// System.Func<object,UnityEngine.Matrix4x4>
	// System.Func<object,UnityEngine.Plane>
	// System.Func<object,UnityEngine.Quaternion>
	// System.Func<object,UnityEngine.Ray>
	// System.Func<object,UnityEngine.Rect>
	// System.Func<object,UnityEngine.Vector2>
	// System.Func<object,UnityEngine.Vector2Int>
	// System.Func<object,UnityEngine.Vector3>
	// System.Func<object,UnityEngine.Vector3Int>
	// System.Func<object,UnityEngine.Vector4>
	// System.Func<object,byte>
	// System.Func<object,double>
	// System.Func<object,float>
	// System.Func<object,int>
	// System.Func<object,long>
	// System.Func<object,object,float>
	// System.Func<object,object>
	// System.Func<object,sbyte>
	// System.Func<object,short>
	// System.Func<object,uint>
	// System.Func<object,ulong>
	// System.Func<object,ushort>
	// System.Func<object>
	// System.Linq.Buffer<byte>
	// System.Linq.Enumerable.<ReverseIterator>d__79<byte>
	// System.Nullable<System.DateTime>
	// System.Nullable<UnityEngine.Vector2>
	// System.Nullable<UnityEngine.Vector3>
	// System.Nullable<byte>
	// System.Nullable<double>
	// System.Nullable<float>
	// System.Nullable<int>
	// System.Predicate<SquareHero.Hotfix.Model.GiftModel>
	// System.Predicate<SquareHero.Hotfix.PlayerAsset>
	// System.Predicate<SquareHero.Hotfix.PropsData>
	// System.Predicate<SquareHero.Hotfix.RoleAttribute>
	// System.Predicate<UnityEngine.EventSystems.RaycastResult>
	// System.Predicate<int>
	// System.Predicate<long>
	// System.Predicate<object>
	// System.Runtime.CompilerServices.ConditionalWeakTable.CreateValueCallback<object,object>
	// System.Runtime.CompilerServices.ConditionalWeakTable.Enumerator<object,object>
	// System.Runtime.CompilerServices.ConditionalWeakTable<object,object>
	// UnityEngine.Events.InvokableCall<UnityEngine.Vector2>
	// UnityEngine.Events.InvokableCall<byte>
	// UnityEngine.Events.InvokableCall<float>
	// UnityEngine.Events.InvokableCall<int>
	// UnityEngine.Events.InvokableCall<object>
	// UnityEngine.Events.UnityAction<UnityEngine.Vector2>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityAction<float>
	// UnityEngine.Events.UnityAction<int>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<UnityEngine.Vector2>
	// UnityEngine.Events.UnityEvent<byte>
	// UnityEngine.Events.UnityEvent<float>
	// UnityEngine.Events.UnityEvent<int>
	// UnityEngine.Events.UnityEvent<object>
	// }}

	public void RefMethods()
	{
		// object Cinemachine.CinemachineVirtualCamera.GetCinemachineComponent<object>()
		// object DG.Tweening.TweenExtensions.Play<object>(object)
		// object DG.Tweening.TweenSettingsExtensions.OnKill<object>(object,DG.Tweening.TweenCallback)
		// object DG.Tweening.TweenSettingsExtensions.SetEase<object>(object,DG.Tweening.Ease)
		// object DG.Tweening.TweenSettingsExtensions.SetLoops<object>(object,int,DG.Tweening.LoopType)
		// object DG.Tweening.TweenSettingsExtensions.SetTarget<object>(object,object)
		// SquareHero.Hotfix.AllPropResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.AllPropResponse>(string)
		// SquareHero.Hotfix.BaseResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.BaseResponse>(string)
		// SquareHero.Hotfix.BuyMessageResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.BuyMessageResponse>(string)
		// SquareHero.Hotfix.GetRoleResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.GetRoleResponse>(string)
		// SquareHero.Hotfix.LoginResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.LoginResponse>(string)
		// SquareHero.Hotfix.PlayerassetsResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.PlayerassetsResponse>(string)
		// SquareHero.Hotfix.SelectPropsResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.SelectPropsResponse>(string)
		// SquareHero.Hotfix.SpinExchangeResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.SpinExchangeResponse>(string)
		// SquareHero.Hotfix.SpinInfoResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.SpinInfoResponse>(string)
		// SquareHero.Hotfix.SpinResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.SpinResponse>(string)
		// SquareHero.Hotfix.WalletDataResponse LitJson.JsonMapper.ToObject<SquareHero.Hotfix.WalletDataResponse>(string)
		// object LitJson.JsonMapper.ToObject<object>(string)
		// System.Void Mirror.NetworkBehaviour.GeneratedSyncVarDeserialize<int>(int&,System.Action<int,int>,int)
		// System.Void Mirror.NetworkBehaviour.GeneratedSyncVarDeserialize<object>(object&,System.Action<object,object>,object)
		// System.Void Mirror.NetworkBehaviour.GeneratedSyncVarSetter<int>(int,int&,ulong,System.Action<int,int>)
		// System.Void Mirror.NetworkBehaviour.GeneratedSyncVarSetter<object>(object,object&,ulong,System.Action<object,object>)
		// System.Void Mirror.NetworkBehaviour.SetSyncVar<int>(int,int&,ulong)
		// System.Void Mirror.NetworkBehaviour.SetSyncVar<object>(object,object&,ulong)
		// bool Mirror.NetworkBehaviour.SyncVarEqual<int>(int,int&)
		// bool Mirror.NetworkBehaviour.SyncVarEqual<object>(object,object&)
		// System.Void Mirror.NetworkConnection.Send<Mirror.SceneMessage>(Mirror.SceneMessage,int)
		// System.Void Mirror.NetworkDiagnostics.OnSend<Mirror.SceneMessage>(Mirror.SceneMessage,int,int,int)
		// System.Void Mirror.NetworkMessages.Pack<Mirror.SceneMessage>(Mirror.SceneMessage,Mirror.NetworkWriter)
		// SquareHero.Hotfix.Model.GiftModel Mirror.NetworkReader.Read<SquareHero.Hotfix.Model.GiftModel>()
		// SquareHero.Hotfix.RoleAttribute Mirror.NetworkReader.Read<SquareHero.Hotfix.RoleAttribute>()
		// byte Mirror.NetworkReader.Read<byte>()
		// int Mirror.NetworkReader.Read<int>()
		// long Mirror.NetworkReader.Read<long>()
		// object Mirror.NetworkReader.Read<object>()
		// byte[] Mirror.NetworkReaderExtensions.ReadArray<byte>(Mirror.NetworkReader)
		// int[] Mirror.NetworkReaderExtensions.ReadArray<int>(Mirror.NetworkReader)
		// System.Collections.Generic.List<SquareHero.Hotfix.Model.GiftModel> Mirror.NetworkReaderExtensions.ReadList<SquareHero.Hotfix.Model.GiftModel>(Mirror.NetworkReader)
		// System.Collections.Generic.List<SquareHero.Hotfix.RoleAttribute> Mirror.NetworkReaderExtensions.ReadList<SquareHero.Hotfix.RoleAttribute>(Mirror.NetworkReader)
		// System.Collections.Generic.List<long> Mirror.NetworkReaderExtensions.ReadList<long>(Mirror.NetworkReader)
		// System.Collections.Generic.List<object> Mirror.NetworkReaderExtensions.ReadList<object>(Mirror.NetworkReader)
		// System.Void Mirror.NetworkServer.SendToAll<Mirror.SceneMessage>(Mirror.SceneMessage,int,bool)
		// System.Void Mirror.NetworkWriter.Write<Mirror.SceneMessage>(Mirror.SceneMessage)
		// System.Void Mirror.NetworkWriter.Write<SquareHero.Hotfix.Model.GiftModel>(SquareHero.Hotfix.Model.GiftModel)
		// System.Void Mirror.NetworkWriter.Write<SquareHero.Hotfix.RoleAttribute>(SquareHero.Hotfix.RoleAttribute)
		// System.Void Mirror.NetworkWriter.Write<byte>(byte)
		// System.Void Mirror.NetworkWriter.Write<int>(int)
		// System.Void Mirror.NetworkWriter.Write<long>(long)
		// System.Void Mirror.NetworkWriter.Write<object>(object)
		// System.Void Mirror.NetworkWriterExtensions.WriteArray<byte>(Mirror.NetworkWriter,byte[])
		// System.Void Mirror.NetworkWriterExtensions.WriteArray<int>(Mirror.NetworkWriter,int[])
		// System.Void Mirror.NetworkWriterExtensions.WriteList<SquareHero.Hotfix.Model.GiftModel>(Mirror.NetworkWriter,System.Collections.Generic.List<SquareHero.Hotfix.Model.GiftModel>)
		// System.Void Mirror.NetworkWriterExtensions.WriteList<SquareHero.Hotfix.RoleAttribute>(Mirror.NetworkWriter,System.Collections.Generic.List<SquareHero.Hotfix.RoleAttribute>)
		// System.Void Mirror.NetworkWriterExtensions.WriteList<long>(Mirror.NetworkWriter,System.Collections.Generic.List<long>)
		// System.Void Mirror.NetworkWriterExtensions.WriteList<object>(Mirror.NetworkWriter,System.Collections.Generic.List<object>)
		// System.Void ProtoBuf.Serializer.Serialize<object>(System.IO.Stream,object)
		// System.Void QFramework.Architecture<object>.RegisterModel<object>(object)
		// System.Void QFramework.Architecture<object>.RegisterSystem<object>(object)
		// object QFramework.CanGetModelExtension.GetModel<object>(QFramework.ICanGetModel)
		// object QFramework.EasyEvents.GetEvent<object>()
		// object QFramework.EasyEvents.GetOrAddEvent<object>()
		// object QFramework.IArchitecture.GetModel<object>()
		// System.Void QFramework.IManager.RegisterEvent<int>(int,System.Action<int,object[]>)
		// System.Void QFramework.IManager.SendEvent<int>(int)
		// System.Void QFramework.IManager.UnRegisterEvent<int>(int,System.Action<int,object[]>)
		// System.Void QFramework.IOCContainer.Register<object>(object)
		// System.Void QFramework.QMonoBehaviour.SendEvent<int>(int)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.GameEvents.CreateRobot>(System.Action<SquareHero.Hotfix.Events.GameEvents.CreateRobot>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.GameEvents.OnClientGameOver>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnClientGameOver>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.GameEvents.OnClientStartGame>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnClientStartGame>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.GameEvents.OnServerGameOver>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnServerGameOver>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.GameEvents.OnServerStartGame>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnServerStartGame>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap>(System.Action<SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.PlayerEvents.GetCoin>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.GetCoin>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.PlayerEvents.JoinGame>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.JoinGame>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerInitialized>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerInitialized>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets>(System.Action<SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets>)
		// QFramework.IUnRegister QFramework.TypeEventSystem.Register<SquareHero.Hotfix.SystemEvents.UpdatePlayerProps>(System.Action<SquareHero.Hotfix.SystemEvents.UpdatePlayerProps>)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.GameEvents.GameReadyedOnClient>(SquareHero.Hotfix.Events.GameEvents.GameReadyedOnClient)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized>(SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed>(SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.GameEvents.OnClientGameOver>(SquareHero.Hotfix.Events.GameEvents.OnClientGameOver)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene>(SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.GameEvents.OnClientStartGame>(SquareHero.Hotfix.Events.GameEvents.OnClientStartGame)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange>(SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.GameEvents.OnServerGameOver>(SquareHero.Hotfix.Events.GameEvents.OnServerGameOver)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.GameEvents.OnServerStartGame>(SquareHero.Hotfix.Events.GameEvents.OnServerStartGame)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap>(SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.MapEvents.OnServerFinishCreateMap>(SquareHero.Hotfix.Events.MapEvents.OnServerFinishCreateMap)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.PlayerEvents.GetCoin>(SquareHero.Hotfix.Events.PlayerEvents.GetCoin)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.PlayerEvents.JoinGame>(SquareHero.Hotfix.Events.PlayerEvents.JoinGame)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode>(SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.PlayerEvents.OnPropAffectEnd>(SquareHero.Hotfix.Events.PlayerEvents.OnPropAffectEnd)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.PlayerEvents.OnPropUse>(SquareHero.Hotfix.Events.PlayerEvents.OnPropUse)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived>(SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets>(SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets)
		// System.Void QFramework.TypeEventSystem.Send<SquareHero.Hotfix.SystemEvents.UpdatePlayerProps>(SquareHero.Hotfix.SystemEvents.UpdatePlayerProps)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.GameEvents.CreateRobot>(System.Action<SquareHero.Hotfix.Events.GameEvents.CreateRobot>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerInitialized>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnAllPlayerReadyed>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.GameEvents.OnClientGameOver>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnClientGameOver>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnClientLoadedScene>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.GameEvents.OnClientStartGame>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnClientStartGame>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnCoinRankChange>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.GameEvents.OnServerGameOver>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnServerGameOver>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.GameEvents.OnServerStartGame>(System.Action<SquareHero.Hotfix.Events.GameEvents.OnServerStartGame>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap>(System.Action<SquareHero.Hotfix.Events.MapEvents.OnClientFinishCreateMap>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.PlayerEvents.GetCoin>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.GetCoin>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.PlayerEvents.JoinGame>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.JoinGame>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerInitialized>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerInitialized>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.OnPlayerTriggerMoveNode>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived>(System.Action<SquareHero.Hotfix.Events.PlayerEvents.PlayerArrived>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets>(System.Action<SquareHero.Hotfix.SystemEvents.UpdatePlayerAssets>)
		// System.Void QFramework.TypeEventSystem.UnRegister<SquareHero.Hotfix.SystemEvents.UpdatePlayerProps>(System.Action<SquareHero.Hotfix.SystemEvents.UpdatePlayerProps>)
		// System.Void QFramework.UIKit.ClosePanel<object>()
		// object QFramework.UIKit.GetPanel<object>()
		// System.Void QFramework.UIKit.OpenPanelAsync<object>(System.Action<object>,QFramework.UILevel,QFramework.IUIData,string,string)
		// System.Void QFramework.UnityEngineObjectExtension.DestroySelf<object>(object)
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// int System.Array.IndexOf<object>(object[],object)
		// int System.Array.IndexOfImpl<object>(object[],object,int,int)
		// System.Void System.Array.Reverse<byte>(byte[])
		// System.Void System.Array.Reverse<byte>(byte[],int,int)
		// System.Collections.Generic.List<object> System.Collections.Generic.List<object>.ConvertAll<object>(System.Converter<object,object>)
		// System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.Reverse<byte>(System.Collections.Generic.IEnumerable<byte>)
		// System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.ReverseIterator<byte>(System.Collections.Generic.IEnumerable<byte>)
		// byte[] System.Linq.Enumerable.ToArray<byte>(System.Collections.Generic.IEnumerable<byte>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// byte& System.Runtime.CompilerServices.Unsafe.Add<byte>(byte&,int)
		// byte& System.Runtime.CompilerServices.Unsafe.As<byte,byte>(byte&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// bool System.Runtime.CompilerServices.Unsafe.IsAddressLessThan<byte>(byte&,byte&)
		// object System.Threading.Interlocked.CompareExchange<object>(object&,object,object)
		// SquareHero.Hotfix.AllPropResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.AllPropResponse>(string)
		// SquareHero.Hotfix.BaseResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.BaseResponse>(string)
		// SquareHero.Hotfix.BuyMessageResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.BuyMessageResponse>(string)
		// SquareHero.Hotfix.GetRoleResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.GetRoleResponse>(string)
		// SquareHero.Hotfix.LoginResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.LoginResponse>(string)
		// SquareHero.Hotfix.PlayerassetsResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.PlayerassetsResponse>(string)
		// SquareHero.Hotfix.SelectPropsResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.SelectPropsResponse>(string)
		// SquareHero.Hotfix.SpinExchangeResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.SpinExchangeResponse>(string)
		// SquareHero.Hotfix.SpinInfoResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.SpinInfoResponse>(string)
		// SquareHero.Hotfix.SpinResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.SpinResponse>(string)
		// SquareHero.Hotfix.WalletDataResponse UnityCommon.Util.JsonUtil.FromJson<SquareHero.Hotfix.WalletDataResponse>(string)
		// object UnityCommon.Util.JsonUtil.FromJson<object>(string)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInParent<object>()
		// System.Void UnityEngine.Component.GetComponentsInChildren<object>(System.Collections.Generic.List<object>)
		// System.Void UnityEngine.Component.GetComponentsInChildren<object>(bool,System.Collections.Generic.List<object>)
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// System.Void UnityEngine.GameObject.GetComponentsInChildren<object>(bool,System.Collections.Generic.List<object>)
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion)
		// object UnityEngine.Resources.Load<object>(string)
		// System.Void UnityEngine.UI.LayoutGroup.SetProperty<byte>(byte&,byte)
		// System.Void UnityEngine.UI.LayoutGroup.SetProperty<float>(float&,float)
		// System.Void UnityEngine.UI.LayoutGroup.SetProperty<int>(int&,int)
		// object YooAsset.AssetOperationHandle.GetAssetObject<object>()
		// YooAsset.AssetOperationHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string)
		// YooAsset.AssetOperationHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
		// YooAsset.AssetOperationHandle YooAsset.YooAssets.LoadAssetAsync<object>(string)
		// YooAsset.AssetOperationHandle YooAsset.YooAssets.LoadAssetSync<object>(string)
	}
}