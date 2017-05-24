using System;
using System.Text;
using NUnit.Framework;

namespace ClassLibrary1
{
    [TestFixture]
    public class CalculatorTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("\t  \n")]
        public void Throws_exception_for_empty_input(string expression)
        {
            try
            {
                new Calculator().Calculate(expression);
                Assert.Fail("Expected exception was not thrown");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }
        }

        [TestCase("0", 0)]
        [TestCase("+0", 0)]
        [TestCase("-0", 0)]
        [TestCase("-0000", 0)]
        [TestCase("1", 1)]
        [TestCase("123", 123)]
        [TestCase("+123", 123)]
        [TestCase("-123", -123)]
        public void Returns_just_the_number_for_expression_with_single_number_only(string expression, int expected)
        {
            Assert.AreEqual(expected, new Calculator().Calculate(expression));
        }

        [TestCase("1+2", 3)]
        [TestCase("1+000", 1)]
        [TestCase("1+2+3", 6)]
        [TestCase("+1+2+3", 6)]
        [TestCase("-1+2+3", 4)]
        [TestCase("4+5+6", 15)]
        [TestCase("-1+2+3-1", 3)]
        [TestCase("-11+22+33-111", -67)]
        [TestCase("-11+22+33-111", -67)]
        public void Calculates_simple_expressions(string expression, int expected)
        {
            Assert.AreEqual(expected, new Calculator().Calculate(expression));
        }

        [TestCase("  - 11 + \n \n \t   22 + 33   \t   	    -   111      ", -67)]
        public void Ignores_whitespaces_tabs_and_newlines(string expression, int expected)
        {
            Assert.AreEqual(expected, new Calculator().Calculate(expression));
        }

        [TestCase("-11+0+22-0+33-111", -67)]
        [TestCase("0-11+22+33-111", -67)]
        [TestCase("-0-11+22+33-111", -67)]
        [TestCase("-11+22+33-111+0-0+0-0+0-0+0-0+0-0+0-0+0-0+0-0+0-0+0-0+0-0+0-0", -67)]
        public void Calculates_expressions_with_zeros(string expression, int expected)
        {
            Assert.AreEqual(expected, new Calculator().Calculate(expression));
        }

        [TestCase("2147483648")]// int.MaxValue+1
        [TestCase("9999999999")]
        [TestCase("99999999999")]
        [TestCase("99999999999")]
        public void Throws_when_single_number_is_too_big(string expression)
        {
            try
            {
                new Calculator().Calculate(expression);
                Assert.Fail("Expected exception was not thrown");
            }
            catch (OverflowException e)
            {
                Console.WriteLine(e);
            }
        }

        [Test]
        public void Straightly_sums_up_when_sum_is_too_big()
        {
            int a = int.MaxValue;
            Assert.AreEqual(a + 1, new Calculator().Calculate(int.MaxValue + "+1"));
        }

        [TestCase("+")]
        [TestCase("-")]
        [TestCase("--")]
        [TestCase("-+")]
        [TestCase("1++")]
        [TestCase("1++2")]
        [TestCase("1+")]
        [TestCase("1+2;")]
        [TestCase("1*2")]
        [TestCase("1/2")]
        [TestCase("1-")]
        [TestCase("1-2-")]
        [TestCase("1-2-2 2")]
        [TestCase("1+2p")]
        [TestCase("1+2 p")]
        [TestCase("asfd")]
        public void Throws_format_exception_for_invalid_input(string expression)
        {
            try
            {
                new Calculator().Calculate(expression);
                Assert.Fail("Expected exception was not thrown");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e);
            }
        }

        [Test]
        public void Calculate_long_expression()
        {
            int n = short.MaxValue * 10;
            var sb = new StringBuilder("11");
            for (int i = 0; i < n; i++)
                sb.Append("+11");

            Assert.AreEqual(11 + 11 * n, new Calculator().Calculate(sb.ToString()));
        }
    }
}