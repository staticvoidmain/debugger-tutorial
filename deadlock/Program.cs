using System;
using System.Threading;
using System.Threading.Tasks;

namespace deadlock
{
	public class Program
	{
		private static object lockOne = new object();
		private static object lockTwo = new object();

		public static void Main(string[] args)
		{
			ManualResetEvent mre = new ManualResetEvent(false);

			var task1 = StartFirstTask(mre);
			var task2 = StartSecondTask(mre);

			mre.Set();

			Task.WaitAll(task1, task2);

			Console.WriteLine("This line will not be reached.");
		}

		private static Task StartSecondTask(ManualResetEvent mre)
		{
			var task2 = Task.Factory.StartNew(() =>
			{
				mre.WaitOne();

				lock (lockOne)
				{
					Console.WriteLine("Task-2 entered lock#1");

					Thread.Sleep(100);

					lock (lockTwo)
					{
						Console.WriteLine("Task-2 entered lock#2!");
					}
				}
			});

			return task2;
		}

		private static Task StartFirstTask(ManualResetEvent mre)
		{
			var task1 = Task.Factory.StartNew(() =>
			{
				mre.WaitOne();

				lock (lockTwo)
				{
					Console.WriteLine("Task-1 entered lock#2");

					Thread.Sleep(100);

					lock (lockOne)
					{
						Console.WriteLine("Task-1 entered lock#1!");
					}
				}
			});

			return task1;
		}
	}
}
