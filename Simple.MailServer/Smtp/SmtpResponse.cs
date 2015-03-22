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
using System.Collections.ObjectModel;
using System.Linq;

namespace Simple.MailServer.Smtp
{
    public class SmtpResponse : ICloneable, IEquatable<SmtpResponse>
    {
        public static readonly SmtpResponse None = new SmtpResponse();

        public ReadOnlyCollection<string> AdditionalLines { get; private set; }

        public bool Success { get { return ResponseCode >= 200 && ResponseCode < 400; } }
        public bool HasValue { get { return !None.Equals(this); } }

        public int ResponseCode { get; private set; }
        public string ResponseText { get; private set; }

        private SmtpResponse()
        {
        }

        public SmtpResponse(int responseCode, string responseText, IList<string> additionalLines = null)
        {
            AdditionalLines = new ReadOnlyCollection<string>(additionalLines ?? new List<string>());
            ResponseCode = responseCode;
            ResponseText = responseText;
        }

        public SmtpResponse Clone()
        {
            return new SmtpResponse(ResponseCode, ResponseText, AdditionalLines.ToList());
        }

        public SmtpResponse CloneAndChange(string newResponseText)
        {
            return new SmtpResponse(ResponseCode, newResponseText, AdditionalLines.ToList());
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public bool Equals(SmtpResponse other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ResponseCode != other.ResponseCode) return false;
            if (ResponseText != other.ResponseText) return false;

            if (AdditionalLines.Count != other.AdditionalLines.Count) return false;
            for (var i = AdditionalLines.Count - 1; i >= 0; i--)
            {
                if (AdditionalLines[i] != other.AdditionalLines[i])
                    return false;
            }

            return true;
        }

        bool IEquatable<SmtpResponse>.Equals(SmtpResponse other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SmtpResponse)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ResponseCode.GetHashCode();
                hashCode = (hashCode * 397) ^ (ResponseText != null ? ResponseText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AdditionalLines.Count.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return ResponseCode + " " + ResponseText;
        }
    }
}
