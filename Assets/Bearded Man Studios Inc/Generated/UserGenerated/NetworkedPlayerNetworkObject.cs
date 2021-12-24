using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15,0.15,0,0,0,0]")]
	public partial class NetworkedPlayerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 7;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Vector3 _pos;
		public event FieldEvent<Vector3> posChanged;
		public InterpolateVector3 posInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 pos
		{
			get { return _pos; }
			set
			{
				// Don't do anything if the value is the same
				if (_pos == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_pos = value;
				hasDirtyFields = true;
			}
		}

		public void SetposDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_pos(ulong timestep)
		{
			if (posChanged != null) posChanged(_pos, timestep);
			if (fieldAltered != null) fieldAltered("pos", _pos, timestep);
		}
		[ForgeGeneratedField]
		private Quaternion _rot;
		public event FieldEvent<Quaternion> rotChanged;
		public InterpolateQuaternion rotInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };
		public Quaternion rot
		{
			get { return _rot; }
			set
			{
				// Don't do anything if the value is the same
				if (_rot == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_rot = value;
				hasDirtyFields = true;
			}
		}

		public void SetrotDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_rot(ulong timestep)
		{
			if (rotChanged != null) rotChanged(_rot, timestep);
			if (fieldAltered != null) fieldAltered("rot", _rot, timestep);
		}
		[ForgeGeneratedField]
		private char _team;
		public event FieldEvent<char> teamChanged;
		public Interpolated<char> teamInterpolation = new Interpolated<char>() { LerpT = 0f, Enabled = false };
		public char team
		{
			get { return _team; }
			set
			{
				// Don't do anything if the value is the same
				if (_team == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_team = value;
				hasDirtyFields = true;
			}
		}

		public void SetteamDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_team(ulong timestep)
		{
			if (teamChanged != null) teamChanged(_team, timestep);
			if (fieldAltered != null) fieldAltered("team", _team, timestep);
		}
		[ForgeGeneratedField]
		private long _name;
		public event FieldEvent<long> nameChanged;
		public Interpolated<long> nameInterpolation = new Interpolated<long>() { LerpT = 0f, Enabled = false };
		public long name
		{
			get { return _name; }
			set
			{
				// Don't do anything if the value is the same
				if (_name == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_name = value;
				hasDirtyFields = true;
			}
		}

		public void SetnameDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_name(ulong timestep)
		{
			if (nameChanged != null) nameChanged(_name, timestep);
			if (fieldAltered != null) fieldAltered("name", _name, timestep);
		}
		[ForgeGeneratedField]
		private int _model;
		public event FieldEvent<int> modelChanged;
		public Interpolated<int> modelInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int model
		{
			get { return _model; }
			set
			{
				// Don't do anything if the value is the same
				if (_model == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x10;
				_model = value;
				hasDirtyFields = true;
			}
		}

		public void SetmodelDirty()
		{
			_dirtyFields[0] |= 0x10;
			hasDirtyFields = true;
		}

		private void RunChange_model(ulong timestep)
		{
			if (modelChanged != null) modelChanged(_model, timestep);
			if (fieldAltered != null) fieldAltered("model", _model, timestep);
		}
		[ForgeGeneratedField]
		private float _walking;
		public event FieldEvent<float> walkingChanged;
		public InterpolateFloat walkingInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
		public float walking
		{
			get { return _walking; }
			set
			{
				// Don't do anything if the value is the same
				if (_walking == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x20;
				_walking = value;
				hasDirtyFields = true;
			}
		}

		public void SetwalkingDirty()
		{
			_dirtyFields[0] |= 0x20;
			hasDirtyFields = true;
		}

		private void RunChange_walking(ulong timestep)
		{
			if (walkingChanged != null) walkingChanged(_walking, timestep);
			if (fieldAltered != null) fieldAltered("walking", _walking, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			posInterpolation.current = posInterpolation.target;
			rotInterpolation.current = rotInterpolation.target;
			teamInterpolation.current = teamInterpolation.target;
			nameInterpolation.current = nameInterpolation.target;
			modelInterpolation.current = modelInterpolation.target;
			walkingInterpolation.current = walkingInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _pos);
			UnityObjectMapper.Instance.MapBytes(data, _rot);
			UnityObjectMapper.Instance.MapBytes(data, _team);
			UnityObjectMapper.Instance.MapBytes(data, _name);
			UnityObjectMapper.Instance.MapBytes(data, _model);
			UnityObjectMapper.Instance.MapBytes(data, _walking);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_pos = UnityObjectMapper.Instance.Map<Vector3>(payload);
			posInterpolation.current = _pos;
			posInterpolation.target = _pos;
			RunChange_pos(timestep);
			_rot = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			rotInterpolation.current = _rot;
			rotInterpolation.target = _rot;
			RunChange_rot(timestep);
			_team = UnityObjectMapper.Instance.Map<char>(payload);
			teamInterpolation.current = _team;
			teamInterpolation.target = _team;
			RunChange_team(timestep);
			_name = UnityObjectMapper.Instance.Map<long>(payload);
			nameInterpolation.current = _name;
			nameInterpolation.target = _name;
			RunChange_name(timestep);
			_model = UnityObjectMapper.Instance.Map<int>(payload);
			modelInterpolation.current = _model;
			modelInterpolation.target = _model;
			RunChange_model(timestep);
			_walking = UnityObjectMapper.Instance.Map<float>(payload);
			walkingInterpolation.current = _walking;
			walkingInterpolation.target = _walking;
			RunChange_walking(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _pos);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _rot);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _team);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _name);
			if ((0x10 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _model);
			if ((0x20 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _walking);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (posInterpolation.Enabled)
				{
					posInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					posInterpolation.Timestep = timestep;
				}
				else
				{
					_pos = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_pos(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (rotInterpolation.Enabled)
				{
					rotInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					rotInterpolation.Timestep = timestep;
				}
				else
				{
					_rot = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_rot(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (teamInterpolation.Enabled)
				{
					teamInterpolation.target = UnityObjectMapper.Instance.Map<char>(data);
					teamInterpolation.Timestep = timestep;
				}
				else
				{
					_team = UnityObjectMapper.Instance.Map<char>(data);
					RunChange_team(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (nameInterpolation.Enabled)
				{
					nameInterpolation.target = UnityObjectMapper.Instance.Map<long>(data);
					nameInterpolation.Timestep = timestep;
				}
				else
				{
					_name = UnityObjectMapper.Instance.Map<long>(data);
					RunChange_name(timestep);
				}
			}
			if ((0x10 & readDirtyFlags[0]) != 0)
			{
				if (modelInterpolation.Enabled)
				{
					modelInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					modelInterpolation.Timestep = timestep;
				}
				else
				{
					_model = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_model(timestep);
				}
			}
			if ((0x20 & readDirtyFlags[0]) != 0)
			{
				if (walkingInterpolation.Enabled)
				{
					walkingInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					walkingInterpolation.Timestep = timestep;
				}
				else
				{
					_walking = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_walking(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (posInterpolation.Enabled && !posInterpolation.current.UnityNear(posInterpolation.target, 0.0015f))
			{
				_pos = (Vector3)posInterpolation.Interpolate();
				//RunChange_pos(posInterpolation.Timestep);
			}
			if (rotInterpolation.Enabled && !rotInterpolation.current.UnityNear(rotInterpolation.target, 0.0015f))
			{
				_rot = (Quaternion)rotInterpolation.Interpolate();
				//RunChange_rot(rotInterpolation.Timestep);
			}
			if (teamInterpolation.Enabled && !teamInterpolation.current.UnityNear(teamInterpolation.target, 0.0015f))
			{
				_team = (char)teamInterpolation.Interpolate();
				//RunChange_team(teamInterpolation.Timestep);
			}
			if (nameInterpolation.Enabled && !nameInterpolation.current.UnityNear(nameInterpolation.target, 0.0015f))
			{
				_name = (long)nameInterpolation.Interpolate();
				//RunChange_name(nameInterpolation.Timestep);
			}
			if (modelInterpolation.Enabled && !modelInterpolation.current.UnityNear(modelInterpolation.target, 0.0015f))
			{
				_model = (int)modelInterpolation.Interpolate();
				//RunChange_model(modelInterpolation.Timestep);
			}
			if (walkingInterpolation.Enabled && !walkingInterpolation.current.UnityNear(walkingInterpolation.target, 0.0015f))
			{
				_walking = (float)walkingInterpolation.Interpolate();
				//RunChange_walking(walkingInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public NetworkedPlayerNetworkObject() : base() { Initialize(); }
		public NetworkedPlayerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public NetworkedPlayerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
