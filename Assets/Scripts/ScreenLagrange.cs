using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLagrange : IScreen
{
    [SerializeField] private InputField pointsText;
    [SerializeField] private InputField valueCalculate;
    [SerializeField] private Text textResult;
    private List<Vector2> points = new List<Vector2>();
    
    public void CalculateLagrange()
    {
        /*
        var pointsStringArray = pointsText.text.Trim().Split(' ');
        foreach (var t in pointsStringArray)
        {
            var pointString = t.Split(',');
            var point = new Vector2(float.Parse(pointString[0]), float.Parse(pointString[1]));
            points.Add(point);
        }
        */
        points = Utils.ConvertInputTextToListPoints(pointsText);

        var value = float.Parse(valueCalculate.text);
        var result = MathUtils.Lagrange(points, value);
        textResult.text = $"p({valueCalculate.text}): {result}";
        points.Clear();
    }
}
