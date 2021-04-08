using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Season.GameBaseSeason.Common
{
	public class GameBaseSeasonProtocol
	{
		public readonly ClientSession Session;
		public readonly string ServerAddr;
		public Dictionary<int, ControllerDelegate> MessageControllers { get;  private set; }

		public GameBaseSeasonProtocol()
		{
			Session = null;
			ServerAddr = string.Empty;

			Init();
		}

		public GameBaseSeasonProtocol(ClientSession session, string serverAddr)
		{
			Session = session;
			ServerAddr = serverAddr;

			Init();
		}

		void Init()
		{
			MessageControllers = new Dictionary<int, ControllerDelegate>();
			MessageControllers.Add(SeasonInfoRequest.ProtocolId, SeasonInfoReqController);
			MessageControllers.Add(SeasonInfoResponse.ProtocolId, SeasonInfoResController);
			MessageControllers.Add(SeasonResetRequest.ProtocolId, SeasonResetReqController);
			MessageControllers.Add(SeasonResetResponse.ProtocolId, SeasonResetResController);
			MessageControllers.Add(SeasonRankRequest.ProtocolId, SeasonRankReqController);
			MessageControllers.Add(SeasonRankResponse.ProtocolId, SeasonRankResController);
			MessageControllers.Add(SeasonPassRewardRequest.ProtocolId, SeasonPassRewardReqController);
			MessageControllers.Add(SeasonPassRewardResponse.ProtocolId, SeasonPassRewardResController);
			MessageControllers.Add(SeasonPassStepRequest.ProtocolId, SeasonPassStepReqController);
			MessageControllers.Add(SeasonPassStepResponse.ProtocolId, SeasonPassStepResController);
		}

		#region SeasonInfo ------------------------------------------
		public bool SeasonInfoReq(SeasonInfoRequest request, SeasonInfoResCallback callback)
		{
			OnSeasonInfoResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(SeasonInfoRequest.ProtocolId,
				ServerAddr + "seasoninfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonInfoResponse SeasonInfoReqCallback(ClientSession session, SeasonInfoRequest request);
		public SeasonInfoReqCallback OnSeasonInfoReqCallback;
		public ISerializer SeasonInfoReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonInfoRequest request = SeasonInfoRequest.JsonDeserialize(json);
			return OnSeasonInfoReqCallback(session, request) as ISerializer;
		}

		public delegate bool SeasonInfoResCallback(SeasonInfoResponse response);
		public SeasonInfoResCallback OnSeasonInfoResCallback;
		public ISerializer SeasonInfoResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonInfoResponse response = SeasonInfoResponse.JsonDeserialize(json);
			OnSeasonInfoResCallback(response);
			return null;
		}
		#endregion

		#region SeasonReset ------------------------------------------
		public bool SeasonResetReq(SeasonResetRequest request, SeasonResetResCallback callback)
		{
			OnSeasonResetResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(SeasonResetRequest.ProtocolId,
				ServerAddr + "seasonreset-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonResetResponse SeasonResetReqCallback(ClientSession session, SeasonResetRequest request);
		public SeasonResetReqCallback OnSeasonResetReqCallback;
		public ISerializer SeasonResetReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonResetRequest request = SeasonResetRequest.JsonDeserialize(json);
			return OnSeasonResetReqCallback(session, request) as ISerializer;
		}

		public delegate bool SeasonResetResCallback(SeasonResetResponse response);
		public SeasonResetResCallback OnSeasonResetResCallback;
		public ISerializer SeasonResetResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonResetResponse response = SeasonResetResponse.JsonDeserialize(json);
			OnSeasonResetResCallback(response);
			return null;
		}
		#endregion

		#region SeasonRank ------------------------------------------
		public bool SeasonRankReq(SeasonRankRequest request, SeasonRankResCallback callback)
		{
			OnSeasonRankResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(SeasonRankRequest.ProtocolId,
				ServerAddr + "seasonrank-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonRankResponse SeasonRankReqCallback(ClientSession session, SeasonRankRequest request);
		public SeasonRankReqCallback OnSeasonRankReqCallback;
		public ISerializer SeasonRankReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonRankRequest request = SeasonRankRequest.JsonDeserialize(json);
			return OnSeasonRankReqCallback(session, request) as ISerializer;
		}

		public delegate bool SeasonRankResCallback(SeasonRankResponse response);
		public SeasonRankResCallback OnSeasonRankResCallback;
		public ISerializer SeasonRankResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonRankResponse response = SeasonRankResponse.JsonDeserialize(json);
			OnSeasonRankResCallback(response);
			return null;
		}
		#endregion

		#region SeasonPassReward ------------------------------------------
		public bool SeasonPassRewardReq(SeasonPassRewardRequest request, SeasonPassRewardResCallback callback)
		{
			OnSeasonPassRewardResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(SeasonPassRewardRequest.ProtocolId,
				ServerAddr + "seasonpassreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonPassRewardResponse SeasonPassRewardReqCallback(ClientSession session, SeasonPassRewardRequest request);
		public SeasonPassRewardReqCallback OnSeasonPassRewardReqCallback;
		public ISerializer SeasonPassRewardReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonPassRewardRequest request = SeasonPassRewardRequest.JsonDeserialize(json);
			return OnSeasonPassRewardReqCallback(session, request) as ISerializer;
		}

		public delegate bool SeasonPassRewardResCallback(SeasonPassRewardResponse response);
		public SeasonPassRewardResCallback OnSeasonPassRewardResCallback;
		public ISerializer SeasonPassRewardResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonPassRewardResponse response = SeasonPassRewardResponse.JsonDeserialize(json);
			OnSeasonPassRewardResCallback(response);
			return null;
		}
		#endregion

		#region SeasonPassStep ------------------------------------------
		public bool SeasonPassStepReq(SeasonPassStepRequest request, SeasonPassStepResCallback callback)
		{
			OnSeasonPassStepResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(SeasonPassStepRequest.ProtocolId,
				ServerAddr + "seasonpassstep-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonPassStepResponse SeasonPassStepReqCallback(ClientSession session, SeasonPassStepRequest request);
		public SeasonPassStepReqCallback OnSeasonPassStepReqCallback;
		public ISerializer SeasonPassStepReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonPassStepRequest request = SeasonPassStepRequest.JsonDeserialize(json);
			return OnSeasonPassStepReqCallback(session, request) as ISerializer;
		}

		public delegate bool SeasonPassStepResCallback(SeasonPassStepResponse response);
		public SeasonPassStepResCallback OnSeasonPassStepResCallback;
		public ISerializer SeasonPassStepResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonPassStepResponse response = SeasonPassStepResponse.JsonDeserialize(json);
			OnSeasonPassStepResCallback(response);
			return null;
		}
		#endregion

	}
}
