// using System;
// using System.Collections.Generic;
// using Cinemachine;
// using Mirror;
// using QFramework;
// using Sirenix.Serialization;
// using champs3.Hotfix.Events;
// using champs3.Hotfix.GameLogic;
// using champs3.Hotfix.GameLogic.Level1;
// using champs3.Hotfix.Map;
// using UnityEngine;
// using UnityEngine.SceneManagement;
//
// namespace champs3.Hotfix.Player
// {
//     public class PlayerMoveController : NetworkBehaviour
//     {
//         #region EDITOR EXPOSED FIELDS
//         public float SpeedChangeRate = 10;
//         public float Gravity = -9.8f;
//         public float DivedForce = 5;
//         public float time = 0.2f;
//         [OdinSerialize]
//         public Dictionary<MoveType, float> MoveSpeedAdditions = new Dictionary<MoveType, float>()
//         {
//             {MoveType.None,0},
//             {MoveType.Run,0},
//             {MoveType.Swim,0},
//             {MoveType.Climb,0},
//             {MoveType.Fly,0},
//         };
//         #endregion
//         
//         #region FIELDS
//         [SerializeField]
//         private MoveNode _currentMoveNode;
//         [SerializeField]
//         private MoveNode _targetMoveNode;
//         private CharacterAnimatorController _animCtrl;
//         private float _speed;
//         private float _verticalSpeed;
//         [SerializeField]
//         private float _targetSpeed;
//         private bool _isGameing;
//         private bool _applyGravity;
//         private Vector3 _moveDir;
//         private float _speedAddition;
//
//         #endregion
//
//         #region PROPERTIES
//
//         public float SpeedAddition
//         {
//             get { return _speed;}
//             set
//             {
//                 _speed = value;
//             }
//         }
//
//         #endregion
//
//
//         #region EVENT FUNCTION
//
//         private void OnTriggerEnter(Collider other)
//         {
//             if (isServer)
//             {
//                 this.Log($"OnTriggerEnter {other.transform.parent.parent.name}.{other.name}.{other.tag}");
//                 if (other.CompareTag("MoveNode"))
//                 {
//                     var moveNode = other.GetComponent<MoveNode>();
//
//                     if (_currentMoveNode != null && (_currentMoveNode.ActionType == ActionType.ClimbingOnTop))
//                     {
//                         var position = transform.position;
//
//                         transform.position = new Vector3(position.x, moveNode.transform.position.y, position.z);
//                     }
//                     
//                     _currentMoveNode = moveNode;
//                     
//                     TypeEventSystem.Global.Send(new PlayerEvents.OnPlayerTriggerMoveNode()
//                     {
//                         Player = gameObject,
//                         MoveNode = moveNode
//                     });
//                     
//                     var mapModel = LevelManager.Instance.CurrentLevelController.GetArchitecture().GetModel<MapModel>();
//                     
//                     int index = _currentMoveNode.Index + 1;
//                     if (index < mapModel.MoveNodes.Count)
//                     {
//                         _targetMoveNode = mapModel.MoveNodes[index];
//                         _moveDir = _currentMoveNode.MoveDir.normalized;
//
//                         if (_currentMoveNode.ActionType == ActionType.FlyStart)
//                         {
//                             _moveDir = Vector3.ProjectOnPlane((_targetMoveNode.transform.position - Vector3.up * 0.6f - transform.position), transform.right).normalized;
//                         }
//                         
//                     }
//                     else
//                     {
//                         _targetMoveNode = null;
//                         _moveDir = Vector3.zero;
//                     }
//                     
//                     if (_currentMoveNode != null && (_currentMoveNode.ActionType == ActionType.FlyEnd))
//                     {
//                         var position = transform.position;
//
//                         transform.position = new Vector3(position.x, moveNode.transform.position.y, position.z);
//                     }
//
//                     if (_currentMoveNode.ActionType == ActionType.Dive)
//                     {
//                         _applyGravity = true;
//                         _verticalSpeed = DivedForce;
//                     }
//                     else if (_currentMoveNode.ActionType == ActionType.ClimbingOnTop)
//                     {
//                         _verticalSpeed = 1f;
//                     }
//                     else
//                     {
//                         _applyGravity = false;
//                         _verticalSpeed = 0;
//                     }
//                     
//                     if (_currentMoveNode.ActionType == ActionType.Idle)
//                     {
//                         _targetSpeed = 0f;
//                         _speed = _targetSpeed;
//                     }
//                     _animCtrl.SetMoveType(_currentMoveNode.ActionType);
//                 }
//             }
//         }
//
//         private void Update()
//         {
//
//             if (!_isGameing)
//             {
//                 return;
//             }
//             if (isServer)
//             {
//                 UpdateGravity();
//                 UpdateMove();
//             }
//
//         }
//
//         private void UpdateMove()
//         {
//             if (_targetMoveNode)
//             {
//                 
//                 Vector3 position = transform.position;
//                 _targetSpeed = GetSpeed(_currentMoveNode.MoveType);
//                 var motionSpeed = GetMotionSpeed(_currentMoveNode.MoveType);
//                 _animCtrl.SetMotionSeed(motionSpeed);
//                 
//                 if (_currentMoveNode.ActionType == ActionType.Dive || _currentMoveNode.ActionType == ActionType.ClimbStart || _currentMoveNode.ActionType == ActionType.ClimbingOnTop)
//                 {
//                     _targetSpeed = 0.8f;
//                     _speed = _targetSpeed;
//                 }
//                 
//                 _speed = Mathf.Lerp(_speed, _targetSpeed, Time.deltaTime * SpeedChangeRate);
//                 _animCtrl.SetSpeed(_speed);
//                 transform.position = position + _moveDir * _speed * Time.deltaTime + Vector3.up * _verticalSpeed * Time.deltaTime;
//             }
//         }
//
//         private void UpdateGravity()
//         {
//             if (_applyGravity)
//             {
//                 _verticalSpeed += Gravity * Time.deltaTime;
//             }
//         }
//
//         private void OnDestroy()
//         {
//
//         }
//
//
//
//         #endregion
//
//         #region METHODS
//
//         public override void OnStartServer()
//         {
//             base.OnStartServer();
//             _animCtrl = GetComponent<CharacterAnimatorController>();
//             _animCtrl.Initialize();
//             RegisteServerEvents();
//         }
//
//
//         public override void OnStopServer()
//         {
//             base.OnStopServer();
//             UnRegisteServerEvents();
//         }
//
//         private void OnServerStartGame(GameEvents.OnServerStartGame evt)
//         {
//             _isGameing = true;
//             _targetSpeed = GetSpeed(MoveType.Run);
//             _animCtrl.SetMoveType(ActionType.Run);
//             // RpcClientStartGame();
//
//             
//
//             // transform.parent = PlayerContainer.Instance.transform;
//             // var asset = ResourceManager.Instance.GetAsset<GameObject>("Prefabs_CameraTrigger");
//             // Instantiate(asset, transform);
//         }
//
//         #region Client
//         
//         [ClientRpc]
//         private void RpcClientStartGame()
//         {
//             // if (isClientOnly)
//             // {
//             //     if (gameObject.scene  != SceneManager.GetActiveScene())
//             //     {
//             //         transform.parent = null;
//             //         SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
//             //     }
//             //     
//             //     transform.parent = PlayerContainer.Instance.transform;
//             // }
//         }
//
//         #endregion
//
//         private void RegisteServerEvents()
//         {
//             TypeEventSystem.Global.Register<GameEvents.OnServerStartGame>(OnServerStartGame);
//         }
//
//         private void UnRegisteServerEvents()
//         {
//             TypeEventSystem.Global.UnRegister<GameEvents.OnServerStartGame>(OnServerStartGame);
//         }
//
//         public void SetSpeedAddition(float addition)
//         {
//             _speedAddition = addition;
//         }
//         
//         public float GetSpeed(MoveType moveType)
//         {
//             float speed = 0;
//             var controller = GetComponent<SHPlayerController>();
//             switch (moveType)
//             {
//                 case MoveType.Run:
//                     speed = controller.Data.RunSpeed;
//                     break;
//                 case MoveType.Swim:
//                     speed = controller.Data.SwimSpeed;
//                     break;
//                 case MoveType.Climb:
//                     speed = controller.Data.ClimbSpeed;
//                     break;
//                 case MoveType.Fly:
//                     speed = controller.Data.RunSpeed;
//                     break;
//             }
//             
//             speed += speed * MoveSpeedAdditions[moveType];
//             return speed;
//         }
//
//
//         public float GetMotionSpeed(MoveType moveType)
//         {
//             
//             float speed = 1;
//             switch (moveType)
//             {
//                 case MoveType.Run:
//                     speed = _speed / 4f;
//                     break;
//                 case MoveType.Swim:
//                     speed = _speed / 3f;
//                     break;
//                 case MoveType.Climb:
//                     speed = _speed / 2f;
//                     break;
//                 case MoveType.Fly:
//                     speed = _speed / 3f;
//                     break;
//             }
//
//             return speed;
//         }
//
//         #endregion
//     }
// }