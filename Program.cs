using System;

namespace ConsoleApp2;

public sealed class Calculator
{
    private static readonly List<char> _firstOperations = ['+', '-'];
    private static readonly List<char> _secondOperations = ['*', '/'];

    private static float PerformOperation(float left, float right, char operatorChar)
    {
        return operatorChar switch
        {
            '+' => left + right,
            '-' => left - right,
            '*' => left * right,
            '/' => left / right,
            _ => throw new ArgumentException($"invalid operator: {operatorChar}"),
        };
    }

    private static string EvaluateBrackets(string expression)
    {
        var openBracketsCount = 0;
        var startIndex = -1;

        for (var index = 0; index < expression.Length; index++)
        {
            var executedChar = expression[index];
            switch (executedChar)
            {
                case '(':
                    if (openBracketsCount == 0) startIndex = index;
                    openBracketsCount++;
                    break;

                case ')':
                    openBracketsCount--;
                    if (openBracketsCount == 0)
                    {
                        var result = Calculate(expression.Substring(startIndex + 1, index - startIndex - 1));
                        var newExpression = expression[..startIndex] + result + expression[(index + 1)..];
                        return EvaluateBrackets(newExpression);
                    }
                    break;
            }
        }
        return expression;
    }

    private static int FindOperatorIndex(string expression)
    {
        for (int index = expression.Length - 1; index >= 0; index--)
        {
            var pr_char = expression[index];
            var leftChar = index > 0
                ? (char?)expression[index - 1]
                : null;

            if (_firstOperations.Contains(pr_char) && leftChar.HasValue && Char.IsDigit(leftChar.Value))
            {
                return index;
            }
        }
        return expression.LastIndexOfAny(_secondOperations.ToArray());
    }

    public static float Calculate(string expression)
    {
        var simplifiedExpression = EvaluateBrackets(expression);
        var operatorIndex = FindOperatorIndex(simplifiedExpression);
        if (operatorIndex == -1) return float.Parse(simplifiedExpression);

        var leftStr = simplifiedExpression[..operatorIndex];
        var rightStr = simplifiedExpression[(operatorIndex + 1)..];

        var leftFloat = float.TryParse(leftStr, out var leftVal) ? leftVal : Calculate(leftStr);
        var rightFloat = float.TryParse(rightStr, out var rightVal) ? rightVal : Calculate(rightStr);

        var operatorChar = simplifiedExpression[operatorIndex];
        return PerformOperation(leftFloat, rightFloat, operatorChar);
    }

    public static void Main()
    {
        Console.WriteLine($"Доступные операции: {string.Join(", ", _firstOperations.Concat(_secondOperations))}");
        Console.WriteLine("Введите вычисляемое выражение и нажмите Enter");
        var str = Console.ReadLine();
        if (!String.IsNullOrWhiteSpace(str))
            Console.WriteLine(Calculate(str));
    }
}
