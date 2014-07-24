using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MagentoAccess.Misc
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> GetPage<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
		{
			return source.Skip(pageNumber * pageSize).Take(pageSize);
		}

		public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
		{
			foreach (var variable in self)
			{
				action(variable);
			}
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}

		public static async Task<IEnumerable<TResult>> ProcessInBatchAsync<TInput, TResult>(
			this IEnumerable<TInput> inputEnumerable,
			int batchSize,
			Func<TInput, Task<TResult>> processor, bool ignoreNull = true)
		{
			var result = new List<TResult>(inputEnumerable.Count());
			var processingTasks = new List<Task<TResult>>(batchSize);

			foreach (var input in inputEnumerable)
			{
				processingTasks.Add(processor(input));

				if (processingTasks.Count == batchSize) // batch size reached, wait for completion and process
				{
					AddResultToList(await Task.WhenAll(processingTasks).ConfigureAwait(false), result, ignoreNull);
					processingTasks.Clear();
				}
			}
			AddResultToList(await Task.WhenAll(processingTasks).ConfigureAwait(false), result, ignoreNull);
			return result;
		}

		public static async Task DoInBatchAsync<TInput>(
			this IEnumerable<TInput> inputEnumerable,
			int batchSize,
			Func<TInput, Task> processor, bool ignoreNull = true)
		{
			var processingTasks = new List<Task>(batchSize);

			foreach (var input in inputEnumerable)
			{
				processingTasks.Add(processor(input));

				if (processingTasks.Count == batchSize) // batch size reached, wait for completion and process
				{
					await Task.WhenAll(processingTasks).ConfigureAwait(false);
					processingTasks.Clear();
				}
			}
			await Task.WhenAll(processingTasks).ConfigureAwait(false);
		}

		private static void AddResultToList<TResult>(IEnumerable<TResult> intermidiateResult, List<TResult> endResult, bool ignoreNull)
		{
			foreach (var value in intermidiateResult)
			{
				if (ignoreNull && Equals(value, default(TResult)))
					continue;

				endResult.Add(value);
			}
		}

		/// <summary>
		/// Clumps items into same size lots.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source list of items.</param>
		/// <param name="size">The maximum size of the clumps to make.</param>
		/// <returns>A list of list of items, where each list of items is no bigger than the size given.</returns>
		public static IEnumerable<IEnumerable<T>> Clump<T>(this IEnumerable<T> source, int size)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (size < 1)
				throw new ArgumentOutOfRangeException("size", "size must be greater than 0");

			return ClumpIterator(source, size);
		}

		private static IEnumerable<IEnumerable<T>> ClumpIterator<T>(IEnumerable<T> source, int size)
		{
			Debug.Assert(source != null, "source is null.");

			var items = new T[size];
			var count = 0;
			foreach (var item in source)
			{
				items[count] = item;
				count++;

				if (count == size)
				{
					yield return items;
					items = new T[size];
					count = 0;
				}
			}
			if (count > 0)
			{
				if (count == size)
					yield return items;
				else
				{
					var tempItems = new T[count];
					Array.Copy(items, tempItems, count);
					yield return tempItems;
				}
			}
		}
	}
}