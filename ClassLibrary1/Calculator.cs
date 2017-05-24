using System;

namespace ClassLibrary1
{
    public class Calculator
    {
        public int Calculate(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("", "expression");

            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("expression is not allowed to be blank", "expression");

            char? op = null;
            int? num = null;
            int? acc = null;

            int i = 0;
            ConsumeSeparators(expression, ref i);
            while (i < expression.Length)
            {
                char c = expression[i];
                if (IsSeparator(c))
                {
                    ConsumeSeparators(expression, ref i);
                    continue;
                }

                if (char.IsDigit(c))
                {
                    if (op == null && acc != null)
                        throw UnexpectedToken(expression, i);

                    num = ConsumeNumber(expression, ref i);
                    PerformOperation(ref num, ref acc, ref op);
                    continue;
                }

                if (IsOperator(c))
                {
                    if (op != null)
                        throw UnexpectedToken(expression, i);
                    op = expression[i++];
                    continue;
                }

                throw UnexpectedToken(expression, i);
            }

            if (num != null)
            {
                PerformOperation(ref num, ref acc, ref op);
            }
            else if (op != null)
            {
                throw UnexpectedToken(expression, i);
            }

            return (int)acc;
        }

        private static bool IsOperator(char c)
        {
            return c == '+' || c == '-';
        }

        private static bool IsSeparator(char c)
        {
            return c == '\t' || c == '\r' || c == '\n' || char.IsSeparator(c);
        }

        private static void PerformOperation(ref int? num, ref int? acc, ref char? op)
        {
            int x = (int)num;
            acc = (acc ?? 0) + (op == '-' ? -x : x);
            op = null;
            num = null;
        }

        private static void ConsumeSeparators(string expression, ref int i)
        {
            while (i < expression.Length && IsSeparator(expression[i]))
                i++;
        }

        private static int ConsumeNumber(string expression, ref int i)
        {
            int num = 0;
            while (i < expression.Length && char.IsDigit(expression[i]))
                num = checked(num * 10 + (expression[i++] - '0'));
            return num;
        }

        private static Exception UnexpectedToken(string expression, int i)
        {
            int index = Math.Min(i, expression.Length - 1);
            return new FormatException($"Unexpected token '{expression[index]}' at position {index}");
        }
    }
}
