using System;
using System.Text;

namespace EmuWarface.Xmpp
{
	public sealed class Jid : IEquatable<Jid>
	{
		public string Local
		{
			get;
			private set;
		}

		public string Domain
		{
			get;
			private set;
		}

		public string Resource
		{
			get;
			private set;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Local, Domain, Resource);
		}


		// local@domain
		public bool IsBareJid
			=> string.IsNullOrWhiteSpace(Resource);

		// domain
		// domain/resource
		public bool IsServer => string.IsNullOrWhiteSpace(Local) && !string.IsNullOrWhiteSpace(Domain);

		// local@server/resource
		public bool IsFullJid
			=> !IsServer && !IsBareJid;

		public Jid(string jid)
		{
			var at = jid.IndexOf('@');
			var slash = jid.IndexOf('/');

			if (at != -1)
				Local = jid[0..at];

			if (slash == -1)
				Domain = jid[(at + 1)..];
			else
			{
				Domain = jid[(at + 1)..slash];
				Resource = jid[(slash + 1)..];
			}
		}

		public Jid(string domain, string local, string resource = null)
		{
			if (string.IsNullOrEmpty(domain))
				throw new ArgumentNullException(nameof(domain));

			Local = local;
			Domain = domain;
			Resource = resource;
		}

		public static implicit operator string(Jid jid)
			=> jid?.ToString();

		public static implicit operator Jid(string jid)
		{
			if (jid is null)
				return null;

			return new Jid(jid);
		}

		public string Bare
		{
			get
			{
				if (Local == null)
					return Domain;

				return string.Concat(Local, '@', Domain);
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			if (Local != null)
				sb.Append(Local).Append('@');

			sb.Append(Domain);

			if (Resource != null)
				sb.Append('/').Append(Resource);

			return sb.ToString();
		}

		public override bool Equals(object obj)
		{
			return obj is Jid other && Equals(other);
		}

		public bool Equals(Jid other)
		{
			if (other is null)
				return false;

			return string.Equals(Local, other.Local, StringComparison.OrdinalIgnoreCase) // both case insensitive
				&& string.Equals(Domain, other.Domain, StringComparison.OrdinalIgnoreCase) // both case insensitive
				&& string.Equals(Resource, other.Resource, StringComparison.Ordinal); // resource is case sensitive
		}

		public static bool operator ==(Jid a, Jid b)
		{
			if (a is null && b is null)
				return true;

			if (a is null || b is null)
				return false;

			return a.Equals(b);
		}

		public static bool operator !=(Jid a, Jid b) => !(a == b);
	}
}