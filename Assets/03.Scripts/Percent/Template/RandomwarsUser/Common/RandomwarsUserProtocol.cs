using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.User.RandomwarsUser.Common
{
	public class RandomwarsUserProtocol
	{
		public readonly ClientSession Session;
		public readonly string ServerAddr;
		public Dictionary<int, ControllerDelegate> MessageControllers { get;  private set; }

		public RandomwarsUserProtocol()
		{
			Session = null;
			ServerAddr = string.Empty;

			Init();
		}

		public RandomwarsUserProtocol(ClientSession session, string serverAddr)
		{
			Session = session;
			ServerAddr = serverAddr;

			Init();
		}

		void Init()
		{
			MessageControllers = new Dictionary<int, ControllerDelegate>();
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

			request.accessToken = Session.SessionKey;
			return Session.Send(UserInfoRequest.ProtocolId,
				ServerAddr + "userinfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserInfoResponse UserInfoReqCallback(ClientSession session, UserInfoRequest request);
		public UserInfoReqCallback OnUserInfoReqCallback;
		public ISerializer UserInfoReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserInfoRequest request = UserInfoRequest.JsonDeserialize(json);
			return OnUserInfoReqCallback(session, request) as ISerializer;
		}

		public delegate bool UserInfoResCallback(UserInfoResponse response);
		public UserInfoResCallback OnUserInfoResCallback;
		public ISerializer UserInfoResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserInfoResponse response = UserInfoResponse.JsonDeserialize(json);
			OnUserInfoResCallback(response);
			return null;
		}
		#endregion

		#region UserTutorialEnd ------------------------------------------
		public bool UserTutorialEndReq(UserTutorialEndRequest request, UserTutorialEndResCallback callback)
		{
			OnUserTutorialEndResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(UserTutorialEndRequest.ProtocolId,
				ServerAddr + "usertutorialend-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserTutorialEndResponse UserTutorialEndReqCallback(ClientSession session, UserTutorialEndRequest request);
		public UserTutorialEndReqCallback OnUserTutorialEndReqCallback;
		public ISerializer UserTutorialEndReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserTutorialEndRequest request = UserTutorialEndRequest.JsonDeserialize(json);
			return OnUserTutorialEndReqCallback(session, request) as ISerializer;
		}

		public delegate bool UserTutorialEndResCallback(UserTutorialEndResponse response);
		public UserTutorialEndResCallback OnUserTutorialEndResCallback;
		public ISerializer UserTutorialEndResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserTutorialEndResponse response = UserTutorialEndResponse.JsonDeserialize(json);
			OnUserTutorialEndResCallback(response);
			return null;
		}
		#endregion

		#region UserNameInit ------------------------------------------
		public bool UserNameInitReq(UserNameInitRequest request, UserNameInitResCallback callback)
		{
			OnUserNameInitResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(UserNameInitRequest.ProtocolId,
				ServerAddr + "usernameinit-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserNameInitResponse UserNameInitReqCallback(ClientSession session, UserNameInitRequest request);
		public UserNameInitReqCallback OnUserNameInitReqCallback;
		public ISerializer UserNameInitReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserNameInitRequest request = UserNameInitRequest.JsonDeserialize(json);
			return OnUserNameInitReqCallback(session, request) as ISerializer;
		}

		public delegate bool UserNameInitResCallback(UserNameInitResponse response);
		public UserNameInitResCallback OnUserNameInitResCallback;
		public ISerializer UserNameInitResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserNameInitResponse response = UserNameInitResponse.JsonDeserialize(json);
			OnUserNameInitResCallback(response);
			return null;
		}
		#endregion

		#region UserNameChange ------------------------------------------
		public bool UserNameChangeReq(UserNameChangeRequest request, UserNameChangeResCallback callback)
		{
			OnUserNameChangeResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(UserNameChangeRequest.ProtocolId,
				ServerAddr + "usernamechange-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserNameChangeResponse UserNameChangeReqCallback(ClientSession session, UserNameChangeRequest request);
		public UserNameChangeReqCallback OnUserNameChangeReqCallback;
		public ISerializer UserNameChangeReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserNameChangeRequest request = UserNameChangeRequest.JsonDeserialize(json);
			return OnUserNameChangeReqCallback(session, request) as ISerializer;
		}

		public delegate bool UserNameChangeResCallback(UserNameChangeResponse response);
		public UserNameChangeResCallback OnUserNameChangeResCallback;
		public ISerializer UserNameChangeResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserNameChangeResponse response = UserNameChangeResponse.JsonDeserialize(json);
			OnUserNameChangeResCallback(response);
			return null;
		}
		#endregion

		#region UserTrophyReward ------------------------------------------
		public bool UserTrophyRewardReq(UserTrophyRewardRequest request, UserTrophyRewardResCallback callback)
		{
			OnUserTrophyRewardResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(UserTrophyRewardRequest.ProtocolId,
				ServerAddr + "usertrophyreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserTrophyRewardResponse UserTrophyRewardReqCallback(ClientSession session, UserTrophyRewardRequest request);
		public UserTrophyRewardReqCallback OnUserTrophyRewardReqCallback;
		public ISerializer UserTrophyRewardReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserTrophyRewardRequest request = UserTrophyRewardRequest.JsonDeserialize(json);
			return OnUserTrophyRewardReqCallback(session, request) as ISerializer;
		}

		public delegate bool UserTrophyRewardResCallback(UserTrophyRewardResponse response);
		public UserTrophyRewardResCallback OnUserTrophyRewardResCallback;
		public ISerializer UserTrophyRewardResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserTrophyRewardResponse response = UserTrophyRewardResponse.JsonDeserialize(json);
			OnUserTrophyRewardResCallback(response);
			return null;
		}
		#endregion

		#region UserAdReward ------------------------------------------
		public bool UserAdRewardReq(UserAdRewardRequest request, UserAdRewardResCallback callback)
		{
			OnUserAdRewardResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(UserAdRewardRequest.ProtocolId,
				ServerAddr + "useradreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate UserAdRewardResponse UserAdRewardReqCallback(ClientSession session, UserAdRewardRequest request);
		public UserAdRewardReqCallback OnUserAdRewardReqCallback;
		public ISerializer UserAdRewardReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserAdRewardRequest request = UserAdRewardRequest.JsonDeserialize(json);
			return OnUserAdRewardReqCallback(session, request) as ISerializer;
		}

		public delegate bool UserAdRewardResCallback(UserAdRewardResponse response);
		public UserAdRewardResCallback OnUserAdRewardResCallback;
		public ISerializer UserAdRewardResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			UserAdRewardResponse response = UserAdRewardResponse.JsonDeserialize(json);
			OnUserAdRewardResCallback(response);
			return null;
		}
		#endregion

	}
}
