using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Service.Core;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Shop.GameBaseShop.Common
{
	public class ShopInfoRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 100001;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
		}

	}

	public class ShopInfoResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 100002;
		// 상점 정보
		public List<ShopInfo> listShopInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistShopInfo = (listShopInfo == null) ? 0 : listShopInfo.Count;
				bw.Write(lengthlistShopInfo);
				for (int i = 0; i < lengthlistShopInfo; i++)
					listShopInfo[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistShopInfo = br.ReadInt32();
				this.listShopInfo = new List<ShopInfo>(lengthlistShopInfo);
				for (int i = 0; i < lengthlistShopInfo; i++)
					this.listShopInfo.Add(ShopInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("listShopInfo", JsonConvert.SerializeObject(listShopInfo));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listShopInfo = JsonConvert.DeserializeObject<List<ShopInfo>>(jObject["listShopInfo"].ToString());
		}

	}

	public class ShopBuyRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 100011;
		// 
		public int shopId;
		// 
		public int shopProductId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(shopId);
				bw.Write(shopProductId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.shopId = br.ReadInt32();
				this.shopProductId = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("shopId", shopId);
			json.Add("shopProductId", shopProductId);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.shopId = (int)jObject["shopId"];
			this.shopProductId = (int)jObject["shopProductId"];
		}

	}

	public class ShopBuyResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 100012;
		// 상점 아이디
		public int shopId;
		// 상품 정보
		public ShopProductInfo shopProductInfo;
		// 변경 상품 정보
		public ShopProductInfo changeProductInfo;
		// 
		public ItemBaseInfo deleteItemInfo;
		// 
		public List<ItemBaseInfo> listRewardInfo;
		// 
		public List<QuestData> listQuestData;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(shopId);
				shopProductInfo.BinarySerialize(bw);
				changeProductInfo.BinarySerialize(bw);
				deleteItemInfo.BinarySerialize(bw);
				int lengthlistRewardInfo = (listRewardInfo == null) ? 0 : listRewardInfo.Count;
				bw.Write(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					listRewardInfo[i].BinarySerialize(bw);
				int lengthlistQuestData = (listQuestData == null) ? 0 : listQuestData.Count;
				bw.Write(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					listQuestData[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.shopId = br.ReadInt32();
				this.shopProductInfo = ShopProductInfo.BinaryDeserialize(br);
				this.changeProductInfo = ShopProductInfo.BinaryDeserialize(br);
				this.deleteItemInfo = ItemBaseInfo.BinaryDeserialize(br);
				int lengthlistRewardInfo = br.ReadInt32();
				this.listRewardInfo = new List<ItemBaseInfo>(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					this.listRewardInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
				int lengthlistQuestData = br.ReadInt32();
				this.listQuestData = new List<QuestData>(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					this.listQuestData.Add(QuestData.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("shopId", shopId);
			json.Add("shopProductInfo", shopProductInfo.JsonSerialize());
			json.Add("changeProductInfo", changeProductInfo.JsonSerialize());
			json.Add("deleteItemInfo", deleteItemInfo.JsonSerialize());
			json.Add("listRewardInfo", JsonConvert.SerializeObject(listRewardInfo));
			json.Add("listQuestData", JsonConvert.SerializeObject(listQuestData));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.shopId = (int)jObject["shopId"];
			this.shopProductInfo = ShopProductInfo.JsonDeserialize((JObject)jObject["shopProductInfo"]);
			this.changeProductInfo = ShopProductInfo.JsonDeserialize((JObject)jObject["changeProductInfo"]);
			this.deleteItemInfo = ItemBaseInfo.JsonDeserialize((JObject)jObject["deleteItemInfo"]);
			this.listRewardInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listRewardInfo"].ToString());
			this.listQuestData = JsonConvert.DeserializeObject<List<QuestData>>(jObject["listQuestData"].ToString());
		}

	}

	public class ShopPurchaseRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 100021;
		// 
		public int shopId;
		// 
		public int productId;
		// 
		public string receiptInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(shopId);
				bw.Write(productId);
				bw.Write(receiptInfo);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.shopId = br.ReadInt32();
				this.productId = br.ReadInt32();
				this.receiptInfo = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("shopId", shopId);
			json.Add("productId", productId);
			json.Add("receiptInfo", receiptInfo);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.shopId = (int)jObject["shopId"];
			this.productId = (int)jObject["productId"];
			this.receiptInfo = (string)jObject["receiptInfo"];
		}

	}

	public class ShopPurchaseResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 100022;
		// 상점 아이디
		public int shopId;
		// 상품 정보
		public ShopProductInfo shopProductInfo;
		// 변경 상품 정보
		public ShopProductInfo changeProductInfo;
		// 
		public ItemBaseInfo deleteItemInfo;
		// 
		public List<ItemBaseInfo> listRewardInfo;
		// 
		public List<QuestData> listQuestData;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(shopId);
				shopProductInfo.BinarySerialize(bw);
				changeProductInfo.BinarySerialize(bw);
				deleteItemInfo.BinarySerialize(bw);
				int lengthlistRewardInfo = (listRewardInfo == null) ? 0 : listRewardInfo.Count;
				bw.Write(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					listRewardInfo[i].BinarySerialize(bw);
				int lengthlistQuestData = (listQuestData == null) ? 0 : listQuestData.Count;
				bw.Write(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					listQuestData[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.shopId = br.ReadInt32();
				this.shopProductInfo = ShopProductInfo.BinaryDeserialize(br);
				this.changeProductInfo = ShopProductInfo.BinaryDeserialize(br);
				this.deleteItemInfo = ItemBaseInfo.BinaryDeserialize(br);
				int lengthlistRewardInfo = br.ReadInt32();
				this.listRewardInfo = new List<ItemBaseInfo>(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					this.listRewardInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
				int lengthlistQuestData = br.ReadInt32();
				this.listQuestData = new List<QuestData>(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					this.listQuestData.Add(QuestData.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("shopId", shopId);
			json.Add("shopProductInfo", shopProductInfo.JsonSerialize());
			json.Add("changeProductInfo", changeProductInfo.JsonSerialize());
			json.Add("deleteItemInfo", deleteItemInfo.JsonSerialize());
			json.Add("listRewardInfo", JsonConvert.SerializeObject(listRewardInfo));
			json.Add("listQuestData", JsonConvert.SerializeObject(listQuestData));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.shopId = (int)jObject["shopId"];
			this.shopProductInfo = ShopProductInfo.JsonDeserialize((JObject)jObject["shopProductInfo"]);
			this.changeProductInfo = ShopProductInfo.JsonDeserialize((JObject)jObject["changeProductInfo"]);
			this.deleteItemInfo = ItemBaseInfo.JsonDeserialize((JObject)jObject["deleteItemInfo"]);
			this.listRewardInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listRewardInfo"].ToString());
			this.listQuestData = JsonConvert.DeserializeObject<List<QuestData>>(jObject["listQuestData"].ToString());
		}

	}

	public class ShopResetRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 100031;
		// 상점 아이디
		public int shopId;
		// 리셋 타입
		public int resetType;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(shopId);
				bw.Write(resetType);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.shopId = br.ReadInt32();
				this.resetType = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("shopId", shopId);
			json.Add("resetType", resetType);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.shopId = (int)jObject["shopId"];
			this.resetType = (int)jObject["resetType"];
		}

	}

	public class ShopResetResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 100032;
		// 상점 정보
		public ShopInfo shopInfo;
		// 사용 아이템 정보
		public ItemBaseInfo deleteItemInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				shopInfo.BinarySerialize(bw);
				deleteItemInfo.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.shopInfo = ShopInfo.BinaryDeserialize(br);
				this.deleteItemInfo = ItemBaseInfo.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("shopInfo", shopInfo.JsonSerialize());
			json.Add("deleteItemInfo", deleteItemInfo.JsonSerialize());
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.shopInfo = ShopInfo.JsonDeserialize((JObject)jObject["shopInfo"]);
			this.deleteItemInfo = ItemBaseInfo.JsonDeserialize((JObject)jObject["deleteItemInfo"]);
		}

	}

}
