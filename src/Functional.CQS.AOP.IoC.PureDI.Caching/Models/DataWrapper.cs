namespace Functional.CQS.AOP.IoC.PureDI.Caching.Models
{
	// used to guarantee storage of non-null objects in cases where the underlying cache implementation does not support storage of null.
	internal class DataWrapper<T> where T : class
	{
		public DataWrapper(T data)
		{
			Data = data;
		}

		public T Data { get; }
	}
}