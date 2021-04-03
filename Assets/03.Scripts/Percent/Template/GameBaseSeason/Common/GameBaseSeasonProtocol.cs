using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Season.GameBaseSeason.Common
{
	public class GameBaseSeasonProtocol : ProtocolBase
	{
		public GameBaseSeasonProtocol(ISender sender = null, string serverAddr = "")
			: base(sender, serverAddr)
		{
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
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(SeasonInfoRequest.ProtocolId,
				_serverAddr + "seasoninfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonInfoResponse SeasonInfoReqCallback(SeasonInfoRequest request);
		public SeasonInfoReqCallback OnSeasonInfoReqCallback;
		public ISerializer SeasonInfoReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonInfoRequest request = new SeasonInfoRequest();
			request.JsonDeserialize(json);
			return OnSeasonInfoReqCallback(request) as ISerializer;
		}

		public delegate bool SeasonInfoResCallback(SeasonInfoResponse response);
		public SeasonInfoResCallback OnSeasonInfoResCallback;
		public ISerializer SeasonInfoResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonInfoResponse response = new SeasonInfoResponse();
			response.JsonDeserialize(json);
			OnSeasonInfoResCallback(response);
			return null;
		}
		#endregion

		#region SeasonReset ------------------------------------------
		public bool SeasonResetReq(SeasonResetRequest request, SeasonResetResCallback callback)
		{
			OnSeasonResetResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(SeasonResetRequest.ProtocolId,
				_serverAddr + "seasonreset-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonResetResponse SeasonResetReqCallback(SeasonResetRequest request);
		public SeasonResetReqCallback OnSeasonResetReqCallback;
		public ISerializer SeasonResetReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonResetRequest request = new SeasonResetRequest();
			request.JsonDeserialize(json);
			return OnSeasonResetReqCallback(request) as ISerializer;
		}

		public delegate bool SeasonResetResCallback(SeasonResetResponse response);
		public SeasonResetResCallback OnSeasonResetResCallback;
		public ISerializer SeasonResetResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonResetResponse response = new SeasonResetResponse();
			response.JsonDeserialize(json);
			OnSeasonResetResCallback(response);
			return null;
		}
		#endregion

		#region SeasonRank ------------------------------------------
		public bool SeasonRankReq(SeasonRankRequest request, SeasonRankResCallback callback)
		{
			OnSeasonRankResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(SeasonRankRequest.ProtocolId,
				_serverAddr + "seasonrank-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonRankResponse SeasonRankReqCallback(SeasonRankRequest request);
		public SeasonRankReqCallback OnSeasonRankReqCallback;
		public ISerializer SeasonRankReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonRankRequest request = new SeasonRankRequest();
			request.JsonDeserialize(json);
			return OnSeasonRankReqCallback(request) as ISerializer;
		}

		public delegate bool SeasonRankResCallback(SeasonRankResponse response);
		public SeasonRankResCallback OnSeasonRankResCallback;
		public ISerializer SeasonRankResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonRankResponse response = new SeasonRankResponse();
			response.JsonDeserialize(json);
			OnSeasonRankResCallback(response);
			return null;
		}
		#endregion

		#region SeasonPassReward ------------------------------------------
		public bool SeasonPassRewardReq(SeasonPassRewardRequest request, SeasonPassRewardResCallback callback)
		{
			OnSeasonPassRewardResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(SeasonPassRewardRequest.ProtocolId,
				_serverAddr + "seasonpassreward-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonPassRewardResponse SeasonPassRewardReqCallback(SeasonPassRewardRequest request);
		public SeasonPassRewardReqCallback OnSeasonPassRewardReqCallback;
		public ISerializer SeasonPassRewardReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonPassRewardRequest request = new SeasonPassRewardRequest();
			request.JsonDeserialize(json);
			return OnSeasonPassRewardReqCallback(request) as ISerializer;
		}

		public delegate bool SeasonPassRewardResCallback(SeasonPassRewardResponse response);
		public SeasonPassRewardResCallback OnSeasonPassRewardResCallback;
		public ISerializer SeasonPassRewardResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonPassRewardResponse response = new SeasonPassRewardResponse();
			response.JsonDeserialize(json);
			OnSeasonPassRewardResCallback(response);
			return null;
		}
		#endregion

		#region SeasonPassStep ------------------------------------------
		public bool SeasonPassStepReq(SeasonPassStepRequest request, SeasonPassStepResCallback callback)
		{
			OnSeasonPassStepResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(SeasonPassStepRequest.ProtocolId,
				_serverAddr + "seasonpassstep-v1.0.0",
				request.JsonSerialize());
		}

		public delegate SeasonPassStepResponse SeasonPassStepReqCallback(SeasonPassStepRequest request);
		public SeasonPassStepReqCallback OnSeasonPassStepReqCallback;
		public ISerializer SeasonPassStepReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonPassStepRequest request = new SeasonPassStepRequest();
			request.JsonDeserialize(json);
			return OnSeasonPassStepReqCallback(request) as ISerializer;
		}

		public delegate bool SeasonPassStepResCallback(SeasonPassStepResponse response);
		public SeasonPassStepResCallback OnSeasonPassStepResCallback;
		public ISerializer SeasonPassStepResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			SeasonPassStepResponse response = new SeasonPassStepResponse();
			response.JsonDeserialize(json);
			OnSeasonPassStepResCallback(response);
			return null;
		}
		#endregion

	}
}
