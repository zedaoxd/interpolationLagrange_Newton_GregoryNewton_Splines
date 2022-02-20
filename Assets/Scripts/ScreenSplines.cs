using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSplines : IScreen
{
    [SerializeField] private InputField pointInputField;
    private List<Vector2> points = new List<Vector2>();
    [SerializeField] private Text textResult;
    public void CalculateSplines()
    {
        points = Utils.ConvertInputTextToListPoints(pointInputField);
        
        MathUtils.Splines(points, out var equations);
        foreach (var equation in equations)
        {
            textResult.text += $"{equation.Value}\n";
        }
        
        points.Clear();
    }
}
