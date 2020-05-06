using System;
using System.Collections.Generic;
using System.Text;

namespace ExternalNetcoreExtensions.ModifiableDistributed
{
	/// <summary>
	/// Controls <see cref="ModifiableDistributedResponseCache"/>, telling it how to treat the cache.
	/// For example, you may have custom headers whose presence you want to cause to clear
	/// cache values relevant to that request.
	/// </summary>
	public interface IModifiableCacheController
	{
		/// <summary>
		/// If the cache items should be cleared.
		/// </summary>
		bool ShouldClearCache { get; }

		/// <summary>
		/// If the cache should be ignored. It won't be read from or written to.
		/// </summary>
		bool ShouldIgnoreCache { get; }

		/// <summary>
		/// If reads from cache should be permitted. By default, only true if <see cref="ShouldIgnoreCache"/>
		/// and <see cref="ShouldClearCache"/> are true. 
		/// </summary>
		bool ShouldAllowReadFromCache => !(ShouldClearCache || ShouldIgnoreCache);
	}
}
