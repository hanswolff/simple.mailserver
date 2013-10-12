#region
//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 
// refactored by Hans Wolff

#endregion

using System;

namespace Bdev.Net.Dns
{
	/// <summary>
	/// An MX (Mail Exchanger) Resource Record (RR) (RFC1035 3.3.9)
	/// </summary>
	[Serializable]
	public class MXRecord : RecordBase, IComparable
	{
		// an MX record is a domain name and an integer preference
		private readonly string		_domainName;
		private readonly int		_preference;

		// expose these fields public read/only
		public string DomainName	{ get { return _domainName; }}
		public int Preference		{ get { return _preference; }}
				
		/// <summary>
		/// Constructs an MX record by reading bytes from a return message
		/// </summary>
		/// <param name="pointer">A logical pointer to the bytes holding the record</param>
		internal MXRecord(Pointer pointer)
		{
			_preference = pointer.ReadShort();
			_domainName = pointer.ReadDomain();
		}

		public override string ToString()
		{
			return string.Format("Mail Server = {0}, Preference = {1}", _domainName, _preference);
		}

		#region IComparable Members

		/// <summary>
		/// Implements the IComparable interface so that we can sort the MX records by their
		/// lowest preference
		/// </summary>
		/// <param name="obj">the other MxRecord to compare against</param>
		/// <returns>1, 0, -1</returns>
		public int CompareTo(object obj)
		{
			var mxOther = (MXRecord)obj;

			// we want to be able to sort them by preference
			if (mxOther._preference < _preference) return 1;
			if (mxOther._preference > _preference) return -1;
			
			// order mail servers of same preference by name
			return -String.Compare(mxOther._domainName, _domainName, StringComparison.Ordinal);
		}

		public static bool operator==(MXRecord record1, MXRecord record2)
		{
		    if (record1 == null && record2 == null) return true;
		    if (record1 == null) return false;
            if (record2 == null) return false;

			return record1.Equals(record2);
		}
	
		public static bool operator!=(MXRecord record1, MXRecord record2)
		{
			return !(record1 == record2);
		}
/*
		public static bool operator<(MXRecord record1, MXRecord record2)
		{
			if (record1._preference > record2._preference) return false;
			if (record1._domainName > record2._domainName) return false;
			return false;
		}

		public static bool operator>(MXRecord record1, MXRecord record2)
		{
			if (record1._preference < record2._preference) return false;
			if (record1._domainName < record2._domainName) return false;
			return false;
		}
*/

		public override bool Equals(object obj)
		{
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            
			var mxOther = (MXRecord)obj;

			// preference must match
			if (mxOther._preference != _preference) return false;
			
			// and so must the domain name
			if (mxOther._domainName != _domainName) return false;

			// its a match
			return true;
		}

		public override int GetHashCode()
		{
			return _domainName.GetHashCode() * 29 ^ _preference.GetHashCode();
		}

		#endregion
	}
}
