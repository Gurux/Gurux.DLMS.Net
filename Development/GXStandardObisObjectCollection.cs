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
using System.Collections.Generic;
using System.Linq;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// Obis Code Collection is used to save all default OBIS Codes.
    /// </summary>
    class GXStandardObisCodeCollection : List<GXStandardObisCode>
    {
        /// <summary>
        /// Convert logican name string to bytes.
        /// </summary>
        /// <param name="ln"></param>
        /// <returns></returns>
        static byte[] GetBytes(string ln)
        {
            if (String.IsNullOrEmpty(ln))
            {
                return null;
            }
            string[] tmp = ln.Split(new char[] { '.' });
            if (tmp.Length != 6)
            {
                // If value is give as hex.
                byte[] tmp2 = GXCommon.HexToBytes(ln);
                if (tmp2.Length == 6)
                {
                    return new byte[] { tmp2[0], tmp2[1],
                        tmp2[2], tmp2[3], tmp2[4],
                        tmp2[5] };
                }
                throw new ArgumentException("Invalid OBIS Code.");
            }
            byte[] code = new byte[6];
            code[0] = byte.Parse(tmp[0]);
            code[1] = byte.Parse(tmp[1]);
            code[2] = byte.Parse(tmp[2]);
            code[3] = byte.Parse(tmp[3]);
            code[4] = byte.Parse(tmp[4]);
            code[5] = byte.Parse(tmp[5]);
            return code;
        }

        public GXStandardObisCode[] Find(string obisCode, ObjectType objectType, Standard standard)
        {
            return Find(GetBytes(obisCode), (int)objectType, standard);
        }

        /// <summary>
        /// Check is interface included to standard.
        /// </summary>
        static bool EqualsInterface(GXStandardObisCode it, int ic)
        {
            //If all interfaces are allowed.
            if (ic == 0 || it.Interfaces == "*")
            {
                return true;
            }
            return it.Interfaces.Split(new char[] { ',' }).Contains(ic.ToString());
        }

        /// <summary>
        /// Check is obis code item equal to mask.
        /// </summary>
        static bool EqualsMask(string obisMask, byte ic)
        {
            bool number = true;
            if (obisMask.IndexOf(',') != -1)
            {
                string[] tmp = obisMask.Split(new char[] { ',' });
                foreach (string it in tmp)
                {
                    if (it.IndexOf('-') != -1)
                    {
                        if (EqualsMask(it, ic))
                        {
                            return true;
                        }
                    }
                    else if (byte.Parse(it) == ic)
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (obisMask.IndexOf('-') != -1)
            {
                number = false;
                string[] tmp = obisMask.Split(new char[] { '-' });
                return ic >= byte.Parse(tmp[0]) && ic <= byte.Parse(tmp[1]);
            }
            if (number)
            {
                if (obisMask == "&")
                {
                    return ic == 0 || ic == 1 || ic == 7;
                }
                return byte.Parse(obisMask) == ic;
            }
            return false;
        }

        static public bool EqualsMask(string obisMask, string ln)
        {
            return EqualsObisCode(obisMask.Split('.'), GetBytes(ln));
        }

        /// <summary>
        /// Check OBIS code.
        /// </summary>
        static bool EqualsObisCode(string[] obisMask, byte[] ic)
        {
            if (ic == null)
            {
                return true;
            }
            if (obisMask.Length != 6)
            {
                throw new ArgumentException("Invalid OBIS mask.");
            }
            if (!EqualsMask(obisMask[0], ic[0]))
            {
                return false;
            }
            if (!EqualsMask(obisMask[1], ic[1]))
            {
                return false;
            }
            if (!EqualsMask(obisMask[2], ic[2]))
            {
                return false;
            }
            if (!EqualsMask(obisMask[3], ic[3]))
            {
                return false;
            }
            if (!EqualsMask(obisMask[4], ic[4]))
            {
                return false;
            }
            if (!EqualsMask(obisMask[5], ic[5]))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Get N1C description.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string GetN1CDescription(string str)
        {
            if (string.IsNullOrEmpty(str) || str[0] != '$')
            {
                return "";
            }
            int value = int.Parse(str.Substring(1));
            string tmp = "";
            switch (value)
            {
                case 41:
                    tmp = "Absolute temperature";
                    break;
                case 42:
                    tmp = "Absolute pressure";
                    break;
                case 44:
                    tmp = "Velocity of sound";
                    break;
                case 45:
                    tmp = "Density(of gas)";
                    break;
                case 46:
                    tmp = "Relative density";
                    break;
                case 47:
                    tmp = "Gauge pressure";
                    break;
                case 48:
                    tmp = "Differential pressure";
                    break;
                case 49:
                    tmp = "Density of air";
                    break;
            }
            return tmp;
        }

        /// <summary>
        /// Get description.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string GetDescription(string str)
        {
            if (string.IsNullOrEmpty(str) || str[0] != '$')
            {
                return "";
            }
            int value = int.Parse(str.Substring(1));
            switch (value)
            {
                case 1:
                    return "Sum Li Active power+ (QI+QIV)";
                case 2:
                    return "Sum Li Active power- (QII+QIII)";
                case 3:
                    return "Sum Li Reactive power+ (QI+QII)";
                case 4:
                    return "Sum Li Reactive power- (QIII+QIV)";
                case 5:
                    return "Sum Li Reactive power QI";
                case 6:
                    return "Sum Li Reactive power QII";
                case 7:
                    return "Sum Li Reactive power QIII";
                case 8:
                    return "Sum Li Reactive power QIV";
                case 9:
                    return "Sum Li Apparent power+ (QI+QIV)";
                case 10:
                    return "Sum Li Apparent power- (QII+QIII)";
                case 11:
                    return "Current: any phase";
                case 12:
                    return "Voltage: any phase";
                case 13:
                    return "Sum Li Power factor";
                case 14:
                    return "Supply frequency";
                case 15:
                    return "Sum Li Active power (abs(QI+QIV)+abs(QII+QIII))";
                case 16:
                    return "Sum Li Active power        (abs(QI+QIV)-abs(QII+QIII))";
                case 17:
                    return "Sum Li Active power QI";
                case 18:
                    return "Sum Li Active power QII";
                case 19:
                    return "Sum Li Active power QIII";
                case 20:
                    return "Sum Li Active power QIV";
                case 21:
                    return "L1 Active power+ (QI+QIV)";
                case 22:
                    return "L1 Active power- (QII+QIII)";
                case 23:
                    return "L1 Reactive power+ (QI+QII)";
                case 24:
                    return "L1 Reactive power- (QIII+QIV)";
                case 25:
                    return "L1 Reactive power QI";
                case 26:
                    return "L1 Reactive power QII";
                case 27:
                    return "L1 Reactive power QIII";
                case 28:
                    return "L1 Reactive power QIV";
                case 29:
                    return "L1 Apparent power+ (QI+QIV)";
                case 30:
                    return "L1 Apparent power- (QII+QIII)";
                case 31:
                    return "L1 Current";
                case 32:
                    return "L1 Voltage";
                case 33:
                    return "L1 Power factor";
                case 34:
                    return "L1 Supply frequency";
                case 35:
                    return "L1 Active power (abs(QI+QIV)+abs(QII+QIII))";
                case 36:
                    return "L1 Active power (abs(QI+QIV)-abs(QII+QIII))";
                case 37:
                    return "L1 Active power QI";
                case 38:
                    return "L1 Active power QII";
                case 39:
                    return "L1 Active power QIII";
                case 40:
                    return "L1 Active power QIV";
                case 41:
                    return "L2 Active power+ (QI+QIV)";
                case 42:
                    return "L2 Active power- (QII+QIII)";
                case 43:
                    return "L2 Reactive power+ (QI+QII)";
                case 44:
                    return "L2 Reactive power- (QIII+QIV)";
                case 45:
                    return "L2 Reactive power QI";
                case 46:
                    return "L2 Reactive power QII";
                case 47:
                    return "L2 Reactive power QIII";
                case 48:
                    return "L2 Reactive power QIV";
                case 49:
                    return "L2 Apparent power+ (QI+QIV)";
                case 50:
                    return "L2 Apparent power- (QII+QIII)";
                case 51:
                    return "L2 Current";
                case 52:
                    return "L2 Voltage";
                case 53:
                    return "L2 Power factor";
                case 54:
                    return "L2 Supply frequency";
                case 55:
                    return "L2 Active power (abs(QI+QIV)+abs(QII+QIII))";
                case 56:
                    return "L2 Active power (abs(QI+QIV)-abs(QI+QIII))";
                case 57:
                    return "L2 Active power QI";
                case 58:
                    return "L2 Active power QII";
                case 59:
                    return "L2 Active power QIII";
                case 60:
                    return "L2 Active power QIV";
                case 61:
                    return "L3 Active power+ (QI+QIV)";
                case 62:
                    return "L3 Active power- (QII+QIII)";
                case 63:
                    return "L3 Reactive power+ (QI+QII)";
                case 64:
                    return "L3 Reactive power- (QIII+QIV)";
                case 65:
                    return "L3 Reactive power QI";
                case 66:
                    return "L3 Reactive power QII";
                case 67:
                    return "L3 Reactive power QIII";
                case 68:
                    return "L3 Reactive power QIV";
                case 69:
                    return "L3 Apparent power+ (QI+QIV)";
                case 70:
                    return "L3 Apparent power- (QII+QIII)";
                case 71:
                    return "L3 Current";
                case 72:
                    return "L3 Voltage";
                case 73:
                    return "L3 Power factor";
                case 74:
                    return "L3 Supply frequency";
                case 75:
                    return "L3 Active power (abs(QI+QIV)+abs(QII+QIII))";
                case 76:
                    return "L3 Active power (abs(QI+QIV)-abs(QI+QIII))";
                case 77:
                    return "L3 Active power QI";
                case 78:
                    return "L3 Active power QII";
                case 79:
                    return "L3 Active power QIII";
                case 80:
                    return "L3 Active power QIV";
                case 82:
                    return "Unitless quantities (pulses or pieces)";
                case 84:
                    return "Sum Li Power factor-";
                case 85:
                    return "L1 Power factor-";
                case 86:
                    return "L2 Power factor-";
                case 87:
                    return "L3 Power factor-";
                case 88:
                    return "Sum Li A2h QI+QII+QIII+QIV";
                case 89:
                    return "Sum Li V2h QI+QII+QIII+QIV";
                case 90:
                    return "SLi current (algebraic sum of the – unsigned – value of the currents in all phases)";
                case 91:
                    return "Lo Current (neutral)";
                case 92:
                    return "Lo Voltage (neutral)";
            }
            return "";
        }

        /// <summary>
        /// Get OBIS value
        /// </summary>
        /// <param name="formula">OBIS formula.</param>
        /// <param name="value">OBIS value.</param>
        /// <returns></returns>
        private static string GetObisValue(string formula, int value)
        {
            if (formula.Length == 1)
            {
                return value.ToString();
            }
            return (value + int.Parse(formula.Substring(1))).ToString();
        }

        /// <summary>
        /// Find Standard OBIS Code description.
        /// </summary>
        public GXStandardObisCode[] Find(byte[] obisCode, int IC, Standard standard)
        {
            GXStandardObisCode tmp;
            List<GXStandardObisCode> list = new List<GXStandardObisCode>();
            foreach (GXStandardObisCode it in this)
            {
                //Interface is tested first because it's faster.
                if (EqualsInterface(it, IC) && EqualsObisCode(it.OBIS, obisCode))
                {
                    tmp = new GXStandardObisCode(it.OBIS, it.Description, it.Interfaces, it.DataType);
                    tmp.UIDataType = it.UIDataType;
                    list.Add(tmp);
                    string[] tmp2 = it.Description.Split(new char[] { ';' });
                    if (tmp2.Length > 1)
                    {
                        string desc = "";
                        if (obisCode != null && string.Compare("$1", tmp2[1].Trim()) == 0)
                        {
                            if (obisCode[0] == 7)
                            {
                                desc = GetN1CDescription("$" + obisCode[2]);
                            }
                            else
                            {
                                desc = GetDescription("$" + obisCode[2]);
                            }
                        }
                        if (desc != "")
                        {
                            tmp2[1] = desc;
                            tmp.Description = string.Join(";", tmp2);
                        }
                    }
                    if (obisCode != null)
                    {
                        tmp.OBIS[0] = obisCode[0].ToString();
                        tmp.OBIS[1] = obisCode[1].ToString();
                        tmp.OBIS[2] = obisCode[2].ToString();
                        tmp.OBIS[3] = obisCode[3].ToString();
                        tmp.OBIS[4] = obisCode[4].ToString();
                        tmp.OBIS[5] = obisCode[5].ToString();
                        tmp.Description = tmp.Description.Replace("$A", obisCode[0].ToString());
                        tmp.Description = tmp.Description.Replace("$B", obisCode[1].ToString());
                        tmp.Description = tmp.Description.Replace("$C", obisCode[2].ToString());
                        tmp.Description = tmp.Description.Replace("$D", obisCode[3].ToString());
                        tmp.Description = tmp.Description.Replace("$E", obisCode[4].ToString());
                        tmp.Description = tmp.Description.Replace("$F", obisCode[5].ToString());
                        //Increase value
                        int begin = tmp.Description.IndexOf("$(");
                        if (begin != -1)
                        {
                            string[] arr = tmp.Description.Substring(begin + 2).Split(new char[] { '(', ')', '$' }, StringSplitOptions.RemoveEmptyEntries);
                            tmp.Description = tmp.Description.Substring(0, begin);
                            foreach (string v in arr)
                            {
                                switch (v[0])
                                {
                                    case 'A':
                                        tmp.Description += GetObisValue(v, obisCode[0]);
                                        break;
                                    case 'B':
                                        tmp.Description += GetObisValue(v, obisCode[1]);
                                        break;
                                    case 'C':
                                        tmp.Description += GetObisValue(v, obisCode[2]);
                                        break;
                                    case 'D':
                                        tmp.Description += GetObisValue(v, obisCode[3]);
                                        break;
                                    case 'E':
                                        tmp.Description += GetObisValue(v, obisCode[4]);
                                        break;
                                    case 'F':
                                        tmp.Description += GetObisValue(v, obisCode[5]);
                                        break;
                                    default:
                                        tmp.Description += v;
                                        break;
                                }
                            }
                        }
                        tmp.Description = tmp.Description.Replace(';', ' ').Replace("  ", " ").Trim();
                    }
                }
            }
            //If invalid OBIS code.
            if (list.Count == 0)
            {
                tmp = new GXStandardObisCode(null, "Invalid", IC.ToString(), "");
                list.Add(tmp);
                tmp.OBIS[0] = obisCode[0].ToString();
                tmp.OBIS[1] = obisCode[1].ToString();
                tmp.OBIS[2] = obisCode[2].ToString();
                tmp.OBIS[3] = obisCode[3].ToString();
                tmp.OBIS[4] = obisCode[4].ToString();
                tmp.OBIS[5] = obisCode[5].ToString();
            }
            return list.ToArray();
        }
    }
}
