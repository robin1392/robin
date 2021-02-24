using System;
using Service.Core;

namespace Template.Shop.GameBaseShop.Common
{
    // 상점 정보
    [Serializable]
    public class ShopInfo
    {
        // ShopInfo 테이블 아이디
        public int shopId;
        // 갱신 남은 시간(초단위)
        public int resetRemainTime;
        // 갱신 횟수
        public int resetCount;
        // 상품 목록
        public ShopProductInfo[] arrayProductInfo;
        // 초기화 정보
        public ShopResetInfo resetInfo;
    }


    // 상점 상품 정보
    [Serializable]
    public class ShopProductInfo
    {
        // 상품 아이디
        public int shopProductId;
        // 구매 횟수
        public byte buyCount;
    }


    // 상점 초기화 정보
    [Serializable]
    public class ShopResetInfo
    {
    }
}