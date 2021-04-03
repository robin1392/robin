using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.MailBox.GameBaseMailBox.Common
{
	public class GameBaseMailBoxProtocol : ProtocolBase
	{
		public GameBaseMailBoxProtocol(ISender sender = null, string serverAddr = "")
			: base(sender, serverAddr)
		{
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
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MailBoxInfoRequest.ProtocolId,
				_serverAddr + "mailboxinfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailBoxInfoResponse MailBoxInfoReqCallback(MailBoxInfoRequest request);
		public MailBoxInfoReqCallback OnMailBoxInfoReqCallback;
		public ISerializer MailBoxInfoReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailBoxInfoRequest request = new MailBoxInfoRequest();
			request.JsonDeserialize(json);
			return OnMailBoxInfoReqCallback(request) as ISerializer;
		}

		public delegate bool MailBoxInfoResCallback(MailBoxInfoResponse response);
		public MailBoxInfoResCallback OnMailBoxInfoResCallback;
		public ISerializer MailBoxInfoResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailBoxInfoResponse response = new MailBoxInfoResponse();
			response.JsonDeserialize(json);
			OnMailBoxInfoResCallback(response);
			return null;
		}
		#endregion

		#region MailBoxRefresh ------------------------------------------
		public bool MailBoxRefreshReq(MailBoxRefreshRequest request, MailBoxRefreshResCallback callback)
		{
			OnMailBoxRefreshResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MailBoxRefreshRequest.ProtocolId,
				_serverAddr + "mailboxrefresh-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailBoxRefreshResponse MailBoxRefreshReqCallback(MailBoxRefreshRequest request);
		public MailBoxRefreshReqCallback OnMailBoxRefreshReqCallback;
		public ISerializer MailBoxRefreshReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailBoxRefreshRequest request = new MailBoxRefreshRequest();
			request.JsonDeserialize(json);
			return OnMailBoxRefreshReqCallback(request) as ISerializer;
		}

		public delegate bool MailBoxRefreshResCallback(MailBoxRefreshResponse response);
		public MailBoxRefreshResCallback OnMailBoxRefreshResCallback;
		public ISerializer MailBoxRefreshResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailBoxRefreshResponse response = new MailBoxRefreshResponse();
			response.JsonDeserialize(json);
			OnMailBoxRefreshResCallback(response);
			return null;
		}
		#endregion

		#region MailReceive ------------------------------------------
		public bool MailReceiveReq(MailReceiveRequest request, MailReceiveResCallback callback)
		{
			OnMailReceiveResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MailReceiveRequest.ProtocolId,
				_serverAddr + "mailreceive-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailReceiveResponse MailReceiveReqCallback(MailReceiveRequest request);
		public MailReceiveReqCallback OnMailReceiveReqCallback;
		public ISerializer MailReceiveReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailReceiveRequest request = new MailReceiveRequest();
			request.JsonDeserialize(json);
			return OnMailReceiveReqCallback(request) as ISerializer;
		}

		public delegate bool MailReceiveResCallback(MailReceiveResponse response);
		public MailReceiveResCallback OnMailReceiveResCallback;
		public ISerializer MailReceiveResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailReceiveResponse response = new MailReceiveResponse();
			response.JsonDeserialize(json);
			OnMailReceiveResCallback(response);
			return null;
		}
		#endregion

		#region MailReceiveAll ------------------------------------------
		public bool MailReceiveAllReq(MailReceiveAllRequest request, MailReceiveAllResCallback callback)
		{
			OnMailReceiveAllResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MailReceiveAllRequest.ProtocolId,
				_serverAddr + "mailreceiveall-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailReceiveAllResponse MailReceiveAllReqCallback(MailReceiveAllRequest request);
		public MailReceiveAllReqCallback OnMailReceiveAllReqCallback;
		public ISerializer MailReceiveAllReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailReceiveAllRequest request = new MailReceiveAllRequest();
			request.JsonDeserialize(json);
			return OnMailReceiveAllReqCallback(request) as ISerializer;
		}

		public delegate bool MailReceiveAllResCallback(MailReceiveAllResponse response);
		public MailReceiveAllResCallback OnMailReceiveAllResCallback;
		public ISerializer MailReceiveAllResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailReceiveAllResponse response = new MailReceiveAllResponse();
			response.JsonDeserialize(json);
			OnMailReceiveAllResCallback(response);
			return null;
		}
		#endregion

		#region MailSend ------------------------------------------
		public bool MailSendReq(MailSendRequest request, MailSendResCallback callback)
		{
			OnMailSendResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(MailSendRequest.ProtocolId,
				_serverAddr + "mailsend-v1.0.0",
				request.JsonSerialize());
		}

		public delegate MailSendResponse MailSendReqCallback(MailSendRequest request);
		public MailSendReqCallback OnMailSendReqCallback;
		public ISerializer MailSendReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailSendRequest request = new MailSendRequest();
			request.JsonDeserialize(json);
			return OnMailSendReqCallback(request) as ISerializer;
		}

		public delegate bool MailSendResCallback(MailSendResponse response);
		public MailSendResCallback OnMailSendResCallback;
		public ISerializer MailSendResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			MailSendResponse response = new MailSendResponse();
			response.JsonDeserialize(json);
			OnMailSendResCallback(response);
			return null;
		}
		#endregion

		#region SystemMailSend ------------------------------------------
		public bool SystemMailSendReq(SystemMailSendRequest request, SystemMailSendResCallback callback)
		{
			OnSystemMailSendResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(SystemMailSendRequest.ProtocolId,
				_serverAddr + "systemmailsend-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SystemMailSendResponse SystemMailSendReqCallback(SystemMailSendRequest request);
		public SystemMailSendReqCallback OnSystemMailSendReqCallback;
		public ISerializer SystemMailSendReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SystemMailSendRequest request = new SystemMailSendRequest();
			request.JsonDeserialize(json);
			return OnSystemMailSendReqCallback(request) as ISerializer;
		}

		public delegate bool SystemMailSendResCallback(SystemMailSendResponse response);
		public SystemMailSendResCallback OnSystemMailSendResCallback;
		public ISerializer SystemMailSendResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SystemMailSendResponse response = new SystemMailSendResponse();
			response.JsonDeserialize(json);
			OnSystemMailSendResCallback(response);
			return null;
		}
		#endregion

	}
}
