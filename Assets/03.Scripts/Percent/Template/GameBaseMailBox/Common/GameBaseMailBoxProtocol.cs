using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.MailBox.GameBaseMailBox.Common
{
	public class GameBaseMailBoxProtocol
	{
		public readonly ClientSession Session;
		public readonly string ServerAddr;
		public Dictionary<int, ControllerDelegate> MessageControllers { get;  private set; }

		public GameBaseMailBoxProtocol()
		{
			Session = null;
			ServerAddr = string.Empty;

			Init();
		}

		public GameBaseMailBoxProtocol(ClientSession session, string serverAddr)
		{
			Session = session;
			ServerAddr = serverAddr;

			Init();
		}

		void Init()
		{
			MessageControllers = new Dictionary<int, ControllerDelegate>();
			MessageControllers.Add(MailBoxInfoRequest.ProtocolId, MailBoxInfoReqController);
			MessageControllers.Add(MailBoxInfoResponse.ProtocolId, MailBoxInfoResController);
			MessageControllers.Add(MailBoxRefreshRequest.ProtocolId, MailBoxRefreshReqController);
			MessageControllers.Add(MailBoxRefreshResponse.ProtocolId, MailBoxRefreshResController);
			MessageControllers.Add(MailReceiveRequest.ProtocolId, MailReceiveReqController);
			MessageControllers.Add(MailReceiveResponse.ProtocolId, MailReceiveResController);
			MessageControllers.Add(MailReceiveAllRequest.ProtocolId, MailReceiveAllReqController);
			MessageControllers.Add(MailReceiveAllResponse.ProtocolId, MailReceiveAllResController);
			MessageControllers.Add(MailSendRequest.ProtocolId, MailSendReqController);
			MessageControllers.Add(MailSendResponse.ProtocolId, MailSendResController);
			MessageControllers.Add(SystemMailSendRequest.ProtocolId, SystemMailSendReqController);
			MessageControllers.Add(SystemMailSendResponse.ProtocolId, SystemMailSendResController);
		}

		#region MailBoxInfo ------------------------------------------
		public bool MailBoxInfoReq(MailBoxInfoRequest request, MailBoxInfoResCallback callback)
		{
			OnMailBoxInfoResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(MailBoxInfoRequest.ProtocolId,
				ServerAddr + "mailboxinfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailBoxInfoResponse MailBoxInfoReqCallback(ClientSession session, MailBoxInfoRequest request);
		public MailBoxInfoReqCallback OnMailBoxInfoReqCallback;
		public ISerializer MailBoxInfoReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailBoxInfoRequest request = MailBoxInfoRequest.JsonDeserialize(json);
			return OnMailBoxInfoReqCallback(session, request) as ISerializer;
		}

		public delegate bool MailBoxInfoResCallback(MailBoxInfoResponse response);
		public MailBoxInfoResCallback OnMailBoxInfoResCallback;
		public ISerializer MailBoxInfoResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailBoxInfoResponse response = MailBoxInfoResponse.JsonDeserialize(json);
			OnMailBoxInfoResCallback(response);
			return null;
		}
		#endregion

		#region MailBoxRefresh ------------------------------------------
		public bool MailBoxRefreshReq(MailBoxRefreshRequest request, MailBoxRefreshResCallback callback)
		{
			OnMailBoxRefreshResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(MailBoxRefreshRequest.ProtocolId,
				ServerAddr + "mailboxrefresh-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailBoxRefreshResponse MailBoxRefreshReqCallback(ClientSession session, MailBoxRefreshRequest request);
		public MailBoxRefreshReqCallback OnMailBoxRefreshReqCallback;
		public ISerializer MailBoxRefreshReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailBoxRefreshRequest request = MailBoxRefreshRequest.JsonDeserialize(json);
			return OnMailBoxRefreshReqCallback(session, request) as ISerializer;
		}

		public delegate bool MailBoxRefreshResCallback(MailBoxRefreshResponse response);
		public MailBoxRefreshResCallback OnMailBoxRefreshResCallback;
		public ISerializer MailBoxRefreshResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailBoxRefreshResponse response = MailBoxRefreshResponse.JsonDeserialize(json);
			OnMailBoxRefreshResCallback(response);
			return null;
		}
		#endregion

		#region MailReceive ------------------------------------------
		public bool MailReceiveReq(MailReceiveRequest request, MailReceiveResCallback callback)
		{
			OnMailReceiveResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(MailReceiveRequest.ProtocolId,
				ServerAddr + "mailreceive-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailReceiveResponse MailReceiveReqCallback(ClientSession session, MailReceiveRequest request);
		public MailReceiveReqCallback OnMailReceiveReqCallback;
		public ISerializer MailReceiveReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailReceiveRequest request = MailReceiveRequest.JsonDeserialize(json);
			return OnMailReceiveReqCallback(session, request) as ISerializer;
		}

		public delegate bool MailReceiveResCallback(MailReceiveResponse response);
		public MailReceiveResCallback OnMailReceiveResCallback;
		public ISerializer MailReceiveResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailReceiveResponse response = MailReceiveResponse.JsonDeserialize(json);
			OnMailReceiveResCallback(response);
			return null;
		}
		#endregion

		#region MailReceiveAll ------------------------------------------
		public bool MailReceiveAllReq(MailReceiveAllRequest request, MailReceiveAllResCallback callback)
		{
			OnMailReceiveAllResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(MailReceiveAllRequest.ProtocolId,
				ServerAddr + "mailreceiveall-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailReceiveAllResponse MailReceiveAllReqCallback(ClientSession session, MailReceiveAllRequest request);
		public MailReceiveAllReqCallback OnMailReceiveAllReqCallback;
		public ISerializer MailReceiveAllReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailReceiveAllRequest request = MailReceiveAllRequest.JsonDeserialize(json);
			return OnMailReceiveAllReqCallback(session, request) as ISerializer;
		}

		public delegate bool MailReceiveAllResCallback(MailReceiveAllResponse response);
		public MailReceiveAllResCallback OnMailReceiveAllResCallback;
		public ISerializer MailReceiveAllResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailReceiveAllResponse response = MailReceiveAllResponse.JsonDeserialize(json);
			OnMailReceiveAllResCallback(response);
			return null;
		}
		#endregion

		#region MailSend ------------------------------------------
		public bool MailSendReq(MailSendRequest request, MailSendResCallback callback)
		{
			OnMailSendResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(MailSendRequest.ProtocolId,
				ServerAddr + "mailsend-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailSendResponse MailSendReqCallback(ClientSession session, MailSendRequest request);
		public MailSendReqCallback OnMailSendReqCallback;
		public ISerializer MailSendReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailSendRequest request = MailSendRequest.JsonDeserialize(json);
			return OnMailSendReqCallback(session, request) as ISerializer;
		}

		public delegate bool MailSendResCallback(MailSendResponse response);
		public MailSendResCallback OnMailSendResCallback;
		public ISerializer MailSendResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailSendResponse response = MailSendResponse.JsonDeserialize(json);
			OnMailSendResCallback(response);
			return null;
		}
		#endregion

		#region SystemMailSend ------------------------------------------
		public bool SystemMailSendReq(SystemMailSendRequest request, SystemMailSendResCallback callback)
		{
			OnSystemMailSendResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(SystemMailSendRequest.ProtocolId,
				ServerAddr + "systemmailsend-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SystemMailSendResponse SystemMailSendReqCallback(ClientSession session, SystemMailSendRequest request);
		public SystemMailSendReqCallback OnSystemMailSendReqCallback;
		public ISerializer SystemMailSendReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SystemMailSendRequest request = SystemMailSendRequest.JsonDeserialize(json);
			return OnSystemMailSendReqCallback(session, request) as ISerializer;
		}

		public delegate bool SystemMailSendResCallback(SystemMailSendResponse response);
		public SystemMailSendResCallback OnSystemMailSendResCallback;
		public ISerializer SystemMailSendResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SystemMailSendResponse response = SystemMailSendResponse.JsonDeserialize(json);
			OnSystemMailSendResCallback(response);
			return null;
		}
		#endregion

	}
}
