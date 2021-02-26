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
    public Image[] arrImage_SubitemsIcon;

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
            
            if (data.itemValue01 > 0)
            {
                if (arrText_SubitemsCount[0] != null) arrText_SubitemsCount[0].text = $"x{data.itemValue01}";
                TDataItemList item;
                if (TableManager.Get().ItemList.GetData(data.itemId01, out item))
                {
                    if (arrText_SubitemsName[0] != null)
                    {
                        arrText_SubitemsName[0].text = LocalizationManager.GetLangDesc(item.itemName_langId);
                    }

                    if (arrImage_SubitemsIcon[0] != null)
                    {
                        arrImage_SubitemsIcon[0].sprite = FileHelper.GetIcon(item.itemIcon);
                    }
                }
            }
            else
            {
                arrText_SubitemsCount[0].transform.parent.gameObject.SetActive(false);
            }
            
            if (data.itemValue02 > 0)
            {
                if (arrText_SubitemsCount[1] != null) arrText_SubitemsCount[1].text = $"x{data.itemValue02}";
                TDataItemList item;
                if (TableManager.Get().ItemList.GetData(data.itemId02, out item))
                {
                    if (arrText_SubitemsName[1] != null)
                    {
                        arrText_SubitemsName[1].text = LocalizationManager.GetLangDesc(item.itemName_langId);
                    }
                    
                    if (arrImage_SubitemsIcon[1] != null)
                    {
                        arrImage_SubitemsIcon[1].sprite = FileHelper.GetIcon(item.itemIcon);
                    }
                }
            }
            else
            {
                arrText_SubitemsCount[1].transform.parent.gameObject.SetActive(false);
            }
            
            if (data.itemValue03 > 0)
            {
                if (arrText_SubitemsCount[2] != null) arrText_SubitemsCount[2].text = $"x{data.itemValue03}";
                TDataItemList item;
                if (TableManager.Get().ItemList.GetData(data.itemId03, out item))
                {
                    if (arrText_SubitemsName[2] != null)
                    {
                        arrText_SubitemsName[2].text = LocalizationManager.GetLangDesc(item.itemName_langId);
                    }

                    if (arrImage_SubitemsIcon[2] != null)
                    {
                        arrImage_SubitemsIcon[2].sprite = FileHelper.GetIcon(item.itemIcon);
                    }
                }
            }
            else
            {
                arrText_SubitemsCount[2].transform.parent.gameObject.SetActive(false);
            }
            
            if (data.itemValue04 > 0)
            {
                if (arrText_SubitemsCount[3] != null) arrText_SubitemsCount[3].text = $"x{data.itemValue04}";
                TDataItemList item;
                if (TableManager.Get().ItemList.GetData(data.itemId04, out item))
                {
                    if (arrText_SubitemsName[3] != null)
                    {
                        arrText_SubitemsName[3].text = LocalizationManager.GetLangDesc(item.itemName_langId);
                    }

                    if (arrImage_SubitemsIcon[3] != null)
                    {
                        arrImage_SubitemsIcon[3].sprite = FileHelper.GetIcon(item.itemIcon);
                    }
                }
            }
            else
            {
                arrText_SubitemsCount[3].transform.parent.gameObject.SetActive(false);
            }
            
            if (data.itemValue05 > 0)
            {
                if (arrText_SubitemsCount[4] != null) arrText_SubitemsCount[4].text = $"x{data.itemValue05}";
                TDataItemList item;
                if (TableManager.Get().ItemList.GetData(data.itemId05, out item))
                {
                    if (arrText_SubitemsName[4] != null)
                    {
                        arrText_SubitemsName[4].text = LocalizationManager.GetLangDesc(item.itemName_langId);
                    }

                    if (arrImage_SubitemsIcon[4] != null)
                    {
                        arrImage_SubitemsIcon[4].sprite = FileHelper.GetIcon(item.itemIcon);
                    }
                }
            }
            else
            {
                arrText_SubitemsCount[4].transform.parent.gameObject.SetActive(false);
            }
            
            if (data.itemValue06 > 0)
            {
                if (arrText_SubitemsCount[5] != null) arrText_SubitemsCount[5].text = $"x{data.itemValue06}";
                TDataItemList item;
                if (TableManager.Get().ItemList.GetData(data.itemId06, out item))
                {
                    if (arrText_SubitemsName[5] != null)
                    {
                        arrText_SubitemsName[5].text = LocalizationManager.GetLangDesc(item.itemName_langId);
                    }

                    if (arrImage_SubitemsIcon[5] != null)
                    {
                        arrImage_SubitemsIcon[5].sprite = FileHelper.GetIcon(item.itemIcon);
                    }
                }
            }
            else
            {
                arrText_SubitemsCount[5].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
