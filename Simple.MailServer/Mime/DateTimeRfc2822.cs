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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Simple.MailServer.Mime
{
    public static class DateTimeRfc2822
    {
        private static readonly Dictionary<string, string> Timezones = new Dictionary<string, string>
        {
            {"bst", "+0100"},
            {"gmt", "-0000"},
            {"ut",  "-0000"},
            {"edt", "-0400"},
            {"est", "-0500"},
            {"cdt", "-0500"},
            {"cst", "-0600"},
            {"mdt", "-0600"},
            {"mst", "-0700"},
            {"pdt", "-0700"},
            {"pst", "-0800"},
        };

        private static readonly Dictionary<string, string> Months = new Dictionary<string, string>
        {
            {"jan", "01"},
            {"feb", "02"},
            {"mar", "03"},
            {"apr", "04"},
            {"may", "05"},
            {"jun", "06"},
            {"jul", "07"},
            {"aug", "08"},
            {"sep", "09"},
            {"oct", "10"},
            {"nov", "11"},
            {"dec", "12"},
        };

        /// <summary>
        /// Parses RFC 2822 datetime.
        /// </summary>
        /// <param name="date">Date string</param>
        /// <returns>parsed DateTime</returns>
        public static DateTime ParseDateRfc2822(string date)
        {
            DateTime dateTime;
            if (!TryParseDateRfc2822(date, out dateTime))
                throw new FormatException("Invalid date format '" + date + "'");
            return dateTime;
        }

        /// <summary>
        /// Parses RFC 2822 datetime
        /// </summary>
        /// <param name="dateStr">Date string</param>
        /// <param name="dateTime">parsed DateTime or DateTime.MinValue if could not be parsed</param>
        /// <returns></returns>
        public static bool TryParseDateRfc2822(string dateStr, out DateTime dateTime)
        {
            /* Rfc 2822 3.3. Date and Time Specification.			 
                date-time       = [ day-of-week "," ] date FWS time [CFWS]
                date            = day month year
                time            = hour ":" minute [ ":" second ] FWS zone
            */

            /* IMAP date format. 
                date-time       = date FWS time [CFWS]
                date            = day-month-year
                time            = hour ":" minute [ ":" second ] FWS zone
            */

            dateStr = dateStr.ToLowerInvariant();
            dateStr = DateReplaceTimezones(dateStr);
            dateStr = MakeMonthsNumeric(dateStr);
            dateStr = RemoveDayOfWeek(dateStr);
            dateStr = RemoveBrackets(dateStr);
            dateStr = RemoveWhitespaces(dateStr);

            string[] dmyhmsz = SplitParts(dateStr);
            if (dmyhmsz == null)
            {
                dateTime = default(DateTime);
                return false;
            }

            var normalizedDate = CreateNormalizedDate(dmyhmsz);

            const string dateFormat = "dd MM yyyy HH':'mm':'ss zzz";
            return DateTime.TryParseExact(normalizedDate.ToString(),
                dateFormat,
                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                System.Globalization.DateTimeStyles.AdjustToUniversal,
                out dateTime);
        }

        private static StringBuilder CreateNormalizedDate(string[] dmyhmsz)
        {
            var normalizedDate = new StringBuilder();

            // Day
            if (dmyhmsz[0].Length == 1)
                normalizedDate.Append("0" + dmyhmsz[0] + " ");
            else
                normalizedDate.Append(dmyhmsz[0] + " ");

            // Month
            if (dmyhmsz[1].Length == 1)
                normalizedDate.Append("0" + dmyhmsz[1] + " ");
            else
                normalizedDate.Append(dmyhmsz[1] + " ");

            // Year
            if (dmyhmsz[2].Length == 2)
                normalizedDate.Append("20" + dmyhmsz[2] + " ");
            else
                normalizedDate.Append(dmyhmsz[2] + " ");

            // Hour
            if (dmyhmsz[3].Length == 1)
                normalizedDate.Append("0" + dmyhmsz[3] + ":");
            else
                normalizedDate.Append(dmyhmsz[3] + ":");

            // Minute
            if (dmyhmsz[4].Length == 1)
                normalizedDate.Append("0" + dmyhmsz[4] + ":");
            else
                normalizedDate.Append(dmyhmsz[4] + ":");

            // Second
            if (dmyhmsz[5].Length == 1)
                normalizedDate.Append("0" + dmyhmsz[5] + " ");
            else
                normalizedDate.Append(dmyhmsz[5] + " ");

            // TimeZone
            normalizedDate.Append(dmyhmsz[6]);
            return normalizedDate;
        }

        private static string DateReplaceTimezones(string date)
        {
            foreach (var kp in Timezones)
            {
                var timeZone = kp.Key;
                var offset = kp.Value;

                date = Regex.Replace(date, @"\b(" + timeZone + @")\b", offset, RegexOptions.IgnoreCase);
            }
            return date;
        }

        private static string MakeMonthsNumeric(string date)
        {
            foreach (var kp in Months)
            {
                var monthName = kp.Key;
                var monthValue = kp.Value;

                date = Regex.Replace(date, @"\b(" + monthName + @")\b", monthValue, RegexOptions.IgnoreCase);
            }
            return date;
        }

        private static string RemoveDayOfWeek(string dateStr)
        {
            var pos = dateStr.IndexOf(',');
            if (pos >= 0)
                dateStr = dateStr.Substring(pos + 1);
            return dateStr;
        }

        private static string RemoveBrackets(string dateStr)
        {
            // Remove () from date. "Mon, 13 Oct 2003 20:50:57 -0400 (EDT)"
            if (dateStr.IndexOf(" (", StringComparison.Ordinal) > -1)
                dateStr = dateStr.Substring(0, dateStr.IndexOf(" (", StringComparison.Ordinal));
            return dateStr;
        }

        private static string RemoveWhitespaces(string dateStr)
        {
            dateStr = dateStr.Trim();
            while (dateStr.IndexOf("  ", StringComparison.Ordinal) >= 0)
                dateStr = dateStr.Replace("  ", " ");
            return dateStr;
        }

        private static string[] SplitParts(string dateStr)
        {
            string timeString;
            var dmyhmsz = new string[7];
            string[] dateparts = dateStr.Split(' ');

            // Rfc 2822 date (day month year time timeZone)
            if (dateparts.Length == 5)
            {
                dmyhmsz[0] = dateparts[0];
                dmyhmsz[1] = dateparts[1];
                dmyhmsz[2] = dateparts[2];

                timeString = dateparts[3];

                dmyhmsz[6] = dateparts[4];
            }
            // IMAP date (day-month-year time timeZone)
            else if (dateparts.Length == 3)
            {
                string[] dmy = dateparts[0].Split('-');
                if (dmy.Length == 3)
                {
                    dmyhmsz[0] = dmy[0];
                    dmyhmsz[1] = dmy[1];
                    dmyhmsz[2] = dmy[2];

                    timeString = dateparts[1];

                    dmyhmsz[6] = dateparts[2];
                }
                else return null;
            }
            else return null;

            // Parse time part (hour ":" minute [ ":" second ])
            string[] timeParts = timeString.Split(':');
            if (timeParts.Length == 3)
            {
                dmyhmsz[3] = timeParts[0];
                dmyhmsz[4] = timeParts[1];
                dmyhmsz[5] = timeParts[2];
            }
            else if (timeParts.Length == 2)
            {
                dmyhmsz[3] = timeParts[0];
                dmyhmsz[4] = timeParts[1];
                dmyhmsz[5] = "00";
            }
            else return null;
            return dmyhmsz;
        }

        /// <summary>
        /// Converts date to RFC 2822 date time string
        /// </summary>
        /// <param name="dateTime">Date time value</param>
        /// <returns></returns>
        public static string DateTimeToRfc2822(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("r", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }
    }
}
