namespace some.other.module
{
	public class SomePropertyType
	{
		public object Value { get; set; }
	}

	public class SomeClass
	{
		public SomePropertyType Foo { get; set; }
		public SomePropertyType Bar { get; set; }
		public SomePropertyType Baz { get; set; }
	}
}
