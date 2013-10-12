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
