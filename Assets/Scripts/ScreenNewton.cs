using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenNewton : IScreen
{
    [SerializeField] private InputField pointsInputField;
    [SerializeField] private InputField pointToInterpolate;
    [SerializeField] private Text textResult;
    private List<Vector2> points = new List<Vector2>();


    public void CalculateNewton()
    {
        // points na posição X = ponto, Y = f_ponto
        points = Utils.ConvertInputTextToListPoints(pointsInputField);

        var value = float.Parse(pointToInterpolate.text);
        textResult.text = $"Aproximação encontrada para f({value}): {MathUtils.Newton(points, value)}";
        points.Clear();
    }
}
