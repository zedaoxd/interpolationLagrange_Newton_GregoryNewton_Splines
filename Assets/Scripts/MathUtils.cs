using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static float Lagrange(List<Vector2> points, float value)
    {
        var coefficients = new float[points.Count];
        for (var i = 0; i < points.Count; i++)
        {
            var l = 1.0f;
            for (var j = 0; j < points.Count; j++)
            {
                if (i != j)
                {
                    l *= (value - points[j].x) / (points[i].x - points[j].x);
                }
            }
            coefficients[i] = l;
        }

        var pn = 0.0f;
        for (var i = 0; i < coefficients.Length; i++)
        {
            pn += points[i].y * coefficients[i];
        }
        
        return pn;
    }

    public static float Newton(List<Vector2> points, float value)
    {
        var table = new List<List<float>>();
        var f_pontos = new List<float>();
        for (var i = 0; i < points.Count; i++)
        {
            f_pontos.Add(points[i].y);
        }
        table.Add(f_pontos); // add lista com os valores da ordem 0
        
        var step = 1;
        for (var i = 0; i < (points.Count - 1); i++) // -1 pq se passar 3 pontos, poli de grau 2... se passa 4 de grau 3... etc...
        {
            var order = new List<float>();
            for (var j = 0; j < (table[i].Count - 1); j++)
            {
                var dif_dividida = (table[i][j + 1] - table[i][j]) / (points[j + step].x - points[j].x);
                order.Add(dif_dividida);
            }
            table.Add(order); // add mais uma linha na tabela com as diferenças dividas
            step++;
        }
        
        for (var i = 0; i < table.Count; i++)
        {
            Debug.Log($"order[{i}]: ");
            for (var j = 0; j < table[i].Count; j++)
            {
                Debug.Log(table[i][j]);
            }
        }

        // iniciando calculo de aproximação
        var degree  = 0;
        var approximation = 0.0f;
        for (var i = 0; i < table.Count; i++)
        {
            var factor = table[i][0];
            for (var j = 0; j < degree ; j++)
            {
                factor *= (value - points[j].x);
            }
            degree ++;
            approximation += factor;
        }

        return approximation;
    }

    public static float GregoryNewton(List<Vector2> points, float value)
    {
        var h = points[1].x - points[0].x; 
        var t = (value - points[0].x) / h; 
        var coefficient = t; 
        var sum = points[0].y; 
        var k = 1;

        var y = new List<float>();
        foreach (var vector2 in points) // lista somente com os pontos Y
        {
            y.Add(vector2.y);
        }

        for (var i = points.Count; i > 1; i--)
        {
            y = CalculateDifference(y); // retorna a dif dos pontos da lista passada, e como passa y como parametro retorna a lista delta Y, delta2 Y etc
            sum += coefficient * y[0]; 
            coefficient *= (t - k) / (k + 1); 
            k++;
        }

        return sum;
    }

    private static List<float> CalculateDifference(IReadOnlyList<float> axis)
    {
        var list = new List<float>();
        for (var i = 0; i < (axis.Count - 1); i++)
        {
            list.Add(axis[i + 1] - axis[i]);
        }

        return list;
    }

    private static List<float> CalculateDifference(IReadOnlyList<Vector2> listPoints, char axis)
    {
        var list = new List<float>();
        
        switch (axis)
        {
            case 'x':
                for (var i = 0; i < (listPoints.Count - 1); i++)
                {
                    list.Add(listPoints[i + 1].x - listPoints[i].x);
                }
                break;
            
            case 'y':
                for (var i = 0; i < (listPoints.Count - 1); i++)
                {
                    list.Add(listPoints[i + 1].y - listPoints[i].y);
                }
                break;
            
            default:
                Debug.LogError($"unexpected axis: {axis}");
                break;
        }
        
        return list;
    }
}
