using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Match.RandomwarsMatch.Common
{
	public class RandomwarsMatchProtocol
	{
		public readonly ClientSession Session;
		public readonly string ServerAddr;
		public Dictionary<int, ControllerDelegate> MessageControllers { get;  private set; }

		public RandomwarsMatchProtocol()
		{
			Session = null;
			ServerAddr = string.Empty;

			Init();
		}

		public RandomwarsMatchProtocol(ClientSession session, string serverAddr)
		{
			Session = session;
			ServerAddr = serverAddr;

			Init();
		}

		void Init()
		{
			MessageControllers = new Dictionary<int, ControllerDelegate>();
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

			request.accessToken = Session.SessionKey;
			return Session.Send(MatchRequestRequest.ProtocolId,
				ServerAddr + "matchrequest-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchRequestResponse MatchRequestReqCallback(ClientSession session, MatchRequestRequest request);
		public MatchRequestReqCallback OnMatchRequestReqCallback;
		public ISerializer MatchRequestReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchRequestRequest request = MatchRequestRequest.JsonDeserialize(json);
			return OnMatchRequestReqCallback(session, request) as ISerializer;
		}

		public delegate bool MatchRequestResCallback(MatchRequestResponse response);
		public MatchRequestResCallback OnMatchRequestResCallback;
		public ISerializer MatchRequestResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchRequestResponse response = MatchRequestResponse.JsonDeserialize(json);
			OnMatchRequestResCallback(response);
			return null;
		}
		#endregion

		#region MatchStatus ------------------------------------------
		public bool MatchStatusReq(MatchStatusRequest request, MatchStatusResCallback callback)
		{
			OnMatchStatusResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(MatchStatusRequest.ProtocolId,
				ServerAddr + "matchstatus-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchStatusResponse MatchStatusReqCallback(ClientSession session, MatchStatusRequest request);
		public MatchStatusReqCallback OnMatchStatusReqCallback;
		public ISerializer MatchStatusReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchStatusRequest request = MatchStatusRequest.JsonDeserialize(json);
			return OnMatchStatusReqCallback(session, request) as ISerializer;
		}

		public delegate bool MatchStatusResCallback(MatchStatusResponse response);
		public MatchStatusResCallback OnMatchStatusResCallback;
		public ISerializer MatchStatusResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchStatusResponse response = MatchStatusResponse.JsonDeserialize(json);
			OnMatchStatusResCallback(response);
			return null;
		}
		#endregion

		#region MatchCancel ------------------------------------------
		public bool MatchCancelReq(MatchCancelRequest request, MatchCancelResCallback callback)
		{
			OnMatchCancelResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(MatchCancelRequest.ProtocolId,
				ServerAddr + "matchcancel-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchCancelResponse MatchCancelReqCallback(ClientSession session, MatchCancelRequest request);
		public MatchCancelReqCallback OnMatchCancelReqCallback;
		public ISerializer MatchCancelReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchCancelRequest request = MatchCancelRequest.JsonDeserialize(json);
			return OnMatchCancelReqCallback(session, request) as ISerializer;
		}

		public delegate bool MatchCancelResCallback(MatchCancelResponse response);
		public MatchCancelResCallback OnMatchCancelResCallback;
		public ISerializer MatchCancelResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchCancelResponse response = MatchCancelResponse.JsonDeserialize(json);
			OnMatchCancelResCallback(response);
			return null;
		}
		#endregion

		#region MatchInvite ------------------------------------------
		public bool MatchInviteReq(MatchInviteRequest request, MatchInviteResCallback callback)
		{
			OnMatchInviteResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(MatchInviteRequest.ProtocolId,
				ServerAddr + "matchinvite-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchInviteResponse MatchInviteReqCallback(ClientSession session, MatchInviteRequest request);
		public MatchInviteReqCallback OnMatchInviteReqCallback;
		public ISerializer MatchInviteReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchInviteRequest request = MatchInviteRequest.JsonDeserialize(json);
			return OnMatchInviteReqCallback(session, request) as ISerializer;
		}

		public delegate bool MatchInviteResCallback(MatchInviteResponse response);
		public MatchInviteResCallback OnMatchInviteResCallback;
		public ISerializer MatchInviteResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchInviteResponse response = MatchInviteResponse.JsonDeserialize(json);
			OnMatchInviteResCallback(response);
			return null;
		}
		#endregion

		#region MatchJoin ------------------------------------------
		public bool MatchJoinReq(MatchJoinRequest request, MatchJoinResCallback callback)
		{
			OnMatchJoinResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(MatchJoinRequest.ProtocolId,
				ServerAddr + "matchjoin-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MatchJoinResponse MatchJoinReqCallback(ClientSession session, MatchJoinRequest request);
		public MatchJoinReqCallback OnMatchJoinReqCallback;
		public ISerializer MatchJoinReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchJoinRequest request = MatchJoinRequest.JsonDeserialize(json);
			return OnMatchJoinReqCallback(session, request) as ISerializer;
		}

		public delegate bool MatchJoinResCallback(MatchJoinResponse response);
		public MatchJoinResCallback OnMatchJoinResCallback;
		public ISerializer MatchJoinResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MatchJoinResponse response = MatchJoinResponse.JsonDeserialize(json);
			OnMatchJoinResCallback(response);
			return null;
		}
		#endregion

	}
}
