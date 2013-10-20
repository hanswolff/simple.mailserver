#region Header
// BaseSessionFacts.cs
// 
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
using System.Threading;
using Xunit;

namespace Simple.MailServer.Tests
{
    public class BaseSessionFacts
    {
        [Fact]
        public void GetIdleTimeMilliseconds_starts_at_zero()
        {
            var session = new BaseSession(null);
            Assert.InRange(session.GetIdleTimeMilliseconds(), 0, 10);
        }

        [Fact]
        public void GetIdleTimeMilliseconds_increases_over_time()
        {
            var session = new BaseSession(null);
            Thread.Sleep(20);
            Assert.InRange(session.GetIdleTimeMilliseconds(), 10, 100);
        }
    }
}
