using System;
using System.Collections.Generic;
using System.IO;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Shop.GameBaseShop.Common
{
	public class ShopInfo
	{
		// ShopInfo 테이블 아이디
		public int shopId;
		// 갱신 남은 시간(초단위)
		public int resetRemainTime;
		// 상품 목록
		public List<ShopProductInfo> listProductInfo;
		// 재화 갱신 횟수
		public byte pointResetCount;
		// 광고 갱신 횟수
		public byte adResetCount;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(shopId);
			bw.Write(resetRemainTime);
			int lengthlistProductInfo = (listProductInfo == null) ? 0 : listProductInfo.Count;
			bw.Write(lengthlistProductInfo);
			for (int i = 0; i < lengthlistProductInfo; i++)
				listProductInfo[i].BinarySerialize(bw);
			bw.Write(pointResetCount);
			bw.Write(adResetCount);
		}

		public static ShopInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new ShopInfo();
			data.shopId = br.ReadInt32();
			data.resetRemainTime = br.ReadInt32();
			int lengthlistProductInfo = br.ReadInt32();
			data.listProductInfo = new List<ShopProductInfo>(lengthlistProductInfo);
			for (int i = 0; i < lengthlistProductInfo; i++)
				data.listProductInfo.Add(ShopProductInfo.BinaryDeserialize(br));
			data.pointResetCount = br.ReadByte();
			data.adResetCount = br.ReadByte();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("shopId", shopId);
			json.Add("resetRemainTime", resetRemainTime);
			json.Add("listProductInfo", JsonConvert.SerializeObject(listProductInfo));
			json.Add("pointResetCount", pointResetCount);
			json.Add("adResetCount", adResetCount);
			return json.ToString();
		}

		public static ShopInfo JsonDeserialize(JObject json)
		{
			var data = new ShopInfo();
			data.shopId = (int)json["shopId"];
			data.resetRemainTime = (int)json["resetRemainTime"];
			data.listProductInfo = JsonConvert.DeserializeObject<List<ShopProductInfo>>(json["listProductInfo"].ToString());
			data.pointResetCount = (byte)json["pointResetCount"];
			data.adResetCount = (byte)json["adResetCount"];
			return data;
		}

	}

	public class ShopProductInfo
	{
		// 상품 아이디
		public int shopProductId;
		// 구매 횟수
		public byte buyCount;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(shopProductId);
			bw.Write(buyCount);
		}

		public static ShopProductInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new ShopProductInfo();
			data.shopProductId = br.ReadInt32();
			data.buyCount = br.ReadByte();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("shopProductId", shopProductId);
			json.Add("buyCount", buyCount);
			return json.ToString();
		}

		public static ShopProductInfo JsonDeserialize(JObject json)
		{
			var data = new ShopProductInfo();
			data.shopProductId = (int)json["shopProductId"];
			data.buyCount = (byte)json["buyCount"];
			return data;
		}

	}

}
