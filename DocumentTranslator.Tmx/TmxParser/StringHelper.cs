// -
// <copyright file="Str.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System.Diagnostics;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// String helper classes to improve the performance of original HtmlParser code.
    /// </summary>
    public static class Str
    {
        private static string WhiteSpaceChars = " \t\r\n";

        private static string WhiteSpaceChars1 = " \r\n\t/>";

        private static string WhiteSpaceChars2 = " \r\n\t/>=";

        public static bool IsEPT(string str, bool ignoreCase = false)
        {
            return (string.Compare(Token.EPT, str, ignoreCase) == 0) ? true : false;
        }

        public static bool IsBPT(string str, bool ignoreCase = false)
        {
            return (string.Compare(Token.BPT, str, ignoreCase) == 0) ? true : false;
        }

        public static bool IsIT(string str, bool ignoreCase = false)
        {
            return (string.Compare(Token.IT, str, ignoreCase) == 0) ? true : false;
        }

        public static bool IsPH(string str, bool ignoreCase = false)
        {
            return (string.Compare(Token.PH, str, ignoreCase) == 0) ? true : false;
        }

        public static bool IsUT(string str, bool ignoreCase = false)
        {
            return (string.Compare(Token.UT, str, ignoreCase) == 0) ? true : false;
        }

        public static bool IsLT(string str)
        {
            return (string.Compare(Token.LT, str) == 0) ? true : false;
        }

        public static bool IsLT(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.LT);
        }

        public static bool IsGT(string str)
        {
            return (string.Compare(Token.GT, str) == 0) ? true : false;
        }

        public static bool IsGT(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.GT);
        }

        public static bool IsSLGT(string str)
        {
            return (string.Compare(Token.SLGT, str) == 0) ? true : false;
        }

        public static bool IsSLGT(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.SLGT);
        }

        public static bool IsLTSL(string str)
        {
            return (string.Compare(Token.LTSL, str) == 0) ? true : false;
        }
        
        public static bool IsLTSL(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.LTSL);
        }

        public static bool IsEX(string str)
        {
            return (string.Compare(Token.EX, str) == 0) ? true : false;
        }

        public static bool IsEX(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.EX);
        }

        public static bool IsEQ(string str)
        {
            return (string.Compare(Token.EQ, str) == 0) ? true : false;
        }

        public static bool IsEQ(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.EQ);
        }

        public static bool IsSL(string str)
        {
            return (string.Compare(Token.SL, str) == 0) ? true : false;
        }

        public static bool IsSL(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.SL);
        }

        public static bool IsDQ(string str)
        {
            return (string.Compare(Token.DQ, str) == 0) ? true : false;
        }

        public static bool IsDQ(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.DQ);
        }

        public static bool IsSQ(string str)
        {
            return (string.Compare(Token.SQ, str) == 0) ? true : false;
        }

        public static bool IsSQ(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.SQ);
        }

        public static bool IsXCS(string str)
        {
            return (string.Compare(Token.XCS, str) == 0) ? true : false;
        }

        public static bool IsXCS(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.XCS);
        }
        
        public static bool IsSGMLCS(string str)
        {
            return (string.Compare(Token.SGMLCS, str) == 0) ? true : false;
        }

        public static bool IsSGMLCS(string str, int start, int length)
        {
            return SubstrCmp(str, start, length, Token.SGMLCS);
        }

        public static bool HasWhiteSpaceChars(string str)
        {
            return Str.IsIndexOfAny(str, Str.WhiteSpaceChars);
        }

        public static bool HasWhiteSpaceChars(string string1, int start, int length)
        {
            return Str.SubstrIndexofAny(string1, start, length, Str.WhiteSpaceChars);
        }

        public static bool HasWhiteSpaceChars1(string string1, int start, int length)
        {
            return Str.SubstrIndexofAny(string1, start, length, Str.WhiteSpaceChars1);
        }

        public static bool HasWhiteSpaceChars2(string string1, int start, int length)
        {
            return Str.SubstrIndexofAny(string1, start, length, Str.WhiteSpaceChars2);
        }
       
        public static bool IsEqual(string string1, string string2, bool ignoreCase=false)
        {
            return (string.Compare(string1, string2, ignoreCase) == 0) ? true : false;
        }

        public static bool IsSubStrEqual(string string1, int start, int length, string string2, bool ignoreCase=false)
        {
            if (!ignoreCase)
            {
                return SubstrCmp(string1, start, length, string2);
            }
            else
            {
                return SubstrCmpi(string1, start, length, string2);
            }
        }

        public static bool IsIndexOfAny(string string1, string string2)
        {
            return (-1 != string1.IndexOfAny(string2.ToCharArray())) ? true : false;
        }

        private static bool SubstrCmp(string string1, int start, int length, string string2)
        {
            Debug.Assert(string1.Length-start >= length);
            Debug.Assert(string2.Length >= length);
            Debug.Assert(start >= 0);
            Debug.Assert(length > 0);
                        
            int i = 0;
            while (length-- != 0)
            {
                if (string1[start++] != string2[i++])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SubstrCmpi(string string1, int start, int length, string string2)
        {
            Debug.Assert(string1.Length - start >= length);
            Debug.Assert(string2.Length >= length);
            Debug.Assert(start >= 0);
            Debug.Assert(length > 0);

            int i = 0;
            while (length-- != 0)
            {
                int first  = string1[start++];
                int second = string2[i++];

                if (first >= 'A' && first <= 'Z')
                {
                    first -= 'A' - 'a';
                }
                                
                if (second >= 'A' && second <= 'Z')
                {
                    second -= 'A' - 'a';
                }

                if (first != second)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SubstrIndexofAny(string string1, int start, int length, string string2)
        {
            while (length-- != 0)
            {
                if (-1 == string2.IndexOf(string1[start++]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
