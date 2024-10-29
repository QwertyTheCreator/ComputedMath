using System.Diagnostics;

namespace Task1;

public static class Program
{
    static void Main(string[] args)
    {
        var size = 10;

        var matrixTest = GenerateSymmetricPositiveDefiniteMatrix(size);      
        ShowMatrix(matrixTest);
        
        var buildWatch = new Stopwatch();
        buildWatch.Start();

        var holets = new HoletskiyMatrix(matrixTest);

        buildWatch.Stop();

        long total = 0;
        for (int i = 0; i < 1000; i++)
        {
            var rightPart = SetRandomRightPart(size);

            ShowRightPart(rightPart);

            var solutWatch = new Stopwatch();
            solutWatch.Start();

            var answer = holets.SolveSoLE(rightPart);
            var residual = holets.ResidualNorm(answer, rightPart);

            solutWatch.Stop();

            total += solutWatch.ElapsedTicks;

            ShowRightPart(answer, "X:");

            Console.WriteLine("Невязка: " + residual);
        }

        Console.WriteLine($"Определитель: {holets.Determinator}");
        Console.WriteLine($"Затраченное время на трансформацию матрицы: {buildWatch.ElapsedTicks}");
        Console.WriteLine($"Затраченное время на решение систем: {total}");

    }

    public static void ShowMatrix(decimal[][] matrix)
    {
        Console.WriteLine("Исходная матрица:");
        for (int i = 0; i < matrix.Length; i++)
        {
            Console.Write(string.Concat(Enumerable.Repeat("0 ", i)));
            for (int j = 0; j < matrix.Length - i; j++)
            {
                Console.Write(decimal.Round(matrix[i][j], 2) + " ");
            }

            Console.WriteLine();
        }
    }

    public static void ShowRightPart(decimal[] arr, string description = "Правая часть")
    {
        Console.WriteLine(description);
        foreach (var item in arr)
        {
            Console.Write(item + " ");
        }

        Console.WriteLine();
    }

    public static decimal[][] SetRandomMatrix(int size = 5)
    {
        decimal[][] matrix = new decimal[size][];

        for (int i = 0; i < size; i++)
        {
            matrix[i] = new decimal[size - i];
        }

        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < matrix[i].Length; ++j)
            {
                matrix[i][j] = (decimal)Random.Shared.NextDouble();
            }
        }

        return matrix;
    }

    public static decimal[] SetRandomRightPart(int size = 5)
    {
        decimal[] rightPart = new decimal[size];

        for (int i = 0; i < rightPart.Length; i++)
        {
            rightPart[i] = (decimal)Random.Shared.NextDouble();
        }

        return rightPart;
    }

    static decimal[][] GenerateSymmetricPositiveDefiniteMatrix(int size)
    {
        // Генерация случайной матрицы
        decimal[][] randomMatrix = GenerateRandomMatrix(size);

        // Умножаем матрицу на ее транспонированную версию
        decimal[][] symmetricMatrix = MultiplyMatrices(randomMatrix, TransposeMatrix(randomMatrix));

        // Увеличиваем диагональные элементы для обеспечения положительной определенности
        for (int i = 0; i < size; i++)
        {
            symmetricMatrix[i][i] += 10; // Добавляем положительное число к диагонали
        }

        decimal[][] memorized = new decimal[size][];
        for (int i = 0; i < size; i++)
        {
            memorized[i] = new decimal[size - i];
        }

        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < memorized[i].Length; ++j)
            {
                memorized[i][j] = symmetricMatrix[i][j + i];
            }
        }

        return memorized;
    }

    static decimal[][] GenerateRandomMatrix(int size)
    {
        Random random = new Random();
        decimal[][] matrix = new decimal[size][];
        for (int i = 0; i < size; i++)
        {
            matrix[i] = new decimal[size];
            for (int j = 0; j < size; j++)
            {
                matrix[i][j] = (decimal)(random.NextDouble() * 10); // Случайные значения от 0 до 10
            }
        }
        return matrix;
    }

    static decimal[][] TransposeMatrix(decimal[][] matrix)
    {
        int size = matrix.Length;
        decimal[][] transposed = new decimal[size][];
        for (int i = 0; i < size; i++)
        {
            transposed[i] = new decimal[size];
            for (int j = 0; j < size; j++)
            {
                transposed[i][j] = matrix[j][i];
            }
        }
        return transposed;
    }

    static decimal[][] MultiplyMatrices(decimal[][] A, decimal[][] B)
    {
        int size = A.Length;
        decimal[][] result = new decimal[size][];
        for (int i = 0; i < size; i++)
        {
            result[i] = new decimal[size];
            for (int j = 0; j < size; j++)
            {
                result[i][j] = 0;
                for (int k = 0; k < size; k++)
                {
                    result[i][j] += A[i][k] * B[k][j];
                }
            }
        }
        return result;
    }
}
