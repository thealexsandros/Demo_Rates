using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlexSandros.DemoApps.Rates.Test
{
	[TestClass]
	public class StorageTest
	{
		static IConfiguration m_Config;
		static IParser m_Parser;

		[ClassInitialize]
		public static void Init(TestContext ctx)
		{
			m_Config = new ConfigurationStub();
			m_Parser = new Parser(m_Config);
		}
		[TestMethod]
		public void StorageIsWorking()
		{
			var storage = Storage.Instance;
			Assert.IsNull(storage[DateTime.Today], "Empty storage returns not null");
			Assert.IsFalse(storage.TryAddYear(2017, null, m_Parser, m_Config, null), "Successfully added with invalid params");
			var lines2016 = new[]
			{
				"Data|1 GHJ|100500 QWE", "01.Jul 2016|1.1|2.1"
			};
			Assert.IsTrue(storage.TryAddYear(2016, lines2016, m_Parser, m_Config, null), "Unable to add with valid params");
			var rates2016 = new YearRates(2016, lines2016, m_Config, m_Parser, null);
			Assert.IsNull(storage[new DateTime(2015, 12, 31)]?.ToArray(),
				"Value before storage is invalid");
			Assert.IsTrue(
				rates2016[new DateTime(2016, 01, 01)].SequenceEqual(
				storage[new DateTime(2016, 01, 01)]),
				"Value at the beginning of storage is invalid");
			Assert.IsTrue(
				rates2016[new DateTime(2016, 07, 01)].SequenceEqual(
				storage[new DateTime(2016, 07, 01)]),
				"Value in the middle of storage is invalid");
			Assert.IsTrue(
				rates2016[new DateTime(2017, 03, 01)].SequenceEqual(
				storage[new DateTime(2017, 03, 01)]),
				"Value in the end of storage is invalid");
			Assert.IsFalse(storage.TryAddYear(2016, lines2016, m_Parser, m_Config, null), "Successfully added duplicate");
			var lines2017 = new[]
			{
				"Data|1 GHQ|100500 QWE", "01.Feb 2017|3.1|4.1"
			};
			var rates2017 = new YearRates(2017, lines2017, m_Config, m_Parser, null);
			Assert.IsTrue(storage.TryAddYear(2017, lines2017, m_Parser, m_Config, null), "Unable to add with valid params #2");
			Assert.IsTrue(
				rates2017[new DateTime(2017, 03, 01)].SequenceEqual(
				storage[new DateTime(2017, 03, 01)]),
				"Value haven't changed after add");
		}
	}
}