using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Quest.GameBaseQuest.Common
{
	public class GameBaseQuestProtocol : ProtocolBase
	{
		public GameBaseQuestProtocol(ISender sender = null, string serverAddr = "")
			: base(sender, serverAddr)
		{
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
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(QuestInfoRequest.ProtocolId,
				_serverAddr + "questinfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate QuestInfoResponse QuestInfoReqCallback(QuestInfoRequest request);
		public QuestInfoReqCallback OnQuestInfoReqCallback;
		public ISerializer QuestInfoReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestInfoRequest request = new QuestInfoRequest();
			request.JsonDeserialize(json);
			return OnQuestInfoReqCallback(request) as ISerializer;
		}

		public delegate bool QuestInfoResCallback(QuestInfoResponse response);
		public QuestInfoResCallback OnQuestInfoResCallback;
		public ISerializer QuestInfoResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestInfoResponse response = new QuestInfoResponse();
			response.JsonDeserialize(json);
			OnQuestInfoResCallback(response);
			return null;
		}
		#endregion

		#region QuestReward ------------------------------------------
		public bool QuestRewardReq(QuestRewardRequest request, QuestRewardResCallback callback)
		{
			OnQuestRewardResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(QuestRewardRequest.ProtocolId,
				_serverAddr + "questreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate QuestRewardResponse QuestRewardReqCallback(QuestRewardRequest request);
		public QuestRewardReqCallback OnQuestRewardReqCallback;
		public ISerializer QuestRewardReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestRewardRequest request = new QuestRewardRequest();
			request.JsonDeserialize(json);
			return OnQuestRewardReqCallback(request) as ISerializer;
		}

		public delegate bool QuestRewardResCallback(QuestRewardResponse response);
		public QuestRewardResCallback OnQuestRewardResCallback;
		public ISerializer QuestRewardResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestRewardResponse response = new QuestRewardResponse();
			response.JsonDeserialize(json);
			OnQuestRewardResCallback(response);
			return null;
		}
		#endregion

		#region QuestDailyReward ------------------------------------------
		public bool QuestDailyRewardReq(QuestDailyRewardRequest request, QuestDailyRewardResCallback callback)
		{
			OnQuestDailyRewardResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(QuestDailyRewardRequest.ProtocolId,
				_serverAddr + "questdailyreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate QuestDailyRewardResponse QuestDailyRewardReqCallback(QuestDailyRewardRequest request);
		public QuestDailyRewardReqCallback OnQuestDailyRewardReqCallback;
		public ISerializer QuestDailyRewardReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestDailyRewardRequest request = new QuestDailyRewardRequest();
			request.JsonDeserialize(json);
			return OnQuestDailyRewardReqCallback(request) as ISerializer;
		}

		public delegate bool QuestDailyRewardResCallback(QuestDailyRewardResponse response);
		public QuestDailyRewardResCallback OnQuestDailyRewardResCallback;
		public ISerializer QuestDailyRewardResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			QuestDailyRewardResponse response = new QuestDailyRewardResponse();
			response.JsonDeserialize(json);
			OnQuestDailyRewardResCallback(response);
			return null;
		}
		#endregion

	}
}
