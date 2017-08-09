using System;

namespace DMS.Application
{
	public interface IResourceProvider
	{
		void Add<RESOURCE_TYPE>(string name, IResource<RESOURCE_TYPE> resource) where RESOURCE_TYPE : IDisposable;
		IResource<RESOURCE_TYPE> Get<RESOURCE_TYPE>(string name) where RESOURCE_TYPE : IDisposable;
	}
}