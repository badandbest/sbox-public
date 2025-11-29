using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace Sandbox;

public static partial class SandboxSystemExtensions
{
	/// <summary>
	/// Runs each task on this thread but only execute a set amount at a time
	/// </summary>
	public static async Task ForEachTaskAsync<T>( this IEnumerable<T> source, Func<T, Task> body, int maxRunning = 8, CancellationToken token = default )
	{
		var tasks = new List<Task>();

		foreach ( var item in source )
		{
			var t = body( item );
			tasks.Add( t );

			while ( tasks.Count >= maxRunning )
			{
				await Task.WhenAny( tasks );
				tasks.RemoveAll( x => x.IsCompleted );
			}

			token.ThrowIfCancellationRequested();
		}

		await Task.WhenAll( tasks );

		token.ThrowIfCancellationRequested();
	}

	/// <summary>Finds the first common base type of the given types.</summary>
	/// <param name="types">The types.</param>
	/// <returns>The common base type.</returns>
	public static Type GetCommonBaseType( this IEnumerable<Type> types )
	{
		types = types.ToList();
		var baseType = types.First();
		while ( baseType != typeof( object ) && baseType != null )
		{
			if ( types.All( t => baseType.GetTypeInfo().IsAssignableFrom( t.GetTypeInfo() ) ) )
			{
				return baseType;
			}

			baseType = baseType.GetTypeInfo().BaseType;
		}

		return typeof( object );
	}

	/// <summary>
	/// Filters the elements of a sequence based on a specified type.
	/// </summary>
	/// <param name="source">The sequence to filter.</param>
	/// <param name="type">The type to filter the elements of the sequence on.</param>
	/// <returns>A sequence of only that type.</returns>
	public static IEnumerable<object> OfType( this IEnumerable source, Type type )
	{
		return source.OfType<object>().Where( type.IsInstanceOfType );
	}
}
