using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public class Calculator
    {
        private struct Token
        {
            public readonly int Data;
            public readonly bool IsOp;

            private Token(int data, bool isOp)
            {
                Data = data;
                IsOp = isOp;
            }

            public static Token Num(int n) => new Token(n, false);
            public static Token Op(int n) => new Token(n, true);
            public override string ToString() => IsOp ? ((char)Data).ToString() : Data.ToString();
        }

        private const string Operators = "+-*/";
        private static bool IsOperator(char c) => Operators.IndexOf(c) > -1;
        private static int OpPriotity(char c) => Operators.IndexOf(c);
        private static bool IsSeparator(char c) => c == '\t' || c == '\r' || c == '\n' || char.IsSeparator(c);

        public int Calculate(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("expression is not allowed to be blank", nameof(expression));

            List<Token> rpn = ParseIntoRpn(expression);
            return EvaluateRpn(rpn, expression);
        }

        private static List<Token> ParseIntoRpn(string expression)
        {
            char? op = null;
            int i = 0;
            int? num = null;

            var rpn = new List<Token>();
            var operators = new Stack<char>();

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
                    if (op == null && num != null)
                        throw UnexpectedToken(expression, i);

                    num = ConsumeNumber(expression, ref i);
                    rpn.Add(Token.Num((int)num));
                    op = null;
                    continue;
                }

                num = null;

                if (c == '(')
                {
                    ConsumeOpeningBrace(expression, operators, ref i);
                    continue;
                }

                if (c == ')')
                {
                    ConsumeClosingBrace(expression, operators, rpn, ref i);
                    continue;
                }

                if (IsOperator(c))
                {
                    if (op != null)
                        throw UnexpectedToken(expression, i);

                    ConsumeOperator(expression, operators, rpn, ref op, ref i);
                    continue;
                }

                throw UnexpectedToken(expression, i);
            }

            if (op != null)
            {
                throw UnexpectedToken(expression, i);
            }

            while (operators.Count > 0)
                rpn.Add(Token.Op(operators.Pop()));
            return rpn;
        }

        private static int EvaluateRpn(List<Token> rpn, string expression)
        {
            var nums = new Stack<int>();
            foreach (var token in rpn)
            {
                if (token.IsOp)
                {
                    if (nums.Count == 0)
                        throw UnexpectedToken(expression, -1);

                    int b = nums.Pop();
                    bool isUnary = nums.Count == 0;
                    int a = isUnary ? 0 : nums.Pop();
                    switch (token.Data)
                    {
                        case '+':
                            nums.Push(a + b);
                            break;
                        case '-':
                            nums.Push(a - b);
                            break;
                        case '*':
                            if (isUnary)
                                throw UnexpectedToken(expression, -1);
                            nums.Push(a * b);
                            break;
                        case '/':
                            if (isUnary)
                                throw UnexpectedToken(expression, -1);
                            nums.Push(a / b);
                            break;
                        default: throw new NotSupportedException(((char)token.Data).ToString());
                    }
                }
                else
                {
                    nums.Push(token.Data);
                }
            }

            int result = nums.Pop();
            if (nums.Count > 0)
            {
                throw UnexpectedToken(nums.Pop().ToString(), -1);
            }

            return result;
        }

        private static void ConsumeSeparators(string expression, ref int i)
        {
            while (i < expression.Length && IsSeparator(expression[i]))
                i++;
        }

        private static void ConsumeOperator(string expression, Stack<char> operators, List<Token> rpn, ref char? op, ref int i)
        {
            op = expression[i++];
            while (operators.Count > 0 && OpPriotity((char)op) <= OpPriotity(operators.Peek()))
                rpn.Add(Token.Op(operators.Pop()));

            operators.Push((char)op);
        }

        private static int ConsumeNumber(string expression, ref int i)
        {
            int num = 0;
            while (i < expression.Length && char.IsDigit(expression[i]))
                num = checked(num * 10 + (expression[i++] - '0'));
            return num;
        }

        private static void ConsumeOpeningBrace(string expression, Stack<char> operators, ref int i)
        {
            char c = expression[i];
            operators.Push(c);
            i++;
        }

        private static void ConsumeClosingBrace(string expression, Stack<char> operators, List<Token> rpn, ref int i)
        {
            while (operators.Count > 0 && operators.Peek() != '(')
                rpn.Add(Token.Op(operators.Pop()));

            if (operators.Count == 0)
                throw UnexpectedToken(expression, i);
            operators.Pop();
            i++;
        }

        private static Exception UnexpectedToken(string expression, int i)
        {
            if (i < 0)
                throw new FormatException($"Incorrectly formatted expression: ${expression}");

            int index = Math.Min(i, expression.Length - 1);
            return new FormatException($"Unexpected token '{expression[index]}' at position {index}");
        }
    }
}
