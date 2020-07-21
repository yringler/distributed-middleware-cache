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
		/// If the cache items should be cleared. This will not cause the entire cache to be cleared,
		/// rather only the cached items whose access is attempted when this property is true.
		/// </summary>
		bool ShouldClearCache { get; }

		/// <summary>
		/// If the cache can be read from.
		/// </summary>
		bool ShouldReadFromCache { get; }

		/// <summary>
		/// If the cache can be updated.
		/// </summary>
		bool ShouldWriteToCache { get; }
	}
}
