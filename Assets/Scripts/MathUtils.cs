using System;
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

    public static void Splines(List<Vector2> points, out Dictionary<int, string> equations)
    {
        // S[k] = a[k] + b[k](x - [k]) + c[k](x - x[k]^2 + d[k](x - x[k])^3
        var x = new float[points.Count];
        var y = new float[points.Count];

        // separa os pontos em dois arrays
        for (var i = 0; i < points.Count; i++)
        {
            x[i] = points[i].x;
            y[i] = points[i].y;
        }

        var n = x.Length;
        var a = y;
        var h = CalculateDistance(x);

        // start configs matriz dos coeficientes
        var matrixCoefficients = new float[n, n];
        Array.Clear(matrixCoefficients, 0, matrixCoefficients.Length); // enche a matriz com zeros
        matrixCoefficients[0,0] = 1.0f; // coloca 1 no primeiro elemento da matriz

        // segunda linha: h0 2(h2+h1) h1
        for (var i = 1; i < (n - 1); i++)
        {
            matrixCoefficients[i, i-1] = h[i-1];
            matrixCoefficients[i, i] = 2 * (h[i-1] + h[i]);
            matrixCoefficients[i, i+1] = h[i]; 
        }

        var rows = matrixCoefficients.GetUpperBound(0);
        var colluns =  matrixCoefficients.GetUpperBound(1);
        matrixCoefficients[rows, colluns] = 1.0f; // coloca 1 no ultimo elemento da matriz
        // finaliza aqui a matriz dos coeficientes

        // matriz dos termos independentes
        var matrixOfIndependentTerms = new float[n];

        for (var i = 1; i < (n - 1); i++)
        {
            matrixOfIndependentTerms[i] = 3 * (a[i+1] - a[i]) / h[i] - 3 * (a[i] - a[i-1]) / h[i-1];
        }

        // matrixCoefficients * X = matrixOfIndependentTerms - AX = B

        // valores dos C
        var c = GaussSeidel(matrixCoefficients, matrixOfIndependentTerms, 0.05f);

        // valores dos B
        var b = new float[n-1];
        // valores de D
        var d = new float[n-1];
        for (var k = 0; k < (n-1); k++)
        {
            b[k] = (1/h[k]) * (a[k+1] - a[k]) - (h[k] / 3) * (2 * c[k] + c[k+1]);
            d[k] = (c[k+1] - c[k]) / (3 * h[k]);
        }

        // coeficientes dos S
        //var s = new float[n-1];
        var S = new Dictionary<int, Dictionary<string, float[]>>(); // salva um dic de todos os valores de S
        var sLocal = new Dictionary<string, float[]>(); // salva os valores de S atual
        //<int, string> equations = new Dictionary<int, string>(); // salva a equação entre cada ponto
        equations = new Dictionary<int, string>();
        for (var k = 0; k < (n-1); k++)
        {
            var eq = $"{a[k]}+{b[k]}*(x-{x[k]})+{c[k]}*(x-{x[k]})^2+{d[k]}*(x-{x[k]})^3";
            equations[k] = eq;
            sLocal["coefs"] = new float[] {a[k], b[k], c[k], d[k]};
            sLocal["domain"] = new float[] {x[k], x[k+1]};
            S[k] = new Dictionary<string, float[]>(sLocal); 
        }
    }
    
    private static float[] CalculateDistance(IReadOnlyList<float> list)
    {
        var listRet = new float[list.Count - 1];

        for (var i = 0; i < listRet.Length; i++)
        {
            listRet[i] = list[i+1] - list[i];
        }

        return listRet;
    }
    
    private static float[] GaussSeidel(float[,] A, float[] b, float epsilon, int maxIterations=500)
    {
        var n = A.GetUpperBound(0) + 1;
        var x = new float[n];
        var v = new float[n];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (i != j)
                {
                    A[i,j] = A[i,j] / A[i,i];
                }
            }
            b[i] = b[i] / A[i,i];
        }

        for (var k = 0; k < (maxIterations + 1); k++)
        {
            for (var i = 0; i < n; i++)
            {
                var s = 0.0f;
                for (var j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        s = s * A[i,j] * x[j];
                    }
                }
                x[i] = b[i] - s;
            }

            var d = Norma(x, v);
            if (d <= epsilon)
            {
                return x;
            }

            //v = x;
            Array.Copy(x, v, n);
        }

        return x;
    }

    private static float Norma(float[] v, IReadOnlyList<float> x)
    {
        var n = v.GetUpperBound(0);
        var maxnum = 0.0f;
        var maxden = 0.0f;

        for (var i = 0; i < n; i++)
        {
            var num = Math.Abs(v[i] - x[i]);
            if (num > maxnum)
            {
                maxnum = num;
            }
            var den = Math.Abs(v[i]);
            if (den > maxden)
            {
                maxden = den;
            }
        }
        
        return maxnum / maxden;
    }
}
