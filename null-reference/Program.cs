using System;
using some.other.module;

namespace null_reference
{
	class Program
	{
		static void Main(string[] args)
		{
			SomeClass root = MakeInstance();
			
			try
			{
				MakeTheStackLookMoreImpressive(root);
			}
			catch (Exception ex) { Console.WriteLine(ex.ToString()); }

			Console.WriteLine("The end of the program.");
			Console.Read();
		}

		private static void MakeTheStackLookMoreImpressive(SomeClass root)
		{
			if (root != null)
			{
				ByAddingMoreLayers(root);
			}
		}

		private static void ByAddingMoreLayers(SomeClass root)
		{
			if (root.Foo.Value == null || root.Bar.Value.ToString() == "foo" || root.Baz.Value != null)
			{
				Console.WriteLine("The code inside the block.");
			}
		}

		#region No Peeking!

		private static SomeClass MakeInstance()
		{
			var root = new SomeClass()
			{
				Foo = new SomePropertyType() { Value = DateTime.Now },
				Bar = new SomePropertyType() { Value = null },
				Baz = new SomePropertyType() { Value = new SomePropertyType() }
			};

			return root;
		}

		#endregion
	}
}