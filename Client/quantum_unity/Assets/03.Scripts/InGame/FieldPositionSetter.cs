using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = ED.Debug;

public class FieldPositionSetter : MonoBehaviour
{
     public float Gap = 1;
     
     [Button]
     public void Apply()
     {
          var fieldCount = 15;
          var columnCount = 5;
          if (transform.childCount != fieldCount)
          {
               Debug.LogError("자식 트랜스폼의 갯수가 15가 아닙니다.");
               return;
          }

          var rowCount = fieldCount / columnCount;
          var xMidIndex = columnCount / 2;
          var yMidIndex = rowCount / 2;

          for (var i = 0; i < fieldCount; ++i)
          {
               var child = transform.GetChild(i);
               var xIndex = i % columnCount;
               var yIndex = i / columnCount;
               var xPosition = (xIndex - xMidIndex) * Gap;
               var yPosition = (yMidIndex - yIndex) * Gap;
               child.localPosition = new Vector3(xPosition, yPosition, 0);
          }
     }
}
