using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Account.GameBaseAccount.Common
{
	public class GameBaseAccountProtocol
	{
		public readonly ClientSession Session;
		public readonly string ServerAddr;
		public Dictionary<int, ControllerDelegate> MessageControllers { get;  private set; }

		public GameBaseAccountProtocol()
		{
			Session = null;
			ServerAddr = string.Empty;

			Init();
		}

		public GameBaseAccountProtocol(ClientSession session, string serverAddr)
		{
			Session = session;
			ServerAddr = serverAddr;

			Init();
		}

		void Init()
		{
			MessageControllers = new Dictionary<int, ControllerDelegate>();
			MessageControllers.Add(AccountLoginRequest.ProtocolId, AccountLoginReqController);
			MessageControllers.Add(AccountLoginResponse.ProtocolId, AccountLoginResController);
			MessageControllers.Add(AccountPlatformLinkRequest.ProtocolId, AccountPlatformLinkReqController);
			MessageControllers.Add(AccountPlatformLinkResponse.ProtocolId, AccountPlatformLinkResController);
			MessageControllers.Add(AccountLogoutRequest.ProtocolId, AccountLogoutReqController);
			MessageControllers.Add(AccountLogoutResponse.ProtocolId, AccountLogoutResController);
		}

		#region AccountLogin ------------------------------------------
		public bool AccountLoginReq(AccountLoginRequest request, AccountLoginResCallback callback)
		{
			OnAccountLoginResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(AccountLoginRequest.ProtocolId,
				ServerAddr + "accountlogin-v1.0.0",
				request.JsonSerialize());
		}

		public delegate AccountLoginResponse AccountLoginReqCallback(ClientSession session, AccountLoginRequest request);
		public AccountLoginReqCallback OnAccountLoginReqCallback;
		public ISerializer AccountLoginReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountLoginRequest request = AccountLoginRequest.JsonDeserialize(json);
			return OnAccountLoginReqCallback(session, request) as ISerializer;
		}

		public delegate bool AccountLoginResCallback(AccountLoginResponse response);
		public AccountLoginResCallback OnAccountLoginResCallback;
		public ISerializer AccountLoginResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountLoginResponse response = AccountLoginResponse.JsonDeserialize(json);
			OnAccountLoginResCallback(response);
			return null;
		}
		#endregion

		#region AccountPlatformLink ------------------------------------------
		public bool AccountPlatformLinkReq(AccountPlatformLinkRequest request, AccountPlatformLinkResCallback callback)
		{
			OnAccountPlatformLinkResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(AccountPlatformLinkRequest.ProtocolId,
				ServerAddr + "accountplatformlink-v1.0.0",
				request.JsonSerialize());
		}

		public delegate AccountPlatformLinkResponse AccountPlatformLinkReqCallback(ClientSession session, AccountPlatformLinkRequest request);
		public AccountPlatformLinkReqCallback OnAccountPlatformLinkReqCallback;
		public ISerializer AccountPlatformLinkReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountPlatformLinkRequest request = AccountPlatformLinkRequest.JsonDeserialize(json);
			return OnAccountPlatformLinkReqCallback(session, request) as ISerializer;
		}

		public delegate bool AccountPlatformLinkResCallback(AccountPlatformLinkResponse response);
		public AccountPlatformLinkResCallback OnAccountPlatformLinkResCallback;
		public ISerializer AccountPlatformLinkResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountPlatformLinkResponse response = AccountPlatformLinkResponse.JsonDeserialize(json);
			OnAccountPlatformLinkResCallback(response);
			return null;
		}
		#endregion

		#region AccountLogout ------------------------------------------
		public bool AccountLogoutReq(AccountLogoutRequest request, AccountLogoutResCallback callback)
		{
			OnAccountLogoutResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(AccountLogoutRequest.ProtocolId,
				ServerAddr + "accountlogout-v1.0.0",
				request.JsonSerialize());
		}

		public delegate AccountLogoutResponse AccountLogoutReqCallback(ClientSession session, AccountLogoutRequest request);
		public AccountLogoutReqCallback OnAccountLogoutReqCallback;
		public ISerializer AccountLogoutReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountLogoutRequest request = AccountLogoutRequest.JsonDeserialize(json);
			return OnAccountLogoutReqCallback(session, request) as ISerializer;
		}

		public delegate bool AccountLogoutResCallback(AccountLogoutResponse response);
		public AccountLogoutResCallback OnAccountLogoutResCallback;
		public ISerializer AccountLogoutResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountLogoutResponse response = AccountLogoutResponse.JsonDeserialize(json);
			OnAccountLogoutResCallback(response);
			return null;
		}
		#endregion

	}
}
