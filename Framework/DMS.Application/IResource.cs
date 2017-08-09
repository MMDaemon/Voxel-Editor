using System;

namespace DMS.Application
{
	public interface IResource<RESOURCE_TYPE> where RESOURCE_TYPE : IDisposable
	{
		bool IsValueCreated { get; }
		RESOURCE_TYPE Value { get; }

		event EventHandler<RESOURCE_TYPE> Change;
	}
}
