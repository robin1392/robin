using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Item.RandomwarsItem.Common
{
	public class RandomwarsItemProtocol
	{
		public readonly ClientSession Session;
		public readonly string ServerAddr;
		public Dictionary<int, ControllerDelegate> MessageControllers { get;  private set; }

		public RandomwarsItemProtocol()
		{
			Session = null;
			ServerAddr = string.Empty;

			Init();
		}

		public RandomwarsItemProtocol(ClientSession session, string serverAddr)
		{
			Session = session;
			ServerAddr = serverAddr;

			Init();
		}

		void Init()
		{
			MessageControllers = new Dictionary<int, ControllerDelegate>();
			MessageControllers.Add(BoxOpenRequest.ProtocolId, BoxOpenReqController);
			MessageControllers.Add(BoxOpenResponse.ProtocolId, BoxOpenResController);
			MessageControllers.Add(EmoticonEquipRequest.ProtocolId, EmoticonEquipReqController);
			MessageControllers.Add(EmoticonEquipResponse.ProtocolId, EmoticonEquipResController);
		}

		#region BoxOpen ------------------------------------------
		public bool BoxOpenReq(BoxOpenRequest request, BoxOpenResCallback callback)
		{
			OnBoxOpenResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(BoxOpenRequest.ProtocolId,
				ServerAddr + "boxopen-v1.0.0",
				request.JsonSerialize());
		}

		public delegate BoxOpenResponse BoxOpenReqCallback(ClientSession session, BoxOpenRequest request);
		public BoxOpenReqCallback OnBoxOpenReqCallback;
		public ISerializer BoxOpenReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			BoxOpenRequest request = BoxOpenRequest.JsonDeserialize(json);
			return OnBoxOpenReqCallback(session, request) as ISerializer;
		}

		public delegate bool BoxOpenResCallback(BoxOpenResponse response);
		public BoxOpenResCallback OnBoxOpenResCallback;
		public ISerializer BoxOpenResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			BoxOpenResponse response = BoxOpenResponse.JsonDeserialize(json);
			OnBoxOpenResCallback(response);
			return null;
		}
		#endregion

		#region EmoticonEquip ------------------------------------------
		public bool EmoticonEquipReq(EmoticonEquipRequest request, EmoticonEquipResCallback callback)
		{
			OnEmoticonEquipResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(EmoticonEquipRequest.ProtocolId,
				ServerAddr + "emoticonequip-v1.0.0",
				request.JsonSerialize());
		}

		public delegate EmoticonEquipResponse EmoticonEquipReqCallback(ClientSession session, EmoticonEquipRequest request);
		public EmoticonEquipReqCallback OnEmoticonEquipReqCallback;
		public ISerializer EmoticonEquipReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			EmoticonEquipRequest request = EmoticonEquipRequest.JsonDeserialize(json);
			return OnEmoticonEquipReqCallback(session, request) as ISerializer;
		}

		public delegate bool EmoticonEquipResCallback(EmoticonEquipResponse response);
		public EmoticonEquipResCallback OnEmoticonEquipResCallback;
		public ISerializer EmoticonEquipResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			EmoticonEquipResponse response = EmoticonEquipResponse.JsonDeserialize(json);
			OnEmoticonEquipResCallback(response);
			return null;
		}
		#endregion

	}
}
