using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.User.RandomwarsUser.Common
{
	public class RandomwarsUserProtocol : ProtocolBase
	{
		public RandomwarsUserProtocol(ISender sender = null, string serverAddr = "")
			: base(sender, serverAddr)
		{
			MessageControllers.Add(UserInfoRequest.ProtocolId, UserInfoReqController);
			MessageControllers.Add(UserInfoResponse.ProtocolId, UserInfoResController);
			MessageControllers.Add(UserTutorialEndRequest.ProtocolId, UserTutorialEndReqController);
			MessageControllers.Add(UserTutorialEndResponse.ProtocolId, UserTutorialEndResController);
			MessageControllers.Add(UserNameInitRequest.ProtocolId, UserNameInitReqController);
			MessageControllers.Add(UserNameInitResponse.ProtocolId, UserNameInitResController);
			MessageControllers.Add(UserNameChangeRequest.ProtocolId, UserNameChangeReqController);
			MessageControllers.Add(UserNameChangeResponse.ProtocolId, UserNameChangeResController);
			MessageControllers.Add(UserTrophyRewardRequest.ProtocolId, UserTrophyRewardReqController);
			MessageControllers.Add(UserTrophyRewardResponse.ProtocolId, UserTrophyRewardResController);
			MessageControllers.Add(UserAdRewardRequest.ProtocolId, UserAdRewardReqController);
			MessageControllers.Add(UserAdRewardResponse.ProtocolId, UserAdRewardResController);
		}

		#region UserInfo ------------------------------------------
		public bool UserInfoReq(UserInfoRequest request, UserInfoResCallback callback)
		{
			OnUserInfoResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(UserInfoRequest.ProtocolId,
				_serverAddr + "userinfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserInfoResponse UserInfoReqCallback(UserInfoRequest request);
		public UserInfoReqCallback OnUserInfoReqCallback;
		public ISerializer UserInfoReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserInfoRequest request = new UserInfoRequest();
			request.JsonDeserialize(json);
			return OnUserInfoReqCallback(request) as ISerializer;
		}

		public delegate bool UserInfoResCallback(UserInfoResponse response);
		public UserInfoResCallback OnUserInfoResCallback;
		public ISerializer UserInfoResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserInfoResponse response = new UserInfoResponse();
			response.JsonDeserialize(json);
			OnUserInfoResCallback(response);
			return null;
		}
		#endregion

		#region UserTutorialEnd ------------------------------------------
		public bool UserTutorialEndReq(UserTutorialEndRequest request, UserTutorialEndResCallback callback)
		{
			OnUserTutorialEndResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(UserTutorialEndRequest.ProtocolId,
				_serverAddr + "usertutorialend-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserTutorialEndResponse UserTutorialEndReqCallback(UserTutorialEndRequest request);
		public UserTutorialEndReqCallback OnUserTutorialEndReqCallback;
		public ISerializer UserTutorialEndReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserTutorialEndRequest request = new UserTutorialEndRequest();
			request.JsonDeserialize(json);
			return OnUserTutorialEndReqCallback(request) as ISerializer;
		}

		public delegate bool UserTutorialEndResCallback(UserTutorialEndResponse response);
		public UserTutorialEndResCallback OnUserTutorialEndResCallback;
		public ISerializer UserTutorialEndResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserTutorialEndResponse response = new UserTutorialEndResponse();
			response.JsonDeserialize(json);
			OnUserTutorialEndResCallback(response);
			return null;
		}
		#endregion

		#region UserNameInit ------------------------------------------
		public bool UserNameInitReq(UserNameInitRequest request, UserNameInitResCallback callback)
		{
			OnUserNameInitResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(UserNameInitRequest.ProtocolId,
				_serverAddr + "usernameinit-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserNameInitResponse UserNameInitReqCallback(UserNameInitRequest request);
		public UserNameInitReqCallback OnUserNameInitReqCallback;
		public ISerializer UserNameInitReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserNameInitRequest request = new UserNameInitRequest();
			request.JsonDeserialize(json);
			return OnUserNameInitReqCallback(request) as ISerializer;
		}

		public delegate bool UserNameInitResCallback(UserNameInitResponse response);
		public UserNameInitResCallback OnUserNameInitResCallback;
		public ISerializer UserNameInitResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserNameInitResponse response = new UserNameInitResponse();
			response.JsonDeserialize(json);
			OnUserNameInitResCallback(response);
			return null;
		}
		#endregion

		#region UserNameChange ------------------------------------------
		public bool UserNameChangeReq(UserNameChangeRequest request, UserNameChangeResCallback callback)
		{
			OnUserNameChangeResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(UserNameChangeRequest.ProtocolId,
				_serverAddr + "usernamechange-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserNameChangeResponse UserNameChangeReqCallback(UserNameChangeRequest request);
		public UserNameChangeReqCallback OnUserNameChangeReqCallback;
		public ISerializer UserNameChangeReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserNameChangeRequest request = new UserNameChangeRequest();
			request.JsonDeserialize(json);
			return OnUserNameChangeReqCallback(request) as ISerializer;
		}

		public delegate bool UserNameChangeResCallback(UserNameChangeResponse response);
		public UserNameChangeResCallback OnUserNameChangeResCallback;
		public ISerializer UserNameChangeResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserNameChangeResponse response = new UserNameChangeResponse();
			response.JsonDeserialize(json);
			OnUserNameChangeResCallback(response);
			return null;
		}
		#endregion

		#region UserTrophyReward ------------------------------------------
		public bool UserTrophyRewardReq(UserTrophyRewardRequest request, UserTrophyRewardResCallback callback)
		{
			OnUserTrophyRewardResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(UserTrophyRewardRequest.ProtocolId,
				_serverAddr + "usertrophyreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserTrophyRewardResponse UserTrophyRewardReqCallback(UserTrophyRewardRequest request);
		public UserTrophyRewardReqCallback OnUserTrophyRewardReqCallback;
		public ISerializer UserTrophyRewardReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserTrophyRewardRequest request = new UserTrophyRewardRequest();
			request.JsonDeserialize(json);
			return OnUserTrophyRewardReqCallback(request) as ISerializer;
		}

		public delegate bool UserTrophyRewardResCallback(UserTrophyRewardResponse response);
		public UserTrophyRewardResCallback OnUserTrophyRewardResCallback;
		public ISerializer UserTrophyRewardResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserTrophyRewardResponse response = new UserTrophyRewardResponse();
			response.JsonDeserialize(json);
			OnUserTrophyRewardResCallback(response);
			return null;
		}
		#endregion

		#region UserAdReward ------------------------------------------
		public bool UserAdRewardReq(UserAdRewardRequest request, UserAdRewardResCallback callback)
		{
			OnUserAdRewardResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(UserAdRewardRequest.ProtocolId,
				_serverAddr + "useradreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserAdRewardResponse UserAdRewardReqCallback(UserAdRewardRequest request);
		public UserAdRewardReqCallback OnUserAdRewardReqCallback;
		public ISerializer UserAdRewardReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserAdRewardRequest request = new UserAdRewardRequest();
			request.JsonDeserialize(json);
			return OnUserAdRewardReqCallback(request) as ISerializer;
		}

		public delegate bool UserAdRewardResCallback(UserAdRewardResponse response);
		public UserAdRewardResCallback OnUserAdRewardResCallback;
		public ISerializer UserAdRewardResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserAdRewardResponse response = new UserAdRewardResponse();
			response.JsonDeserialize(json);
			OnUserAdRewardResCallback(response);
			return null;
		}
		#endregion

	}
}
