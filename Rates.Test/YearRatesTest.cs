using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlexSandros.DemoApps.Rates.Test
{
	[TestClass]
	public class YearRatesTest
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
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullLinesThrowsException()
		{
			new YearRates(1, null, m_Config, m_Parser, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullParserThrowsException()
		{
			new YearRates(1, new[] {string.Empty}, m_Config, null, null);
		}

		[TestMethod]
		[ExpectedException(typeof (ArgumentException))]
		public void EmptyLinesThrowsException()
		{
			new YearRates(1, new string[] {}, m_Config, m_Parser, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void EmptyCurrenciesThrowsException()
		{
			new YearRates(1, new [] { string.Empty }, m_Config, m_Parser, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void CurrenciesWithoutSeparatorsThrowsException()
		{
			new YearRates(1, new[] { "TEST" }, m_Config, m_Parser, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void NoRatesThrowsException()
		{
			new YearRates(1, new [] { "Date|1 TST" }, m_Config, m_Parser, null);
		}

		[TestMethod]
		public void SimpleLineIsStoredCorrect()
		{
			var rates = new YearRates(2017, new[] {"Date|1 TST|1 TRY", "01.Jan 2017|13.01|15.01"}, m_Config, m_Parser, null);
			var result = rates[new DateTime(2017, 1, 1)]?.ToArray();
			Assert.IsNotNull(result, "Result is null");
			Assert.AreEqual(2, result.Length, "Result length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1 is incorrect");
			Assert.AreEqual(13.01f, result[0].Value, "Value #0 is incorrect");
			Assert.AreEqual(15.01f, result[1].Value, "Value #1 is incorrect");
		}

		[TestMethod]
		public void NextDateIsRetrievedCorrect()
		{
			var rates = new YearRates(2017, new[] { "Date|1 TST|1 TRY", "01.Jan 2017|13.01|15.01" }, m_Config, m_Parser, null);
			var result = rates[new DateTime(2017, 1, 2)]?.ToArray();
			Assert.IsNotNull(result, "Result is null");
			Assert.AreEqual(2, result.Length, "Result length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1 is incorrect");
			Assert.AreEqual(13.01f, result[0].Value, "Value #0 is incorrect");
			Assert.AreEqual(15.01f, result[1].Value, "Value #1 is incorrect");
		}

		[TestMethod]
		public void PrevYearDateIsRetrievedCorrect()
		{
			var rates = new YearRates(2017, new[] { "Date|1 TST|1 TRY", "01.Jan 2017|13.01|15.01" }, m_Config, m_Parser, null);
			var result = rates[new DateTime(2015, 12, 31)]?.ToArray();
			Assert.IsNull(result, "Result is not null");
		}

		[TestMethod]
		public void ThisYearPrevDateIsRetrievedCorrect()
		{
			var rates = new YearRates(2017, new[] { "Date|1 TST|1 TRY", "02.Jan 2017|13.01|15.01" }, m_Config, m_Parser, null);
			var result = rates[new DateTime(2017, 1, 1)]?.ToArray();
			Assert.IsNotNull(result, "Result is null");
			Assert.AreEqual(2, result.Length, "Result length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1 is incorrect");
			Assert.AreEqual(float.NaN, result[0].Value, "Value #0 is incorrect");
			Assert.AreEqual(float.NaN, result[1].Value, "Value #1 is incorrect");
		}

		[TestMethod]
		public void InvalidDateDoesNotSaveRates()
		{
			var rates = new YearRates(2017,
				new[]
				{
					"Date|1 TST|1 TRY",
					"01.Jan 2017|13.01|15.01",
					"32.Jan 2017|17.01|19.01"
				}, m_Config, m_Parser, null);
			var result = rates[new DateTime(2017, 3, 1)]?.ToArray();
			Assert.IsNotNull(result, "Result is null");
			Assert.AreEqual(2, result.Length, "Result length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1 is incorrect");
			Assert.AreEqual(13.01f, result[0].Value, "Value #0 is incorrect");
			Assert.AreEqual(15.01f, result[1].Value, "Value #1 is incorrect");
		}

		[TestMethod]
		public void SameDateIsStoredOnce()
		{
			var rates = new YearRates(2017, 
				new[]
				{
					"Date|1 TST|1 TRY",
					"01.Jan 2017|13.01|15.01",
					"01.Jan 2017|17.01|19.01"
				}, m_Config, m_Parser, null);
			var result = rates[new DateTime(2017, 1, 1)]?.ToArray();
			Assert.IsNotNull(result, "Result is null");
			Assert.AreEqual(2, result.Length, "Result length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1 is incorrect");
			Assert.AreEqual(13.01f, result[0].Value, "Value #0 is incorrect");
			Assert.AreEqual(15.01f, result[1].Value, "Value #1 is incorrect");
		}

		[TestMethod]
		public void DateFromOtherYearIsIgnored()
		{
			var rates = new YearRates(2017,
				new[]
				{
					"Date|1 TST|1 TRY",
					"01.Jan 2017|13.01|15.01",
					"01.Jan 2018|17.01|19.01"
				}, m_Config, m_Parser, null);
			var result = rates[new DateTime(2018, 1, 2)]?.ToArray();
			Assert.IsNotNull(result, "Result is null");
			Assert.AreEqual(2, result.Length, "Result length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1 is incorrect");
			Assert.AreEqual(13.01f, result[0].Value, "Value #0 is incorrect");
			Assert.AreEqual(15.01f, result[1].Value, "Value #1 is incorrect");
		}

		[TestMethod]
		public void ShortLineIsFilledWithNaN()
		{
			var rates = new YearRates(2017,
				new[]
				{
					"Date|1 TST|1 TRY",
					"01.Jan 2017|13.01"
				}, m_Config, m_Parser, null);
			var result = rates[new DateTime(2017, 1, 1)]?.ToArray();
			Assert.IsNotNull(result, "Result is null");
			Assert.AreEqual(2, result.Length, "Result length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1 is incorrect");
			Assert.AreEqual(13.01f, result[0].Value, "Value #0 is incorrect");
			Assert.AreEqual(float.NaN, result[1].Value, "Value #1 is incorrect");
		}

		[TestMethod]
		public void LongLineIsIgnored()
		{
			var rates = new YearRates(2017,
				new[]
				{
					"Date|1 TST|1 TRY",
					"01.Jan 2017|13.01|15.01|17.01"
				}, m_Config, m_Parser, null);
			var result = rates[new DateTime(2017, 1, 1)]?.ToArray();
			Assert.IsNotNull(result, "Result is null");
			Assert.AreEqual(2, result.Length, "Result length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1 is incorrect");
			Assert.AreEqual(13.01f, result[0].Value, "Value #0 is incorrect");
			Assert.AreEqual(15.01f, result[1].Value, "Value #1 is incorrect");
		}

		[TestMethod]
		public void InvalidFloatIsTreatedAsNaN()
		{
			var rates = new YearRates(2017,
				new[]
				{
					"Date|1 TST|1 TRY",
					"01.Jan 2017|13.01|15,01"
				}, m_Config, m_Parser, null);
			var result = rates[new DateTime(2017, 1, 1)]?.ToArray();
			Assert.IsNotNull(result, "Result is null");
			Assert.AreEqual(2, result.Length, "Result length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1 is incorrect");
			Assert.AreEqual(13.01f, result[0].Value, "Value #0 is incorrect");
			Assert.AreEqual(float.NaN, result[1].Value, "Value #1 is incorrect");
		}

		[TestMethod] public void MultipleLinesAreStoredCorrectly()
		{
			var rates = new YearRates(2017,
				new[]
				{
					"Date|1 TST|1 TRY",
					"01.Jan 2017|13.01|15.01",
					"01.Mar 2017|17.01|19.01",
					"01.Jun 2017|21.01|23.01"
				}, m_Config, m_Parser, null);
			var result = rates[new DateTime(2017, 1, 31)]?.ToArray();
			Assert.IsNotNull(result, "Result #1 is null");
			Assert.AreEqual(2, result.Length, "Result #1 length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #1-0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #1-1 is incorrect");
			Assert.AreEqual(13.01f, result[0].Value, "Value #1-0 is incorrect");
			Assert.AreEqual(15.01f, result[1].Value, "Value #1-1 is incorrect");

			result = rates[new DateTime(2017, 3, 1)]?.ToArray();
			Assert.IsNotNull(result, "Result #2 is null");
			Assert.AreEqual(2, result.Length, "Result #2 length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #2-0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #2-1 is incorrect");
			Assert.AreEqual(17.01f, result[0].Value, "Value #2-0 is incorrect");
			Assert.AreEqual(19.01f, result[1].Value, "Value #2-1 is incorrect");

			result = rates[new DateTime(2018, 12, 31)]?.ToArray();
			Assert.IsNotNull(result, "Result #3 is null");
			Assert.AreEqual(2, result.Length, "Result #3 length is incorrect");
			Assert.AreEqual("1 TST", result[0].Key, "Key #3-0 is incorrect");
			Assert.AreEqual("1 TRY", result[1].Key, "Key #3-1 is incorrect");
			Assert.AreEqual(21.01f, result[0].Value, "Value #3-0 is incorrect");
			Assert.AreEqual(23.01f, result[1].Value, "Value #3-1 is incorrect");
		}
	}
}
