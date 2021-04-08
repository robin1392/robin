using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Quest.GameBaseQuest.Common
{
	public class GameBaseQuestProtocol
	{
		public readonly ClientSession Session;
		public readonly string ServerAddr;
		public Dictionary<int, ControllerDelegate> MessageControllers { get;  private set; }

		public GameBaseQuestProtocol()
		{
			Session = null;
			ServerAddr = string.Empty;

			Init();
		}

		public GameBaseQuestProtocol(ClientSession session, string serverAddr)
		{
			Session = session;
			ServerAddr = serverAddr;

			Init();
		}

		void Init()
		{
			MessageControllers = new Dictionary<int, ControllerDelegate>();
			MessageControllers.Add(QuestInfoRequest.ProtocolId, QuestInfoReqController);
			MessageControllers.Add(QuestInfoResponse.ProtocolId, QuestInfoResController);
			MessageControllers.Add(QuestRewardRequest.ProtocolId, QuestRewardReqController);
			MessageControllers.Add(QuestRewardResponse.ProtocolId, QuestRewardResController);
			MessageControllers.Add(QuestDailyRewardRequest.ProtocolId, QuestDailyRewardReqController);
			MessageControllers.Add(QuestDailyRewardResponse.ProtocolId, QuestDailyRewardResController);
		}

		#region QuestInfo ------------------------------------------
		public bool QuestInfoReq(QuestInfoRequest request, QuestInfoResCallback callback)
		{
			OnQuestInfoResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(QuestInfoRequest.ProtocolId,
				ServerAddr + "questinfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate QuestInfoResponse QuestInfoReqCallback(ClientSession session, QuestInfoRequest request);
		public QuestInfoReqCallback OnQuestInfoReqCallback;
		public ISerializer QuestInfoReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestInfoRequest request = QuestInfoRequest.JsonDeserialize(json);
			return OnQuestInfoReqCallback(session, request) as ISerializer;
		}

		public delegate bool QuestInfoResCallback(QuestInfoResponse response);
		public QuestInfoResCallback OnQuestInfoResCallback;
		public ISerializer QuestInfoResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestInfoResponse response = QuestInfoResponse.JsonDeserialize(json);
			OnQuestInfoResCallback(response);
			return null;
		}
		#endregion

		#region QuestReward ------------------------------------------
		public bool QuestRewardReq(QuestRewardRequest request, QuestRewardResCallback callback)
		{
			OnQuestRewardResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(QuestRewardRequest.ProtocolId,
				ServerAddr + "questreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate QuestRewardResponse QuestRewardReqCallback(ClientSession session, QuestRewardRequest request);
		public QuestRewardReqCallback OnQuestRewardReqCallback;
		public ISerializer QuestRewardReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestRewardRequest request = QuestRewardRequest.JsonDeserialize(json);
			return OnQuestRewardReqCallback(session, request) as ISerializer;
		}

		public delegate bool QuestRewardResCallback(QuestRewardResponse response);
		public QuestRewardResCallback OnQuestRewardResCallback;
		public ISerializer QuestRewardResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestRewardResponse response = QuestRewardResponse.JsonDeserialize(json);
			OnQuestRewardResCallback(response);
			return null;
		}
		#endregion

		#region QuestDailyReward ------------------------------------------
		public bool QuestDailyRewardReq(QuestDailyRewardRequest request, QuestDailyRewardResCallback callback)
		{
			OnQuestDailyRewardResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(QuestDailyRewardRequest.ProtocolId,
				ServerAddr + "questdailyreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate QuestDailyRewardResponse QuestDailyRewardReqCallback(ClientSession session, QuestDailyRewardRequest request);
		public QuestDailyRewardReqCallback OnQuestDailyRewardReqCallback;
		public ISerializer QuestDailyRewardReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestDailyRewardRequest request = QuestDailyRewardRequest.JsonDeserialize(json);
			return OnQuestDailyRewardReqCallback(session, request) as ISerializer;
		}

		public delegate bool QuestDailyRewardResCallback(QuestDailyRewardResponse response);
		public QuestDailyRewardResCallback OnQuestDailyRewardResCallback;
		public ISerializer QuestDailyRewardResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestDailyRewardResponse response = QuestDailyRewardResponse.JsonDeserialize(json);
			OnQuestDailyRewardResCallback(response);
			return null;
		}
		#endregion

	}
}
