#region Header
// Copyright (c) 2013 Hans Wolff
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Simple.MailServer.Mime
{
    public static class Rfc5335Decoder
    {
        static readonly Regex RegexRfc5335 = new Regex(@"=\?(?<charset>.*?)\?(?<encoding>[qQbB])\?(?<value>.*?)\?=\s*", RegexOptions.Compiled);

        public static string Decode(string text)
        {
            bool isRfc;
            return Decode(text, out isRfc);
        }

        public static string Decode(string text, out bool isRfc5335)
        {
            isRfc5335 = false;
            if (String.IsNullOrEmpty(text)) return text;

            int startAt = 0;
            var result = new StringBuilder();

            do
            {
                var match = RegexRfc5335.Match(text, startAt);
                if (!match.Success)
                {
                    if (startAt > 0 && text[startAt-1] == ' ')
                        startAt--;

                    result.Append(text.Substring(startAt, text.Length - startAt));
                    return result.ToString();
                }

                isRfc5335 = true;

                var charset = match.Groups["charset"].Value;
                var encoding = match.Groups["encoding"].Value.ToUpperInvariant();
                var value = match.Groups["value"].Value;

                {
                    var between = text.Substring(startAt, match.Index - startAt);
                    between = between.TrimEnd();
                    result.Append(between);
                }

                if (encoding.Equals("B"))
                {
                    var decodedValue = DecodeBase64(value, charset);
                    result.Append(decodedValue);
                }
                if (encoding.Equals("Q"))
                {
                    var decodedValue = DecodeQuotedPrintable(value, charset);
                    result.Append(decodedValue);
                }

                startAt = match.Index + match.Length;

            } while (true);
        }

        private static string DecodeBase64(string value, string charset)
        {
            var bytes = Convert.FromBase64String(value);
            return Encoding.GetEncoding(charset).GetString(bytes);
        }

        private static string DecodeQuotedPrintable(string value, string charset)
        {
            using (var mem = new MemoryStream(Encoding.Default.GetBytes(value)))
            using (var decoder = new QuotedPrintableDecoderStream(mem, true))
            using (var reader = new StreamReader(decoder, Encoding.GetEncoding(charset), false))
            {
                var part = reader.ReadToEnd();
                part = part.Replace('_', ' ');
                return part;
            }
        }
    }
}
