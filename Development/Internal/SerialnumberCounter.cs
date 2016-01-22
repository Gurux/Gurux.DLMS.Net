
using System;
using System.Text;
using System.Collections.Generic;
namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// This class is used to calculate SN.
    /// </summary>
    internal class SerialnumberCounter
    {
        /// <summary>
        /// Get values to count together.
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        private static List<string> GetValues(string expressions)
        {
            List<string> values = new List<string>();
            int last = 0, index = 0;
            foreach(char ch in expressions)
            {
                switch (ch)
                {
                    case '%':
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    {
                        values.Add(expressions.Substring(last, index - last));
                        values.Add(ch.ToString());
                        last = index + 1;
                    }
                    break;
                    default:
                    break;
                }
                ++index;
            }
            if (index != last)
            {
                values.Add(expressions.Substring(last, index - last));
            }
            return values;
        }

        static int GetValue(string value, int sn)
        {
            if (value == "sn")
            {
                return sn;
            }
            return int.Parse(value);
        }

        /// <summary>
        /// Count serial number using formula.
        /// </summary>
        /// <param name="sn">Serial number</param>
        /// <param name="formula">Formula to used.</param>
        /// <returns></returns>
        public static int Count(int sn, string formula)
        {
            List<string> values = GetValues(FormatString(formula));
            if (values.Count % 2 == 0)
            {
                throw new ArgumentException("Invalid serial number formula.");
            }
            int value = GetValue(values[0], sn);
            for (int index = 1; index != values.Count; index += 2)
            {
                switch (values[index])
                {
                    case "%":
                        value = value % GetValue(values[index + 1], sn);
                        break;
                    case "+":
                        value = value + GetValue(values[index + 1], sn);
                        break;
                    case "-":
                        value = value - GetValue(values[index + 1], sn);
                        break;
                    case "*":
                        value = value * GetValue(values[index + 1], sn);
                        break;
                    case "/":
                        value = value / GetValue(values[index + 1], sn);
                        break;
                    default:
                        throw new ArgumentException("Invalid serial number formula.");
                }
            }
            return value;
        }

        /// <summary>
        /// Produce formatted string by the given math expression.
        /// </summary>
        /// <param name="expression">Unformatted math expression.</param>
        /// <returns>Formatted math expression.</returns>
        private static string FormatString(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("Expression is null or empty");
            }

            StringBuilder sb = new StringBuilder();
            foreach (char ch in expression)
            {
                if (ch == '(' || ch == ')')
                {
                    throw new ArgumentException("Invalid serial number formula.");
                }
                if (Char.IsWhiteSpace(ch))
                {
                    continue;
                }
                else if (Char.IsUpper(ch))
                {
                    sb.Append(Char.ToLower(ch));
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }        
    }
}
