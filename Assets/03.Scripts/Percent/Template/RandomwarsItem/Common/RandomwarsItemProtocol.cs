using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Item.RandomwarsItem.Common
{
	public class RandomwarsItemProtocol : ProtocolBase
	{
		public RandomwarsItemProtocol(ISender sender = null, string serverAddr = "")
			: base(sender, serverAddr)
		{
			MessageControllers.Add(BoxOpenRequest.ProtocolId, BoxOpenReqController);
			MessageControllers.Add(BoxOpenResponse.ProtocolId, BoxOpenResController);
			MessageControllers.Add(EmoticonEquipRequest.ProtocolId, EmoticonEquipReqController);
			MessageControllers.Add(EmoticonEquipResponse.ProtocolId, EmoticonEquipResController);
		}

		#region BoxOpen ------------------------------------------
		public bool BoxOpenReq(BoxOpenRequest request, BoxOpenResCallback callback)
		{
			OnBoxOpenResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(BoxOpenRequest.ProtocolId,
				_serverAddr + "boxopen-v1.0.0",
				request.JsonSerialize());
		}

		public delegate BoxOpenResponse BoxOpenReqCallback(BoxOpenRequest request);
		public BoxOpenReqCallback OnBoxOpenReqCallback;
		public ISerializer BoxOpenReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			BoxOpenRequest request = new BoxOpenRequest();
			request.JsonDeserialize(json);
			return OnBoxOpenReqCallback(request) as ISerializer;
		}

		public delegate bool BoxOpenResCallback(BoxOpenResponse response);
		public BoxOpenResCallback OnBoxOpenResCallback;
		public ISerializer BoxOpenResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			BoxOpenResponse response = new BoxOpenResponse();
			response.JsonDeserialize(json);
			OnBoxOpenResCallback(response);
			return null;
		}
		#endregion

		#region EmoticonEquip ------------------------------------------
		public bool EmoticonEquipReq(EmoticonEquipRequest request, EmoticonEquipResCallback callback)
		{
			OnEmoticonEquipResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(EmoticonEquipRequest.ProtocolId,
				_serverAddr + "emoticonequip-v1.0.0",
				request.JsonSerialize());
		}

		public delegate EmoticonEquipResponse EmoticonEquipReqCallback(EmoticonEquipRequest request);
		public EmoticonEquipReqCallback OnEmoticonEquipReqCallback;
		public ISerializer EmoticonEquipReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			EmoticonEquipRequest request = new EmoticonEquipRequest();
			request.JsonDeserialize(json);
			return OnEmoticonEquipReqCallback(request) as ISerializer;
		}

		public delegate bool EmoticonEquipResCallback(EmoticonEquipResponse response);
		public EmoticonEquipResCallback OnEmoticonEquipResCallback;
		public ISerializer EmoticonEquipResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			EmoticonEquipResponse response = new EmoticonEquipResponse();
			response.JsonDeserialize(json);
			OnEmoticonEquipResCallback(response);
			return null;
		}
		#endregion

	}
}
