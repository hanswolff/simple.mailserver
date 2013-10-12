using Simple.MailServer.Mime;
using System;
using Xunit;

namespace Simple.MailServer.Tests.Mime
{
    public class DateTimeRfc2822Facts
    {
        [Fact]
        public void parse_gmt_datetime()
        {
            var dateTime = new DateTime(2000, 01, 02, 03, 04, 05);
            var dateString = dateTime.ToString("ddd, dd MMM yyyy hh:mm:ss ") + "GMT";

            Assert.Equal(dateTime, DateTimeRfc2822.ParseDateRfc2822(dateString));
        }
    }
}
