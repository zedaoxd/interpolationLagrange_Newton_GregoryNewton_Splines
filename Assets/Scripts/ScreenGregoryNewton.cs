using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGregoryNewton : IScreen
{
    [SerializeField] private InputField pointsInputField;
    [SerializeField] private InputField valueToInterpolate;
    [SerializeField] private Text textResult;
    private List<Vector2> pointList = new List<Vector2>();

    
    public void CalculateGregoryNewton()
    {
        pointList = Utils.ConvertInputTextToListPoints(pointsInputField);
        var interpolation = MathUtils.GregoryNewton(pointList, float.Parse(valueToInterpolate.text));
        textResult.text = $"Valor interpolado: {interpolation}";
        pointList.Clear();
    }
}
