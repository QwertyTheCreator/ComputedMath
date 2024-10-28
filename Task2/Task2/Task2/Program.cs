namespace Task2;

public static class Program
{
    static void Main(string[] args)
    {
        var size = 15;

        var matrix = ConstructMatrix(size);
        var rightPart = RandomRightPart(size);

        var answers = SolveWithRotation(matrix, rightPart);

        foreach (var ans in answers)
        {
            Console.WriteLine(ans);
        }

        var residualNorm = ResidualNorm(matrix, answers, rightPart);
        Console.WriteLine($"Невязка: {residualNorm}");
    }

    static decimal[] SolveWithRotation(decimal[][] m, decimal[] b)
    {
        int n = m.Length;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                decimal c = m[i][i] / (decimal)Math.Sqrt((double)(m[i][i] * m[i][i] + m[j][i] * m[j][i]));
                decimal s = m[j][i] / (decimal)Math.Sqrt((double)(m[i][i] * m[i][i] + m[j][i] * m[j][i]));

                decimal[] tmp1 = new decimal[m[i].Length];
                decimal[] tmp2 = new decimal[m[j].Length];

                for (int k = 0; k < m[i].Length; k++)
                {
                    tmp1[k] = m[i][k] * c + m[j][k] * s;
                    tmp2[k] = m[i][k] * -s + m[j][k] * c;
                }

                m[i] = tmp1;
                m[j] = tmp2;
            }
        }

        decimal[] x = new decimal[n];
        for (int k = n - 1; k >= 0; k--)
        {
            x[k] = (b[k] - Enumerable.Range(k + 1, n - k - 1).Select(j => m[k][j] * x[j]).Sum()) / m[k][k];
        }

        return x;
    }

    static (decimal[][] Q, decimal[][] R) QRDecomposition(decimal[][] A)
    {
        int m = A.Length;
        int n = A[0].Length;
        decimal[][] Q = new decimal[m][];
        decimal[][] R = new decimal[n][];

        // Инициализация Q и R
        for (int i = 0; i < m; i++)
            Q[i] = new decimal[m];
        for (int j = 0; j < n; j++)
            R[j] = new decimal[n];

        for (int j = 0; j < n; j++)
        {
            // Копируем вектор A[:, j] в R[:, j]
            for (int i = 0; i < m; i++)
            {
                R[j][i] = A[i][j];
            }

            // Вычисляем Q[:, j] и R[j][j]
            for (int i = 0; i < j; i++)
            {
                decimal dotProduct = 0;
                for (int k = 0; k < m; k++)
                {
                    dotProduct += Q[k][i] * R[j][k];
                }
                R[i][j] = dotProduct;

                for (int k = 0; k < m; k++)
                {
                    R[j][k] -= dotProduct * Q[k][i];
                }
            }

            // Нормируем вектор R[:, j] и сохраняем в Q[:, j]
            decimal norm = (decimal)Math.Sqrt((double)R[j].Select(x => x * x).Sum());
            for (int i = 0; i < m; i++)
            {
                Q[i][j] = R[j][i] / (decimal)norm;
                R[j][i] = R[j][i] / (decimal)norm; // Нормируем для R
            }
        }

        return (Q, R);
    }

    public static decimal ResidualNorm(decimal[][] matrix, decimal[] answerVector, decimal[] rightVector) //Метод для расчёта нормы(Кубической) невязки
    {
        decimal maxAbs = 0m;

        for (int i = 0; i < answerVector.Count(); i++)
        {
            var residualAbs = decimal.Abs(GetEquationResidual(matrix[i], answerVector, rightVector[i]));

            maxAbs = residualAbs > maxAbs
                ? residualAbs
                : maxAbs;
        }

        return maxAbs;
    }

    public static decimal GetEquationResidual(decimal[] equation, decimal[] answers, decimal rightPart)
    {
        var sum = 0m;

        for (int i = 0; i < answers.Count(); i++)
        {
            sum += equation[i] * answers[i];
        }

        return rightPart - sum;
    }

    public static decimal[][] ConstructMatrix(int size = 15)
    {
        var matrix = new decimal[size][];

        for (int i = 0; i < size; i++)
        {
            matrix[i] = new decimal[size];
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i == j)
                {
                    matrix[i][j] = 5m * (i + 1);

                    continue;
                }

                matrix[i][j] = 0.1m * (i + 1) * (j + 1);
            }
        }

        return matrix;
    }

    public static decimal[] RandomRightPart(int size = 15)
    {
        var rightPart = new decimal[size];

        for (int i = 0; i < size; i++)
        {
            rightPart[i] = (decimal)Random.Shared.NextDouble();
        }

        return rightPart;
    }
}
