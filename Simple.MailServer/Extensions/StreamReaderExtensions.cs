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
using System.Reflection;

namespace Simple.MailServer.Extensions
{
    public static class StreamReaderExtensions
    {
        public static long GetCurrentStreamPosition(this StreamReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            var type = reader.GetType();

            var charpos = (int) type.InvokeMember("charPos",
                                                  BindingFlags.DeclaredOnly |
                                                  BindingFlags.Public | BindingFlags.NonPublic |
                                                  BindingFlags.Instance | BindingFlags.GetField
                                                  , null, reader, null);

            var charlen = (int) type.InvokeMember("charLen",
                                                  BindingFlags.DeclaredOnly |
                                                  BindingFlags.Public | BindingFlags.NonPublic |
                                                  BindingFlags.Instance | BindingFlags.GetField
                                                  , null, reader, null);

            return reader.BaseStream.Position - charlen + charpos;
        }

        public static long Seek(this StreamReader streamReader, long position, SeekOrigin origin)
        {
            if (streamReader == null)
                throw new ArgumentNullException("streamReader");

            if (!streamReader.BaseStream.CanSeek)
                throw new ArgumentException("Underlying stream should be seekable.", "streamReader");

            var preamble = (byte[])streamReader.GetType().InvokeMember("_preamble", BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, streamReader, null);
            if (preamble.Length > 0 && position < preamble.Length) // preamble or BOM must be skipped
                position += preamble.Length;

            var newPosition = streamReader.BaseStream.Seek(position, origin); // seek
            streamReader.DiscardBufferedData(); // this updates the buffer

            return newPosition;
        }
    }
}
