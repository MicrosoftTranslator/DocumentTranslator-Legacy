// -
// <copyright file="TmxEncoder.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.IO;
using System.Text;

/*
* This class contains valid code. Commented for code coverage purpose.
* 
namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// HTML 4 Entity coding routines.
    /// </summary>
    public abstract class TmxEncoder
    {

        /// <summary>
        /// This function performs encoding rules on the given string.
        /// </summary>
        /// <param name="value">String which required to encode.</param>
        /// <returns>Encoded value.</returns>
        public static string EncodeValue(string value)
        {
            StringBuilder output = new StringBuilder();
            using (StringReader reader = new StringReader(value))
            {
                int c = reader.Read();
                while (c != -1)
                {
                    switch (c)
                    {
                        case '<':
                            output.Append("&lt;");
                            break;
                        case '>':
                            output.Append("&gt;");
                            break;
                        case '\"':
                            output.Append("&quot;");
                            break;
                        case '&':
                            output.Append("&amp;");
                            break;
                        case 193:
                            output.Append("&Aacute;");
                            break;
                        case 225:
                            output.Append("&aacute;");
                            break;
                        case 194:
                            output.Append("&Acirc;");
                            break;
                        case 226:
                            output.Append("&acirc;");
                            break;
                        case 180:
                            output.Append("&acute;");
                            break;
                        case 198:
                            output.Append("&AElig;");
                            break;
                        case 230:
                            output.Append("&aelig;");
                            break;
                        case 192:
                            output.Append("&Agrave;");
                            break;
                        case 224:
                            output.Append("&agrave;");
                            break;
                        case 8501:
                            output.Append("&alefsym;");
                            break;
                        case 913:
                            output.Append("&Alpha;");
                            break;
                        case 945:
                            output.Append("&alpha;");
                            break;

                        case 8743:
                            output.Append("&and;");
                            break;
                        case 8736:
                            output.Append("&ang;");
                            break;
                        case 197:
                            output.Append("&Aring;");
                            break;
                        case 229:
                            output.Append("&aring;");
                            break;
                        case 8776:
                            output.Append("&asymp;");
                            break;
                        case 195:
                            output.Append("&Atilde;");
                            break;
                        case 227:
                            output.Append("&atilde;");
                            break;
                        case 196:
                            output.Append("&Auml;");
                            break;
                        case 228:
                            output.Append("&auml;");
                            break;
                        case 8222:
                            output.Append("&bdquo;");
                            break;
                        case 914:
                            output.Append("&Beta;");
                            break;
                        case 946:
                            output.Append("&beta;");
                            break;
                        case 166:
                            output.Append("&brvbar;");
                            break;
                        case 8226:
                            output.Append("&bull;");
                            break;
                        case 8745:
                            output.Append("&cap;");
                            break;
                        case 199:
                            output.Append("&Ccedil;");
                            break;
                        case 231:
                            output.Append("&ccedil;");
                            break;
                        case 184:
                            output.Append("&cedil;");
                            break;
                        case 162:
                            output.Append("&cent;");
                            break;
                        case 935:
                            output.Append("&Chi;");
                            break;
                        case 967:
                            output.Append("&chi;");
                            break;
                        case 710:
                            output.Append("&circ;");
                            break;
                        case 9827:
                            output.Append("&clubs;");
                            break;
                        case 8773:
                            output.Append("&cong;");
                            break;
                        ////case 169:
                        ////    output.Append("&copy;");
                        ////    break;
                        case 8629:
                            output.Append("&crarr;");
                            break;
                        case 8746:
                            output.Append("&cup;");
                            break;
                        case 164:
                            output.Append("&curren;");
                            break;
                        case 8224:
                            output.Append("&dagger;");
                            break;
                        case 8225:
                            output.Append("&Dagger;");
                            break;
                        case 8595:
                            output.Append("&darr;");
                            break;
                        case 8659:
                            output.Append("&dArr;");
                            break;
                        case 176:
                            output.Append("&deg;");
                            break;
                        case 916:
                            output.Append("&Delta;");
                            break;
                        case 948:
                            output.Append("&delta;");
                            break;
                        case 9830:
                            output.Append("&diams;");
                            break;
                        case 247:
                            output.Append("&divide;");
                            break;
                        case 201:
                            output.Append("&Eacute;");
                            break;
                        case 233:
                            output.Append("&eacute;");
                            break;
                        case 202:
                            output.Append("&Ecirc;");
                            break;
                        case 234:
                            output.Append("&ecirc;");
                            break;

                        case 200:
                            output.Append("&Egrave;");
                            break;
                        case 232:
                            output.Append("&egrave;");
                            break;
                        case 8709:
                            output.Append("&empty;");
                            break;
                        case 8195:
                            output.Append("&emsp;");
                            break;
                        case 917:
                            output.Append("&Epsilon;");
                            break;
                        case 949:
                            output.Append("&epsilon;");
                            break;
                        case 8801:
                            output.Append("&equiv;");
                            break;
                        case 919:
                            output.Append("&Eta;");
                            break;
                        case 951:
                            output.Append("&eta;");
                            break;
                        case 208:
                            output.Append("&ETH;");
                            break;
                        case 240:
                            output.Append("&eth;");
                            break;
                        case 203:
                            output.Append("&Euml;");
                            break;
                        case 235:
                            output.Append("&euml;");
                            break;
                        case 128:
                            output.Append("&euro;");
                            break;
                        case 8707:
                            output.Append("&exist;");
                            break;
                        case 402:
                            output.Append("&fnof;");
                            break;
                        case 8704:
                            output.Append("&forall;");
                            break;
                        case 189:
                            output.Append("&frac12;");
                            break;
                        case 188:
                            output.Append("&frac14;");
                            break;
                        case 190:
                            output.Append("&frac34;");
                            break;
                        case 8260:
                            output.Append("&fras1;");
                            break;
                        case 915:
                            output.Append("&Gamma;");
                            break;
                        case 947:
                            output.Append("&gamma;");
                            break;
                        case 8805:
                            output.Append("&ge;");
                            break;
                        case 8596:
                            output.Append("&harr;");
                            break;
                        case 8660:
                            output.Append("&hArr;");
                            break;
                        case 9829:
                            output.Append("&hearts;");
                            break;
                        case 8230:
                            output.Append("&hellip;");
                            break;
                        case 205:
                            output.Append("&Iacute;");
                            break;
                        case 237:
                            output.Append("&iacute;");
                            break;
                        case 206:
                            output.Append("&Icirc;");
                            break;
                        case 238:
                            output.Append("&icirc;");
                            break;
                        case 161:
                            output.Append("&iexcl;");
                            break;
                        case 204:
                            output.Append("&Igrave;");
                            break;
                        case 236:
                            output.Append("&igrave;");
                            break;
                        case 8465:
                            output.Append("&image;");
                            break;
                        case 8734:
                            output.Append("&infin;");
                            break;
                        case 8747:
                            output.Append("&int;");
                            break;
                        case 921:
                            output.Append("&Iota;");
                            break;

                        case 953:
                            output.Append("&iota;");
                            break;
                        case 191:
                            output.Append("&iquest;");
                            break;
                        case 8712:
                            output.Append("&isin;");
                            break;
                        case 207:
                            output.Append("&Iuml;");
                            break;
                        case 239:
                            output.Append("&iuml;");
                            break;
                        case 922:
                            output.Append("&Kappa;");
                            break;
                        case 954:
                            output.Append("&kappa;");
                            break;
                        case 923:
                            output.Append("&Lambda;");
                            break;
                        case 955:
                            output.Append("&lambda;");
                            break;
                        case 9001:
                            output.Append("&lang;");
                            break;
                        case 171:
                            output.Append("&laquo;");
                            break;
                        case 8592:
                            output.Append("&larr;");
                            break;
                        case 8656:
                            output.Append("&lArr;");
                            break;
                        case 8968:
                            output.Append("&lceil;");
                            break;
                        case 8220:
                            output.Append("&ldquo;");
                            break;
                        case 8804:
                            output.Append("&le;");
                            break;
                        case 8970:
                            output.Append("&lfloor;");
                            break;
                        case 8727:
                            output.Append("&lowast;");
                            break;
                        case 9674:
                            output.Append("&loz;");
                            break;
                        case 8206:
                            output.Append("&lrm;");
                            break;
                        case 8249:
                            output.Append("&lsaquo;");
                            break;
                        case 8216:
                            output.Append("&lsquo;");
                            break;
                        case 175:
                            output.Append("&macr;");
                            break;
                        ////case 8212:
                        ////    output.Append("&mdash;");
                        ////    break;
                        case 181:
                            output.Append("&micro;");
                            break;
                        case 183:
                            output.Append("&middot;");
                            break;
                        case 8722:
                            output.Append("&minus;");
                            break;
                        case 924:
                            output.Append("&Mu;");
                            break;
                        case 956:
                            output.Append("&mu;");
                            break;
                        case 8711:
                            output.Append("&nabla;");
                            break;
                        case 160:
                            output.Append("&nbsp;");
                            break;
                        case 8211:
                            output.Append("&ndash;");
                            break;
                        case 8800:
                            output.Append("&ne;");
                            break;
                        case 8715:
                            output.Append("&ni;");
                            break;
                        case 172:
                            output.Append("&not;");
                            break;
                        case 8713:
                            output.Append("&notin;");
                            break;
                        case 8836:
                            output.Append("&nsub;");
                            break;
                        case 209:
                            output.Append("&Ntilde;");
                            break;
                        case 241:
                            output.Append("&ntilde;");
                            break;
                        case 925:
                            output.Append("&Nu;");
                            break;

                        case 957:
                            output.Append("&nu;");
                            break;
                        case 211:
                            output.Append("&Oacute;");
                            break;
                        case 243:
                            output.Append("&oacute;");
                            break;
                        case 212:
                            output.Append("&Ocirc;");
                            break;
                        case 244:
                            output.Append("&ocirc;");
                            break;
                        case 338:
                            output.Append("&OElig;");
                            break;
                        case 339:
                            output.Append("&oelig;");
                            break;
                        case 210:
                            output.Append("&Ograve;");
                            break;
                        case 242:
                            output.Append("&ograve;");
                            break;
                        case 8254:
                            output.Append("&oline;");
                            break;
                        case 937:
                            output.Append("&Omega;");
                            break;
                        case 969:
                            output.Append("&omega;");
                            break;
                        case 927:
                            output.Append("&Omicron;");
                            break;
                        case 959:
                            output.Append("&omicron;");
                            break;
                        case 8853:
                            output.Append("&oplus;");
                            break;
                        case 8744:
                            output.Append("&or;");
                            break;
                        case 170:
                            output.Append("&ordf;");
                            break;
                        case 186:
                            output.Append("&ordm;");
                            break;
                        case 216:
                            output.Append("&Oslash;");
                            break;
                        case 248:
                            output.Append("&oslash;");
                            break;
                        case 213:
                            output.Append("&Otilde;");
                            break;
                        case 245:
                            output.Append("&otilde;");
                            break;
                        case 8855:
                            output.Append("&otimes;");
                            break;
                        case 214:
                            output.Append("&Ouml;");
                            break;
                        case 246:
                            output.Append("&ouml;");
                            break;
                        case 182:
                            output.Append("&para;");
                            break;
                        case 8706:
                            output.Append("&part;");
                            break;
                        case 8240:
                            output.Append("&permil;");
                            break;
                        case 8869:
                            output.Append("&perp;");
                            break;
                        case 934:
                            output.Append("&Phi;");
                            break;
                        case 966:
                            output.Append("&phi;");
                            break;
                        case 928:
                            output.Append("&Pi;");
                            break;
                        case 960:
                            output.Append("&pi;");
                            break;
                        case 982:
                            output.Append("&piv;");
                            break;
                        case 177:
                            output.Append("&plusmn;");
                            break;
                        case 163:
                            output.Append("&pound;");
                            break;
                        case 8242:
                            output.Append("&prime;");
                            break;
                        case 8243:
                            output.Append("&Prime;");
                            break;
                        case 8719:
                            output.Append("&prod;");
                            break;
                        case 8733:
                            output.Append("&prop;");
                            break;
                        case 936:
                            output.Append("&Psi;");
                            break;

                        case 968:
                            output.Append("&psi;");
                            break;
                        case 8730:
                            output.Append("&radic;");
                            break;
                        case 9002:
                            output.Append("&rang;");
                            break;
                        case 187:
                            output.Append("&raquo;");
                            break;
                        case 8594:
                            output.Append("&rarr;");
                            break;
                        case 8658:
                            output.Append("&rArr;");
                            break;
                        case 8969:
                            output.Append("&rceil;");
                            break;
                        case 8221:
                            output.Append("&rdquo;");
                            break;
                        case 8476:
                            output.Append("&real;");
                            break;
                        case 174:
                            output.Append("&reg;");
                            break;
                        case 8971:
                            output.Append("&rfloor;");
                            break;
                        case 929:
                            output.Append("&Rho;");
                            break;
                        case 961:
                            output.Append("&rho;");
                            break;
                        case 8207:
                            output.Append("&rlm;");
                            break;
                        case 8250:
                            output.Append("&rsaquo;");
                            break;
                        case 8217:
                            output.Append("&rsquo;");
                            break;
                        case 8218:
                            output.Append("&sbquo;");
                            break;
                        case 352:
                            output.Append("&Scaron;");
                            break;
                        case 353:
                            output.Append("&scaron;");
                            break;
                        case 8901:
                            output.Append("&sdot;");
                            break;
                        case 167:
                            output.Append("&sect;");
                            break;
                        case 173:
                            output.Append("&shy;");
                            break;
                        case 931:
                            output.Append("&Sigma;");
                            break;
                        case 963:
                            output.Append("&sigma;");
                            break;
                        case 962:
                            output.Append("&sigmaf;");
                            break;
                        case 8764:
                            output.Append("&sim;");
                            break;
                        case 9824:
                            output.Append("&spades;");
                            break;
                        case 8834:
                            output.Append("&sub;");
                            break;
                        case 8838:
                            output.Append("&sube;");
                            break;
                        case 8721:
                            output.Append("&sum;");
                            break;
                        case 8835:
                            output.Append("&sup;");
                            break;
                        case 185:
                            output.Append("&sup1;");
                            break;
                        case 178:
                            output.Append("&sup2;");
                            break;
                        case 179:
                            output.Append("&sup3;");
                            break;
                        case 8839:
                            output.Append("&supe;");
                            break;
                        case 223:
                            output.Append("&szlig;");
                            break;
                        case 932:
                            output.Append("&Tau;");
                            break;
                        case 964:
                            output.Append("&tau;");
                            break;
                        case 8756:
                            output.Append("&there4;");
                            break;
                        case 920:
                            output.Append("&Theta;");
                            break;

                        case 952:
                            output.Append("&theta;");
                            break;
                        case 977:
                            output.Append("&thetasym;");
                            break;
                        case 8201:
                            output.Append("&thinsp;");
                            break;
                        case 222:
                            output.Append("&THORN;");
                            break;
                        case 254:
                            output.Append("&thorn;");
                            break;
                        case 732:
                            output.Append("&tilde;");
                            break;
                        case 215:
                            output.Append("&times;");
                            break;
                        case 8482:
                            output.Append("&trade;");
                            break;
                        case 218:
                            output.Append("&Uacute;");
                            break;
                        case 250:
                            output.Append("&uacute;");
                            break;
                        case 8593:
                            output.Append("&uarr;");
                            break;
                        case 8657:
                            output.Append("&uArr;");
                            break;
                        case 219:
                            output.Append("&Ucirc;");
                            break;
                        case 251:
                            output.Append("&ucirc;");
                            break;
                        case 217:
                            output.Append("&Ugrave;");
                            break;
                        case 249:
                            output.Append("&ugrave;");
                            break;
                        case 168:
                            output.Append("&uml;");
                            break;
                        case 978:
                            output.Append("&upsih;");
                            break;
                        case 933:
                            output.Append("&Upsilon;");
                            break;
                        case 965:
                            output.Append("&upsilon;");
                            break;
                        case 220:
                            output.Append("&Uuml;");
                            break;
                        case 252:
                            output.Append("&uuml;");
                            break;
                        case 8472:
                            output.Append("&weierp;");
                            break;
                        case 926:
                            output.Append("&Xi;");
                            break;
                        case 958:
                            output.Append("&xi;");
                            break;
                        case 221:
                            output.Append("&Yacute;");
                            break;
                        case 253:
                            output.Append("&yacute;");
                            break;
                        case 165:
                            output.Append("&yen;");
                            break;
                        case 376:
                            output.Append("&Yuml;");
                            break;
                        case 255:
                            output.Append("&yuml;");
                            break;
                        case 918:
                            output.Append("&Zeta;");
                            break;
                        case 950:
                            output.Append("&zeta;");
                            break;
                        case 8205:
                            output.Append("&zwj;");
                            break;
                        case 8204:
                            output.Append("&zwnj;");
                            break;
                        default:
                            ////if (c <= 127)
                            ////{
                            output.Append((char)c);
                            ////}
                            //// else
                            ////{
                            //// output.Append("&#" + c + ";");
                            ////}
                            break;
                    }

                    c = reader.Read();
                }
            }

            return output.ToString();
        }

        public static string DecodeValue(string value)
        {
            StringBuilder output = new StringBuilder();
            using (StringReader reader = new StringReader(value))
            {
                StringBuilder token;
                int c = reader.Read();
                while (c != -1)
                {
                    token = new StringBuilder();

                    while (c != '&' && c != -1)
                    {
                        token.Append((char)c);
                        c = reader.Read();
                    }

                    output.Append(token.ToString());

                    if (c == '&')
                    {
                        token = new StringBuilder();

                        while (c != ';' && c != -1)
                        {
                            token.Append((char)c);
                            c = reader.Read();
                        }

                        if (c == ';')
                        {
                            c = reader.Read();
                            token.Append(';');

                            if (token[1] == '#')
                            {
                                int v = int.Parse(token.ToString().Substring(2, token.Length - 3));
                                output.Append((char)v);
                            }
                            else
                            {
                                switch (token.ToString())
                                {
                                    case "&lt;":
                                        output.Append("<");
                                        break;
                                    case "&gt;":
                                        output.Append(">");
                                        break;
                                    case "&quot;":
                                        output.Append("\"");
                                        break;
                                    case "&amp;":
                                        output.Append("&");
                                        break;
                                    case "&apos;":
                                        output.Append("'");
                                        break;
                                    case "&Aacute;":
                                        output.Append((char)193);
                                        break;
                                    case "&aacute;":
                                        output.Append((char)225);
                                        break;
                                    case "&Acirc;":
                                        output.Append((char)194);
                                        break;
                                    case "&acirc;":
                                        output.Append((char)226);
                                        break;
                                    case "&acute;":
                                        output.Append((char)180);
                                        break;
                                    case "&AElig;":
                                        output.Append((char)198);
                                        break;
                                    case "&aelig;":
                                        output.Append((char)230);
                                        break;
                                    case "&Agrave;":
                                        output.Append((char)192);
                                        break;
                                    case "&agrave;":
                                        output.Append((char)224);
                                        break;
                                    case "&alefsym;":
                                        output.Append((char)8501);
                                        break;
                                    case "&Alpha;":
                                        output.Append((char)913);
                                        break;
                                    case "&alpha;":
                                        output.Append((char)945);
                                        break;

                                    case "&and;":
                                        output.Append((char)8743);
                                        break;
                                    case "&ang;":
                                        output.Append((char)8736);
                                        break;
                                    case "&Aring;":
                                        output.Append((char)197);
                                        break;
                                    case "&aring;":
                                        output.Append((char)229);
                                        break;
                                    case "&asymp;":
                                        output.Append((char)8776);
                                        break;
                                    case "&Atilde;":
                                        output.Append((char)195);
                                        break;
                                    case "&atilde;":
                                        output.Append((char)227);
                                        break;
                                    case "&Auml;":
                                        output.Append((char)196);
                                        break;
                                    case "&auml;":
                                        output.Append((char)228);
                                        break;
                                    case "&bdquo;":
                                        output.Append((char)8222);
                                        break;
                                    case "&Beta;":
                                        output.Append((char)914);
                                        break;
                                    case "&beta;":
                                        output.Append((char)946);
                                        break;
                                    case "&brvbar;":
                                        output.Append((char)166);
                                        break;
                                    case "&bull;":
                                        output.Append((char)8226);
                                        break;
                                    case "&cap;":
                                        output.Append((char)8745);
                                        break;
                                    case "&Ccedil;":
                                        output.Append((char)199);
                                        break;
                                    case "&ccedil;":
                                        output.Append((char)231);
                                        break;
                                    case "&cedil;":
                                        output.Append((char)184);
                                        break;
                                    case "&cent;":
                                        output.Append((char)162);
                                        break;
                                    case "&Chi;":
                                        output.Append((char)935);
                                        break;
                                    case "&chi;":
                                        output.Append((char)967);
                                        break;
                                    case "&circ;":
                                        output.Append((char)710);
                                        break;
                                    case "&clubs;":
                                        output.Append((char)9827);
                                        break;
                                    case "&cong;":
                                        output.Append((char)8773);
                                        break;
                                    case "&copy;":
                                        output.Append((char)169);
                                        break;
                                    case "&crarr;":
                                        output.Append((char)8629);
                                        break;
                                    case "&cup;":
                                        output.Append((char)8746);
                                        break;
                                    case "&curren;":
                                        output.Append((char)164);
                                        break;
                                    case "&dagger;":
                                        output.Append((char)8224);
                                        break;
                                    case "&Dagger;":
                                        output.Append((char)8225);
                                        break;
                                    case "&darr;":
                                        output.Append((char)8595);
                                        break;
                                    case "&dArr;":
                                        output.Append((char)8659);
                                        break;
                                    case "&deg;":
                                        output.Append((char)176);
                                        break;
                                    case "&Delta;":
                                        output.Append((char)916);
                                        break;
                                    case "&delta;":
                                        output.Append((char)948);
                                        break;
                                    case "&diams;":
                                        output.Append((char)9830);
                                        break;
                                    case "&divide;":
                                        output.Append((char)247);
                                        break;
                                    case "&Eacute;":
                                        output.Append((char)201);
                                        break;
                                    case "&eacute;":
                                        output.Append((char)233);
                                        break;
                                    case "&Ecirc;":
                                        output.Append((char)202);
                                        break;
                                    case "&ecirc;":
                                        output.Append((char)234);
                                        break;

                                    case "&Egrave;":
                                        output.Append((char)200);
                                        break;
                                    case "&egrave;":
                                        output.Append((char)232);
                                        break;
                                    case "&empty;":
                                        output.Append((char)8709);
                                        break;
                                    case "&emsp;":
                                        output.Append((char)8195);
                                        break;
                                    case "&Epsilon;":
                                        output.Append((char)917);
                                        break;
                                    case "&epsilon;":
                                        output.Append((char)949);
                                        break;
                                    case "&equiv;":
                                        output.Append((char)8801);
                                        break;
                                    case "&Eta;":
                                        output.Append((char)919);
                                        break;
                                    case "&eta;":
                                        output.Append((char)951);
                                        break;
                                    case "&ETH;":
                                        output.Append((char)208);
                                        break;
                                    case "&eth;":
                                        output.Append((char)240);
                                        break;
                                    case "&Euml;":
                                        output.Append((char)203);
                                        break;
                                    case "&euml;":
                                        output.Append((char)235);
                                        break;
                                    case "&euro;":
                                        output.Append((char)128);
                                        break;
                                    case "&exist;":
                                        output.Append((char)8707);
                                        break;
                                    case "&fnof;":
                                        output.Append((char)402);
                                        break;
                                    case "&forall;":
                                        output.Append((char)8704);
                                        break;
                                    case "&frac12;":
                                        output.Append((char)189);
                                        break;
                                    case "&frac14;":
                                        output.Append((char)188);
                                        break;
                                    case "&frac34;":
                                        output.Append((char)190);
                                        break;
                                    case "&fras1;":
                                        output.Append((char)8260);
                                        break;
                                    case "&Gamma;":
                                        output.Append((char)915);
                                        break;
                                    case "&gamma;":
                                        output.Append((char)947);
                                        break;
                                    case "&ge;":
                                        output.Append((char)8805);
                                        break;
                                    case "&harr;":
                                        output.Append((char)8596);
                                        break;
                                    case "&hArr;":
                                        output.Append((char)8660);
                                        break;
                                    case "&hearts;":
                                        output.Append((char)9829);
                                        break;
                                    case "&hellip;":
                                        output.Append((char)8230);
                                        break;
                                    case "&Iacute;":
                                        output.Append((char)205);
                                        break;
                                    case "&iacute;":
                                        output.Append((char)237);
                                        break;
                                    case "&Icirc;":
                                        output.Append((char)206);
                                        break;
                                    case "&icirc;":
                                        output.Append((char)238);
                                        break;
                                    case "&iexcl;":
                                        output.Append((char)161);
                                        break;
                                    case "&Igrave;":
                                        output.Append((char)204);
                                        break;
                                    case "&igrave;":
                                        output.Append((char)236);
                                        break;
                                    case "&image;":
                                        output.Append((char)8465);
                                        break;
                                    case "&infin;":
                                        output.Append((char)8734);
                                        break;
                                    case "&int;":
                                        output.Append((char)8747);
                                        break;
                                    case "&Iota;":
                                        output.Append((char)921);
                                        break;

                                    case "&iota;":
                                        output.Append((char)953);
                                        break;
                                    case "&iquest;":
                                        output.Append((char)191);
                                        break;
                                    case "&isin;":
                                        output.Append((char)8712);
                                        break;
                                    case "&Iuml;":
                                        output.Append((char)207);
                                        break;
                                    case "&iuml;":
                                        output.Append((char)239);
                                        break;
                                    case "&Kappa;":
                                        output.Append((char)922);
                                        break;
                                    case "&kappa;":
                                        output.Append((char)954);
                                        break;
                                    case "&Lambda;":
                                        output.Append((char)923);
                                        break;
                                    case "&lambda;":
                                        output.Append((char)955);
                                        break;
                                    case "&lang;":
                                        output.Append((char)9001);
                                        break;
                                    case "&laquo;":
                                        output.Append((char)171);
                                        break;
                                    case "&larr;":
                                        output.Append((char)8592);
                                        break;
                                    case "&lArr;":
                                        output.Append((char)8656);
                                        break;
                                    case "&lceil;":
                                        output.Append((char)8968);
                                        break;
                                    case "&ldquo;":
                                        output.Append((char)8220);
                                        break;
                                    case "&le;":
                                        output.Append((char)8804);
                                        break;
                                    case "&lfloor;":
                                        output.Append((char)8970);
                                        break;
                                    case "&lowast;":
                                        output.Append((char)8727);
                                        break;
                                    case "&loz;":
                                        output.Append((char)9674);
                                        break;
                                    case "&lrm;":
                                        output.Append((char)8206);
                                        break;
                                    case "&lsaquo;":
                                        output.Append((char)8249);
                                        break;
                                    case "&lsquo;":
                                        output.Append((char)8216);
                                        break;
                                    case "&macr;":
                                        output.Append((char)175);
                                        break;
                                    case "&mdash;":
                                        output.Append((char)8212);
                                        break;
                                    case "&micro;":
                                        output.Append((char)181);
                                        break;
                                    case "&middot;":
                                        output.Append((char)183);
                                        break;
                                    case "&minus;":
                                        output.Append((char)8722);
                                        break;
                                    case "&Mu;":
                                        output.Append((char)924);
                                        break;
                                    case "&mu;":
                                        output.Append((char)956);
                                        break;
                                    case "&nabla;":
                                        output.Append((char)8711);
                                        break;
                                    case "&nbsp;":
                                        output.Append((char)160);
                                        break;
                                    case "&ndash;":
                                        output.Append((char)8211);
                                        break;
                                    case "&ne;":
                                        output.Append((char)8800);
                                        break;
                                    case "&ni;":
                                        output.Append((char)8715);
                                        break;
                                    case "&not;":
                                        output.Append((char)172);
                                        break;
                                    case "&notin;":
                                        output.Append((char)8713);
                                        break;
                                    case "&nsub;":
                                        output.Append((char)8836);
                                        break;
                                    case "&Ntilde;":
                                        output.Append((char)209);
                                        break;
                                    case "&ntilde;":
                                        output.Append((char)241);
                                        break;
                                    case "&Nu;":
                                        output.Append((char)925);
                                        break;

                                    case "&nu;":
                                        output.Append((char)957);
                                        break;
                                    case "&Oacute;":
                                        output.Append((char)211);
                                        break;
                                    case "&oacute;":
                                        output.Append((char)243);
                                        break;
                                    case "&Ocirc;":
                                        output.Append((char)212);
                                        break;
                                    case "&ocirc;":
                                        output.Append((char)244);
                                        break;
                                    case "&OElig;":
                                        output.Append((char)338);
                                        break;
                                    case "&oelig;":
                                        output.Append((char)339);
                                        break;
                                    case "&Ograve;":
                                        output.Append((char)210);
                                        break;
                                    case "&ograve;":
                                        output.Append((char)242);
                                        break;
                                    case "&oline;":
                                        output.Append((char)8254);
                                        break;
                                    case "&Omega;":
                                        output.Append((char)937);
                                        break;
                                    case "&omega;":
                                        output.Append((char)969);
                                        break;
                                    case "&Omicron;":
                                        output.Append((char)927);
                                        break;
                                    case "&omicron;":
                                        output.Append((char)959);
                                        break;
                                    case "&oplus;":
                                        output.Append((char)8853);
                                        break;
                                    case "&or;":
                                        output.Append((char)8744);
                                        break;
                                    case "&ordf;":
                                        output.Append((char)170);
                                        break;
                                    case "&ordm;":
                                        output.Append((char)186);
                                        break;
                                    case "&Oslash;":
                                        output.Append((char)216);
                                        break;
                                    case "&oslash;":
                                        output.Append((char)248);
                                        break;
                                    case "&Otilde;":
                                        output.Append((char)213);
                                        break;
                                    case "&otilde;":
                                        output.Append((char)245);
                                        break;
                                    case "&otimes;":
                                        output.Append((char)8855);
                                        break;
                                    case "&Ouml;":
                                        output.Append((char)214);
                                        break;
                                    case "&ouml;":
                                        output.Append((char)246);
                                        break;
                                    case "&para;":
                                        output.Append((char)182);
                                        break;
                                    case "&part;":
                                        output.Append((char)8706);
                                        break;
                                    case "&permil;":
                                        output.Append((char)8240);
                                        break;
                                    case "&perp;":
                                        output.Append((char)8869);
                                        break;
                                    case "&Phi;":
                                        output.Append((char)934);
                                        break;
                                    case "&phi;":
                                        output.Append((char)966);
                                        break;
                                    case "&Pi;":
                                        output.Append((char)928);
                                        break;
                                    case "&pi;":
                                        output.Append((char)960);
                                        break;
                                    case "&piv;":
                                        output.Append((char)982);
                                        break;
                                    case "&plusmn;":
                                        output.Append((char)177);
                                        break;
                                    case "&pound;":
                                        output.Append((char)163);
                                        break;
                                    case "&prime;":
                                        output.Append((char)8242);
                                        break;
                                    case "&Prime;":
                                        output.Append((char)8243);
                                        break;
                                    case "&prod;":
                                        output.Append((char)8719);
                                        break;
                                    case "&prop;":
                                        output.Append((char)8733);
                                        break;
                                    case "&Psi;":
                                        output.Append((char)936);
                                        break;

                                    case "&psi;":
                                        output.Append((char)968);
                                        break;
                                    case "&radic;":
                                        output.Append((char)8730);
                                        break;
                                    case "&rang;":
                                        output.Append((char)9002);
                                        break;
                                    case "&raquo;":
                                        output.Append((char)187);
                                        break;
                                    case "&rarr;":
                                        output.Append((char)8594);
                                        break;
                                    case "&rArr;":
                                        output.Append((char)8658);
                                        break;
                                    case "&rceil;":
                                        output.Append((char)8969);
                                        break;
                                    case "&rdquo;":
                                        output.Append((char)8221);
                                        break;
                                    case "&real;":
                                        output.Append((char)8476);
                                        break;
                                    case "&reg;":
                                        output.Append((char)174);
                                        break;
                                    case "&rfloor;":
                                        output.Append((char)8971);
                                        break;
                                    case "&Rho;":
                                        output.Append((char)929);
                                        break;
                                    case "&rho;":
                                        output.Append((char)961);
                                        break;
                                    case "&rlm;":
                                        output.Append((char)8207);
                                        break;
                                    case "&rsaquo;":
                                        output.Append((char)8250);
                                        break;
                                    case "&rsquo;":
                                        output.Append((char)8217);
                                        break;
                                    case "&sbquo;":
                                        output.Append((char)8218);
                                        break;
                                    case "&Scaron;":
                                        output.Append((char)352);
                                        break;
                                    case "&scaron;":
                                        output.Append((char)353);
                                        break;
                                    case "&sdot;":
                                        output.Append((char)8901);
                                        break;
                                    case "&sect;":
                                        output.Append((char)167);
                                        break;
                                    case "&shy;":
                                        output.Append((char)173);
                                        break;
                                    case "&Sigma;":
                                        output.Append((char)931);
                                        break;
                                    case "&sigma;":
                                        output.Append((char)963);
                                        break;
                                    case "&sigmaf;":
                                        output.Append((char)962);
                                        break;
                                    case "&sim;":
                                        output.Append((char)8764);
                                        break;
                                    case "&spades;":
                                        output.Append((char)9824);
                                        break;
                                    case "&sub;":
                                        output.Append((char)8834);
                                        break;
                                    case "&sube;":
                                        output.Append((char)8838);
                                        break;
                                    case "&sum;":
                                        output.Append((char)8721);
                                        break;
                                    case "&sup;":
                                        output.Append((char)8835);
                                        break;
                                    case "&sup1;":
                                        output.Append((char)185);
                                        break;
                                    case "&sup2;":
                                        output.Append((char)178);
                                        break;
                                    case "&sup3;":
                                        output.Append((char)179);
                                        break;
                                    case "&supe;":
                                        output.Append((char)8839);
                                        break;
                                    case "&szlig;":
                                        output.Append((char)223);
                                        break;
                                    case "&Tau;":
                                        output.Append((char)932);
                                        break;
                                    case "&tau;":
                                        output.Append((char)964);
                                        break;
                                    case "&there4;":
                                        output.Append((char)8756);
                                        break;
                                    case "&Theta;":
                                        output.Append((char)920);
                                        break;

                                    case "&theta;":
                                        output.Append((char)952);
                                        break;
                                    case "&thetasym;":
                                        output.Append((char)977);
                                        break;
                                    case "&thinsp;":
                                        output.Append((char)8201);
                                        break;
                                    case "&THORN;":
                                        output.Append((char)222);
                                        break;
                                    case "&thorn;":
                                        output.Append((char)254);
                                        break;
                                    case "&tilde;":
                                        output.Append((char)732);
                                        break;
                                    case "&times;":
                                        output.Append((char)215);
                                        break;
                                    case "&trade;":
                                        output.Append((char)8482);
                                        break;
                                    case "&Uacute;":
                                        output.Append((char)218);
                                        break;
                                    case "&uacute;":
                                        output.Append((char)250);
                                        break;
                                    case "&uarr;":
                                        output.Append((char)8593);
                                        break;
                                    case "&uArr;":
                                        output.Append((char)8657);
                                        break;
                                    case "&Ucirc;":
                                        output.Append((char)219);
                                        break;
                                    case "&ucirc;":
                                        output.Append((char)251);
                                        break;
                                    case "&Ugrave;":
                                        output.Append((char)217);
                                        break;
                                    case "&ugrave;":
                                        output.Append((char)249);
                                        break;
                                    case "&uml;":
                                        output.Append((char)168);
                                        break;
                                    case "&upsih;":
                                        output.Append((char)978);
                                        break;
                                    case "&Upsilon;":
                                        output.Append((char)933);
                                        break;
                                    case "&upsilon;":
                                        output.Append((char)965);
                                        break;
                                    case "&Uuml;":
                                        output.Append((char)220);
                                        break;
                                    case "&uuml;":
                                        output.Append((char)252);
                                        break;
                                    case "&weierp;":
                                        output.Append((char)8472);
                                        break;
                                    case "&Xi;":
                                        output.Append((char)926);
                                        break;
                                    case "&xi;":
                                        output.Append((char)958);
                                        break;
                                    case "&Yacute;":
                                        output.Append((char)221);
                                        break;
                                    case "&yacute;":
                                        output.Append((char)253);
                                        break;
                                    case "&yen;":
                                        output.Append((char)165);
                                        break;
                                    case "&Yuml;":
                                        output.Append((char)376);
                                        break;
                                    case "&yuml;":
                                        output.Append((char)255);
                                        break;
                                    case "&Zeta;":
                                        output.Append((char)918);
                                        break;
                                    case "&zeta;":
                                        output.Append((char)950);
                                        break;
                                    case "&zwj;":
                                        output.Append((char)8205);
                                        break;
                                    case "&zwnj;":
                                        output.Append((char)8204);
                                        break;

                                    default:
                                        output.Append(token.ToString());
                                        break;
                                }
                            }
                        }
                        else
                        {
                            output.Append(token.ToString());
                        }
                    }
                }
            }

            return output.ToString();
        }
    }
}
*/