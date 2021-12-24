using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"uint\", \"char\"][\"int\"][\"Vector3\", \"Vector3\"][\"string\", \"float\"][\"char\", \"string\"][\"char\", \"string\", \"Vector3\"][\"char\", \"string\"][\"char\"][][\"int\", \"Vector3\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"playerId\", \"team\"][\"gunIndex\"][\"hitpoint\", \"hitnormal\"][\"targetName\", \"damage\"][\"flagteam\", \"player\"][\"flagteam\", \"player\", \"newPos\"][\"team\", \"player\"][\"team\"][][\"SoundIndex\", \"pos\"]]")]
	public abstract partial class NetworkedPlayerBehavior : NetworkBehavior
	{
		public const byte RPC_SET_TEAM = 0 + 5;
		public const byte RPC_SET_GUN = 1 + 5;
		public const byte RPC_SHOOT_GUN = 2 + 5;
		public const byte RPC_DO_DAMAGE = 3 + 5;
		public const byte RPC_PICKUP_FLAG = 4 + 5;
		public const byte RPC_DROP_FLAG = 5 + 5;
		public const byte RPC_TEAM_SCORE = 6 + 5;
		public const byte RPC_RESPAWN_FLAG = 7 + 5;
		public const byte RPC_JUMP = 8 + 5;
		public const byte RPC_PLAY_SOUND = 9 + 5;
		
		public NetworkedPlayerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (NetworkedPlayerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("SetTeam", SetTeam, typeof(uint), typeof(char));
			networkObject.RegisterRpc("SetGun", SetGun, typeof(int));
			networkObject.RegisterRpc("ShootGun", ShootGun, typeof(Vector3), typeof(Vector3));
			networkObject.RegisterRpc("DoDamage", DoDamage, typeof(string), typeof(float));
			networkObject.RegisterRpc("PickupFlag", PickupFlag, typeof(char), typeof(string));
			networkObject.RegisterRpc("DropFlag", DropFlag, typeof(char), typeof(string), typeof(Vector3));
			networkObject.RegisterRpc("TeamScore", TeamScore, typeof(char), typeof(string));
			networkObject.RegisterRpc("RespawnFlag", RespawnFlag, typeof(char));
			networkObject.RegisterRpc("Jump", Jump);
			networkObject.RegisterRpc("PlaySound", PlaySound, typeof(int), typeof(Vector3));

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId)){
					uint newId = obj.NetworkId + 1;
					ProcessOthers(gameObject.transform, ref newId);
				}
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new NetworkedPlayerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new NetworkedPlayerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// uint playerId
		/// char team
		/// </summary>
		public abstract void SetTeam(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int gunIndex
		/// </summary>
		public abstract void SetGun(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// Vector3 hitpoint
		/// Vector3 hitnormal
		/// </summary>
		public abstract void ShootGun(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string targetName
		/// float damage
		/// </summary>
		public abstract void DoDamage(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// char flagteam
		/// string player
		/// </summary>
		public abstract void PickupFlag(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// char flagteam
		/// string player
		/// Vector3 newPos
		/// </summary>
		public abstract void DropFlag(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// char team
		/// string player
		/// </summary>
		public abstract void TeamScore(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// char team
		/// </summary>
		public abstract void RespawnFlag(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Jump(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void PlaySound(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}