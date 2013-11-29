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

using Simple.MailServer.Mime;
using System;
using Xunit;

namespace Simple.MailServer.Tests.Mime
{
    public class DateTimeRfc2822Facts
    {
        [Fact]
        public void parse_month_jan_datetime()
        {
            var universalTime = new DateTime(2000, 01, 01, 00, 00, 00);
            const string dateStringToParse = "Sat, 01 Jan 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_feb_datetime()
        {
            var universalTime = new DateTime(2000, 02, 01, 00, 00, 00);
            const string dateStringToParse = "Tue, 01 Feb 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_mar_datetime()
        {
            var universalTime = new DateTime(2000, 03, 01, 00, 00, 00);
            const string dateStringToParse = "Wed, 01 Mar 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_apr_datetime()
        {
            var universalTime = new DateTime(2000, 04, 01, 00, 00, 00);
            const string dateStringToParse = "Sat, 01 Apr 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_may_datetime()
        {
            var universalTime = new DateTime(2000, 05, 01, 00, 00, 00);
            const string dateStringToParse = "Mon, 01 May 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_jun_datetime()
        {
            var universalTime = new DateTime(2000, 06, 01, 00, 00, 00);
            const string dateStringToParse = "Thu, 01 Jun 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_jul_datetime()
        {
            var universalTime = new DateTime(2000, 07, 01, 00, 00, 00);
            const string dateStringToParse = "Sat, 01 Jul 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_aug_datetime()
        {
            var universalTime = new DateTime(2000, 08, 01, 00, 00, 00);
            const string dateStringToParse = "Tue, 01 Aug 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_sep_datetime()
        {
            var universalTime = new DateTime(2000, 09, 01, 00, 00, 00);
            const string dateStringToParse = "Fri, 01 Sep 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_oct_datetime()
        {
            var universalTime = new DateTime(2000, 10, 01, 00, 00, 00);
            const string dateStringToParse = "Sun, 01 Oct 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_nov_datetime()
        {
            var universalTime = new DateTime(2000, 11, 01, 00, 00, 00);
            const string dateStringToParse = "Wed, 01 Nov 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_month_dec_datetime()
        {
            var universalTime = new DateTime(2000, 12, 01, 00, 00, 00);
            const string dateStringToParse = "Fri, 01 Dec 2000 00:00:00 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_gmt_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 02 Jan 2000 03:04:05 GMT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_timeoffset_brackets_ignored_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 02 Jan 2000 04:04:05 +0100 (PST)";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_bst_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 02 Jan 2000 04:04:05 BST";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_edt_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 01 Jan 2000 23:04:05 EDT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_est_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 01 Jan 2000 22:04:05 EST";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_cdt_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 01 Jan 2000 22:04:05 CDT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_cst_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 01 Jan 2000 21:04:05 CST";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_mdt_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 01 Jan 2000 21:04:05 MDT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_mst_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 01 Jan 2000 20:04:05 MST";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_pdt_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 01 Jan 2000 20:04:05 PDT";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }

        [Fact]
        public void parse_pst_datetime()
        {
            var universalTime = new DateTime(2000, 01, 02, 03, 04, 05);
            const string dateStringToParse = "Sun, 01 Jan 2000 19:04:05 PST";

            Assert.Equal(universalTime, DateTimeRfc2822.ParseDateRfc2822(dateStringToParse));
        }
    }
}
