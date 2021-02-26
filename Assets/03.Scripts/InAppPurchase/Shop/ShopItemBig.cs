using System;
using System.Collections;
using System.Collections.Generic;
using Percent.Platform;
using RandomWarsResource.Data;
using Template.Shop.GameBaseShop.Common;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemBig : ShopItem
{
    [Header("Subitems")]
    public Text[] arrText_SubitemsCount;
    public Text[] arrText_SubitemsName;

    public override void UpdateContent(ShopInfo shopInfo, ShopProductInfo shopProductInfo)
    {
        base.UpdateContent(shopInfo, shopProductInfo);
        
        SetSubitem(shopProductInfo.shopProductId);
    }

    private void SetSubitem(int id)
    {
        
        TDataShopProductList data;
        if (TableManager.Get().ShopProductList.GetData(id, out data))
        {
            textPItemId.text = LocalizationManager.GetLangDesc(Int32.Parse(data.packageName));
                
            if (data.itemValue01 > 0) arrText_SubitemsCount[0].text = $"x{data.itemValue01}";
            if (data.itemValue02 > 0) arrText_SubitemsCount[1].text = $"x{data.itemValue02}";
            if (data.itemValue03 > 0) arrText_SubitemsCount[2].text = $"x{data.itemValue03}";
            if (data.itemValue04 > 0) arrText_SubitemsCount[3].text = $"x{data.itemValue04}";
            if (data.itemValue05 > 0) arrText_SubitemsCount[4].text = $"x{data.itemValue05}";
            if (data.itemValue06 > 0) arrText_SubitemsCount[5].text = $"x{data.itemValue06}";
        }
    }
}
