using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Account.GameBaseAccount.Common
{
	public class GameBaseAccountProtocol : ProtocolBase
	{
		public GameBaseAccountProtocol(ISender sender = null, string serverAddr = "")
			: base(sender, serverAddr)
		{
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
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(AccountLoginRequest.ProtocolId,
				_serverAddr + "accountlogin-v1.0.0",
				request.JsonSerialize());
		}

		public delegate AccountLoginResponse AccountLoginReqCallback(AccountLoginRequest request);
		public AccountLoginReqCallback OnAccountLoginReqCallback;
		public ISerializer AccountLoginReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountLoginRequest request = new AccountLoginRequest();
			request.JsonDeserialize(json);
			return OnAccountLoginReqCallback(request) as ISerializer;
		}

		public delegate bool AccountLoginResCallback(AccountLoginResponse response);
		public AccountLoginResCallback OnAccountLoginResCallback;
		public ISerializer AccountLoginResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountLoginResponse response = new AccountLoginResponse();
			response.JsonDeserialize(json);
			OnAccountLoginResCallback(response);
			return null;
		}
		#endregion

		#region AccountPlatformLink ------------------------------------------
		public bool AccountPlatformLinkReq(AccountPlatformLinkRequest request, AccountPlatformLinkResCallback callback)
		{
			OnAccountPlatformLinkResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(AccountPlatformLinkRequest.ProtocolId,
				_serverAddr + "accountplatformlink-v1.0.0",
				request.JsonSerialize());
		}

		public delegate AccountPlatformLinkResponse AccountPlatformLinkReqCallback(AccountPlatformLinkRequest request);
		public AccountPlatformLinkReqCallback OnAccountPlatformLinkReqCallback;
		public ISerializer AccountPlatformLinkReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountPlatformLinkRequest request = new AccountPlatformLinkRequest();
			request.JsonDeserialize(json);
			return OnAccountPlatformLinkReqCallback(request) as ISerializer;
		}

		public delegate bool AccountPlatformLinkResCallback(AccountPlatformLinkResponse response);
		public AccountPlatformLinkResCallback OnAccountPlatformLinkResCallback;
		public ISerializer AccountPlatformLinkResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountPlatformLinkResponse response = new AccountPlatformLinkResponse();
			response.JsonDeserialize(json);
			OnAccountPlatformLinkResCallback(response);
			return null;
		}
		#endregion

		#region AccountLogout ------------------------------------------
		public bool AccountLogoutReq(AccountLogoutRequest request, AccountLogoutResCallback callback)
		{
			OnAccountLogoutResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(AccountLogoutRequest.ProtocolId,
				_serverAddr + "accountlogout-v1.0.0",
				request.JsonSerialize());
		}

		public delegate AccountLogoutResponse AccountLogoutReqCallback(AccountLogoutRequest request);
		public AccountLogoutReqCallback OnAccountLogoutReqCallback;
		public ISerializer AccountLogoutReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountLogoutRequest request = new AccountLogoutRequest();
			request.JsonDeserialize(json);
			return OnAccountLogoutReqCallback(request) as ISerializer;
		}

		public delegate bool AccountLogoutResCallback(AccountLogoutResponse response);
		public AccountLogoutResCallback OnAccountLogoutResCallback;
		public ISerializer AccountLogoutResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			AccountLogoutResponse response = new AccountLogoutResponse();
			response.JsonDeserialize(json);
			OnAccountLogoutResCallback(response);
			return null;
		}
		#endregion

	}
}
