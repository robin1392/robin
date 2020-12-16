using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ED;
using Template.Player.RandomWarsPlayer.Common;


namespace Template.Player.RandomWarsPlayer
{
    public partial class RandomWarsPlayerTemplate
    {
        bool OnEditNameController(ERandomWarsPlayerErrorCode errorCode, string editName)
        {
            UI_Popup_Userinfo panel = GameObject.FindObjectOfType<UI_Popup_Userinfo>();
            panel.EditNicknameCallback(editName);
            return true;
        }
    }
}
