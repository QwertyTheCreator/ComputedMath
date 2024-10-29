namespace Task1;

public class HoletskiyMatrix
{
    private readonly decimal[] _diag;
    private readonly decimal[][] _sMatrix;
    private readonly decimal[][] _baseMatrix;
    private readonly int _size;
    public decimal Determinator { get; } = 1m;

    public HoletskiyMatrix(decimal[][] baseMatrix)
    {
        _size = baseMatrix.Length;
        _baseMatrix = baseMatrix;
        _diag = new decimal[_size]; //Диагональная матрица D
        _sMatrix = new decimal[_size][]; //Верхнетреугольная матрица S

        for (int i = 0; i < _size; i++)
        {
            _sMatrix[i] = new decimal[_size - i];

            for (int j = 0; j < _size - i; j++)
            {
                if (j == 0)
                {
                    var sdSum = SdSum(i);

                    _diag[i] = D(i, sdSum);
                    _sMatrix[i][j] = SDiag(i, sdSum);

                    Determinator *= _sMatrix[i][j] * _sMatrix[i][j] * _diag[i]; //Вычисление определителя

                    continue;
                }

                _sMatrix[i][j] = S(i, j);
            }
        }
    }

    public decimal[] SolveSoLE(decimal[] rightVector) // Решение СЛАУ
    {
        decimal[] yVector = new decimal[_size];

        for (int i = 0; i < _size; i++)
        {
            yVector[i] = Y(i, rightVector, yVector);
        }

        decimal[] xVector = new decimal[_size];

        for (int i = _size - 1; i >= 0; i--)
        {
            xVector[i] = X(i, yVector, xVector);
        }

        return xVector;
    }

    public decimal ResidualNorm(decimal[] answerVector, decimal[] rightVector) //Метод для расчёта нормы(Кубической) невязки
    {
        var maxAbs = 0m;

        for (int i = 0; i < _size; i++)
        {
            var sum = 0m;

            var ansPos = 0;
            for (int hight = 0, width = i; hight <= i; hight++, width--)
            {
                sum += _baseMatrix[hight][width] * answerVector[ansPos];
                ansPos++;
            }

            for (int j = 1; j < _size - i; j++)
            {
                sum += _baseMatrix[i][j] * answerVector[ansPos];
                ansPos++;
            }

            var resAbs = decimal.Abs(rightVector[i] - sum);

            maxAbs = resAbs > maxAbs
                ? resAbs
                : maxAbs;
        }

        return maxAbs;
    }

    private decimal X(int i, decimal[] yVector, decimal[] xVector) // Поиск Xi
    {
        var sum = 0m;

        var iFor = i;
        for (int k = i + 1; k < _size; k++)
        {
            sum += _diag[k] * _sMatrix[i][k - i] * xVector[k];
            iFor--;
        }

        return (yVector[i] - sum) / _sMatrix[i][0];
    }

    private decimal Y(int i, decimal[] rightVector, decimal[] yVector) //Поиск Yi
    {
        var sum = 0m;

        var iFor = i;
        for (int k = 0; k < i; k++)
        {
            sum += _sMatrix[k][iFor] * yVector[k];
            iFor--;
        }

        return (rightVector[i] - sum) / _sMatrix[i][0];
    }

    private decimal D(int i, decimal sdSum) =>
         decimal.Sign(_baseMatrix[i][0] - sdSum);

    private decimal SDiag(int i, decimal sdSum) => // Функция для получения Sii элемента
                                                   //Матрицы S
        SqrtFromAbs(_baseMatrix[i][0] - sdSum);

    private decimal S(int i, int j) // Функция для получения Sij, j != i элемента
                                    // из матрицы S
    {
        var iFor = i;
        var jFor = j + i;

        var sum = 0m;

        for (int k = 0; k < i; k++)
        {
            sum += _sMatrix[k][iFor] * _diag[k] * _sMatrix[k][jFor];
            iFor--;
            jFor--;
        }

        return (_baseMatrix[i][j] - sum) / (_sMatrix[i][0] * _diag[i]);
    }

    private decimal SdSum(int i) // Функция для подсчёта суммы 
                                 // Из формулы расчёта диагональных элементов матрицы S
    {
        var sum = 0m;

        var iFor = i;
        for (int k = 0; k < i; k++)
        {
            sum += _sMatrix[k][iFor] * _sMatrix[k][iFor] * _diag[k];
            iFor--;
        }

        return sum;
    }

    private decimal SqrtFromAbs(decimal value) => //Урощения операции взятия корня из модуля
        (decimal)Math.Sqrt((double)decimal.Abs(value));
}
