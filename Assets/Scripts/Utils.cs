using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utils : MonoBehaviour
{
    public static List<Vector2> ConvertInputTextToListPoints(InputField inputField)
    {
        var list = new List<Vector2>();
        var pointsStringArray = inputField.text.Trim().Split(' ');
        
        foreach (var t in pointsStringArray)
        {
            var pointString = t.Split(',');
            var point = new Vector2(float.Parse(pointString[0]), float.Parse(pointString[1]));
            list.Add(point);
        }

        return list;
    }
}
