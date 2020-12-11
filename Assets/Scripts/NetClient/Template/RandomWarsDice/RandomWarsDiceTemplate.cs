#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Item.RandomWarsDice.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;


namespace Template.Item.RandomWarsDice
{
    public partial class RandomWarsDiceTemplate : RandomWarsDiceProtocol
    {
        public RandomWarsDiceTemplate()
        {
            ReceiveUpdateDeckAckCallback = OnUpdateDeckController;
            //HttpReceiveLevelupDiceAckCallback = OnLevelupDiceController;
            //HttpReceiveOpenBoxAckCallback = OnOpenBoxController;
        }
    }
}
