//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// More information of Gurux products: https://www.gurux.org
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------


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

        static long GetValue(string value, long sn)
        {
            if (value == "sn")
            {
                return sn;
            }
            return long.Parse(value);
        }

        /// <summary>
        /// Count serial number using formula.
        /// </summary>
        /// <param name="sn">Serial number</param>
        /// <param name="formula">Formula to used.</param>
        /// <returns></returns>
        public static long Count(long sn, string formula)
        {
            List<string> values = GetValues(FormatString(formula));
            if (values.Count % 2 == 0)
            {
                throw new ArgumentException("Invalid serial number formula.");
            }
            long total = 0;
            long value = GetValue(values[0], sn);
            for (int index = 1; index != values.Count; index += 2)
            {
                if (values[index + 1] == "sn")
                {
                    total += value;
                    value = 0;
                }
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
            total += value;
            return total;
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
