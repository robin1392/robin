using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Match.RandomwarsMatch.Common
{
	public class RandomwarsMatchProtocol : ProtocolBase
	{
		public RandomwarsMatchProtocol(ISender sender = null, string serverAddr = "")
			: base(sender, serverAddr)
		{
			MessageControllers.Add(MatchRequestRequest.ProtocolId, MatchRequestReqController);
			MessageControllers.Add(MatchRequestResponse.ProtocolId, MatchRequestResController);
			MessageControllers.Add(MatchStatusRequest.ProtocolId, MatchStatusReqController);
			MessageControllers.Add(MatchStatusResponse.ProtocolId, MatchStatusResController);
			MessageControllers.Add(MatchCancelRequest.ProtocolId, MatchCancelReqController);
			MessageControllers.Add(MatchCancelResponse.ProtocolId, MatchCancelResController);
			MessageControllers.Add(MatchInviteRequest.ProtocolId, MatchInviteReqController);
			MessageControllers.Add(MatchInviteResponse.ProtocolId, MatchInviteResController);
			MessageControllers.Add(MatchJoinRequest.ProtocolId, MatchJoinReqController);
			MessageControllers.Add(MatchJoinResponse.ProtocolId, MatchJoinResController);
		}

		#region MatchRequest ------------------------------------------
		public bool MatchRequestReq(MatchRequestRequest request, MatchRequestResCallback callback)
		{
			OnMatchRequestResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MatchRequestRequest.ProtocolId,
				_serverAddr + "matchrequest-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchRequestResponse MatchRequestReqCallback(MatchRequestRequest request);
		public MatchRequestReqCallback OnMatchRequestReqCallback;
		public ISerializer MatchRequestReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchRequestRequest request = new MatchRequestRequest();
			request.JsonDeserialize(json);
			return OnMatchRequestReqCallback(request) as ISerializer;
		}

		public delegate bool MatchRequestResCallback(MatchRequestResponse response);
		public MatchRequestResCallback OnMatchRequestResCallback;
		public ISerializer MatchRequestResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchRequestResponse response = new MatchRequestResponse();
			response.JsonDeserialize(json);
			OnMatchRequestResCallback(response);
			return null;
		}
		#endregion

		#region MatchStatus ------------------------------------------
		public bool MatchStatusReq(MatchStatusRequest request, MatchStatusResCallback callback)
		{
			OnMatchStatusResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MatchStatusRequest.ProtocolId,
				_serverAddr + "matchstatus-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchStatusResponse MatchStatusReqCallback(MatchStatusRequest request);
		public MatchStatusReqCallback OnMatchStatusReqCallback;
		public ISerializer MatchStatusReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchStatusRequest request = new MatchStatusRequest();
			request.JsonDeserialize(json);
			return OnMatchStatusReqCallback(request) as ISerializer;
		}

		public delegate bool MatchStatusResCallback(MatchStatusResponse response);
		public MatchStatusResCallback OnMatchStatusResCallback;
		public ISerializer MatchStatusResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchStatusResponse response = new MatchStatusResponse();
			response.JsonDeserialize(json);
			OnMatchStatusResCallback(response);
			return null;
		}
		#endregion

		#region MatchCancel ------------------------------------------
		public bool MatchCancelReq(MatchCancelRequest request, MatchCancelResCallback callback)
		{
			OnMatchCancelResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MatchCancelRequest.ProtocolId,
				_serverAddr + "matchcancel-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchCancelResponse MatchCancelReqCallback(MatchCancelRequest request);
		public MatchCancelReqCallback OnMatchCancelReqCallback;
		public ISerializer MatchCancelReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchCancelRequest request = new MatchCancelRequest();
			request.JsonDeserialize(json);
			return OnMatchCancelReqCallback(request) as ISerializer;
		}

		public delegate bool MatchCancelResCallback(MatchCancelResponse response);
		public MatchCancelResCallback OnMatchCancelResCallback;
		public ISerializer MatchCancelResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchCancelResponse response = new MatchCancelResponse();
			response.JsonDeserialize(json);
			OnMatchCancelResCallback(response);
			return null;
		}
		#endregion

		#region MatchInvite ------------------------------------------
		public bool MatchInviteReq(MatchInviteRequest request, MatchInviteResCallback callback)
		{
			OnMatchInviteResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MatchInviteRequest.ProtocolId,
				_serverAddr + "matchinvite-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchInviteResponse MatchInviteReqCallback(MatchInviteRequest request);
		public MatchInviteReqCallback OnMatchInviteReqCallback;
		public ISerializer MatchInviteReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchInviteRequest request = new MatchInviteRequest();
			request.JsonDeserialize(json);
			return OnMatchInviteReqCallback(request) as ISerializer;
		}

		public delegate bool MatchInviteResCallback(MatchInviteResponse response);
		public MatchInviteResCallback OnMatchInviteResCallback;
		public ISerializer MatchInviteResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchInviteResponse response = new MatchInviteResponse();
			response.JsonDeserialize(json);
			OnMatchInviteResCallback(response);
			return null;
		}
		#endregion

		#region MatchJoin ------------------------------------------
		public bool MatchJoinReq(MatchJoinRequest request, MatchJoinResCallback callback)
		{
			OnMatchJoinResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MatchJoinRequest.ProtocolId,
				_serverAddr + "matchjoin-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchJoinResponse MatchJoinReqCallback(MatchJoinRequest request);
		public MatchJoinReqCallback OnMatchJoinReqCallback;
		public ISerializer MatchJoinReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchJoinRequest request = new MatchJoinRequest();
			request.JsonDeserialize(json);
			return OnMatchJoinReqCallback(request) as ISerializer;
		}

		public delegate bool MatchJoinResCallback(MatchJoinResponse response);
		public MatchJoinResCallback OnMatchJoinResCallback;
		public ISerializer MatchJoinResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchJoinResponse response = new MatchJoinResponse();
			response.JsonDeserialize(json);
			OnMatchJoinResCallback(response);
			return null;
		}
		#endregion

	}
}
