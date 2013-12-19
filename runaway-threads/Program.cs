using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NGenerics.DataStructures.Mathematical;

namespace runaway_threads
{
	public class Program
	{
		#region P/Invoke Methods (Not really important)

		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		[DllImport("kernel32.dll")]
		private static extern UIntPtr SetThreadAffinityMask(uint thread, uint mask);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsThreadAFiber();

		[DllImport("kernel32.dll")]
		private static extern UIntPtr SetProcessAffinityMask(IntPtr thread, uint mask);

		#endregion P/Invoke Methods (Not really important)

		private const uint processMask = 0xFF; // 11111111

		static void Main(string[] args)
		{
			SetProcessAffinityMask(Process.GetCurrentProcess().Handle, processMask);

			var tasks = new List<Task>();

			foreach (int i in Enumerable.Range(1, 4))
			{
				tasks.Add(Task.Factory.StartNew(DoSomethingProcessorIntensive, i));
			}

			Console.WriteLine("Just pretend this is doing something useful...");

			Task.WaitAll(tasks.ToArray());
		}

		private static void DoSomethingProcessorIntensive(object state)
		{
			const int matrixRows = 256;
			const int matrixColumns = 256;
			int id = (int)state;

			Random rng = new Random();

			RunWithThreadAffinity(() => 
			{
				// matrix multiply should be fairly expensive.
				while (true)
				{
					var m1 = RandomMatrix(matrixRows, matrixColumns, rng);
					var m2 = RandomMatrix(matrixRows, matrixColumns, rng);

					#region super-secret

					if ((id % 2) == 0)
					{
						Thread.Sleep(500);
					}

					#endregion

					var m3 = m1.Multiply(m2);
					var m4 = m2.Multiply(m3);
					var m5 = m3.Multiply(m4);
				}
			}, id);
		}

		private static void RunWithThreadAffinity(Action action, int id)
		{
			try
			{
				Thread.BeginThreadAffinity();

				if (!IsThreadAFiber())
				{
					uint nativeThreadId = GetCurrentThreadId();
					uint lowestBit = (uint)(id & (uint)(-(int)id));
					SetThreadAffinityMask(nativeThreadId, (processMask & lowestBit));
				}

				action();
			}
			finally
			{
				Thread.EndThreadAffinity();
			}
		}

		private static Matrix RandomMatrix(int matrixRows, int matrixColumns, Random rng)
		{
			Matrix m = new Matrix(matrixRows, matrixColumns);

			for (int i = 0; i < matrixRows; i++)
			{
				for (int j = 0; j < matrixColumns; j++)
				{
					m[i, j] = rng.Next() * rng.NextDouble();
				}
			}

			return m;
		}
	}
}