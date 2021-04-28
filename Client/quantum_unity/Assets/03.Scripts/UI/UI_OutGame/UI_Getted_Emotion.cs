using ED;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Getted_Emotion : MonoBehaviour
{
    public Button btn;
    public Image image_Icon;
    public GameObject obj_SelectBG;
    public Transform ts_Move;

    private int emotionId;
    private Transform _grandParent;
    private VerticalLayoutGroup verticalLayoutGroup;
    private ContentSizeFitter contentSizeFitter;

    private void Awake()
    {
        _grandParent = transform.parent.parent;
        verticalLayoutGroup = GetComponentInParent<VerticalLayoutGroup>();
        contentSizeFitter = _grandParent.GetComponent<ContentSizeFitter>();
    }

    public void Initialize(int id)
    {
        emotionId = id;
        TDataItemList data;
        if (TableManager.Get().ItemList.GetData(id, out data))
        {
            image_Icon.sprite = FileHelper.GetIcon(data.itemIcon);
        }
    }

    public void Deactive()
    {
        btn.interactable = false;
    }

    public void Click()
    {
        _grandParent.BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
        
        obj_SelectBG.SetActive(true);
        verticalLayoutGroup.enabled = false;
        contentSizeFitter.enabled = false;
        ts_Move.SetParent(_grandParent);

        // int diceCount = UserInfoManager.Get().GetUserInfo().dicGettedDice[_data.id][1];
        // int diceLevel = UserInfoManager.Get().GetUserInfo().dicGettedDice[_data.id][0];
        //
        // RandomWarsResource.Data.TDataDiceUpgrade dataDiceUpgrade;
        // if (TableManager.Get().DiceUpgrade
        //         .GetData(x => x.diceLv == diceLevel + 1 && x.diceGrade == (int)_data.grade,
        //             out dataDiceUpgrade) ==
        //     false)
        // {
        //     return;
        // }
        //
        // int needCount = dataDiceUpgrade.needCard;
        // int needGold = dataDiceUpgrade.needGold;
        // bool isEnableLevelUp = diceCount >= needCount;
        //
        // if (isEnableLevelUp)
        // {
        //     button_Info.gameObject.SetActive(false);
        //     button_LevelUp.gameObject.SetActive(true);
        //
        //     text_LevelUpCost.text = needGold.ToString();
        //     text_LevelUpCost.color =
        //         UserInfoManager.Get().GetUserInfo().gold >= needGold ? Color.white : Color.red;
        // }
        // else
        // {
        //     button_Info.gameObject.SetActive(true);
        //     button_LevelUp.gameObject.SetActive(false);
        // }
    }
    
    public void DeactivateSelectedObject()
    {
        obj_SelectBG.SetActive(false);
        verticalLayoutGroup.enabled = true;
        contentSizeFitter.enabled = true;
        ts_Move.SetParent(transform);
    }

    public void Click_Use()
    {
        DeactivateSelectedObject();
        GetComponentInParent<UI_Panel_Dice>().Click_EmotionUse(emotionId);
    }
}
