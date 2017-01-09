using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlexSandros.DemoApps.Rates.Test
{
	[TestClass]
	public class ParserTest
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
		public void ParseUiYearIsOk()
		{
			var year = m_Parser.ParseUiYear("NOW", "NOW");
			Assert.AreEqual((short)DateTime.Today.Year, year, "Year is different");
			Assert.AreEqual((short)2005, m_Parser.ParseUiYear("2005", "j_J"), "Year is not 2005");
			Assert.AreEqual((short)2006, m_Parser.ParseUiYear("2006 ", "j_J"), "Year is not 2006");
			Assert.AreEqual((short)2007, m_Parser.ParseUiYear(" 2007 ", "j_J"), "Year is not 2007");
			Assert.AreEqual(null, m_Parser.ParseUiYear("200T", "j_J"), "Year is not null (200T)");
			Assert.AreEqual(null, m_Parser.ParseUiYear("0", "j_J"), "Year is not null (zero)");
			Assert.AreEqual(null, m_Parser.ParseUiYear("10000", "j_J"), "Year is not null (10000)");
			Assert.AreEqual(null, m_Parser.ParseUiYear("-1", "j_J"), "Year is not null (-1)");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseUiYearNullNowStringThrowsException()
		{
			m_Parser.ParseUiYear("NOW", null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseUiYearEmptyNowStringThrowsException()
		{
			m_Parser.ParseUiYear("NOW", string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseUiYearWhitespaceNowStringThrowsException()
		{
			m_Parser.ParseUiYear("NOW", "       ");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseUiDateTimeNullNowStringThrowsException()
		{
			m_Parser.ParseUiDateTime("NOW", null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseUiDateTimeEmptyNowStringThrowsException()
		{
			m_Parser.ParseUiDateTime("NOW", string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseUiDateTimeWhitespaceNowStringThrowsException()
		{
			m_Parser.ParseUiDateTime("NOW", "       ");
		}

		[TestMethod]
		public void ParseUiDateTimeIsOk()
		{
			var date = m_Parser.ParseUiDateTime("NOW", "NOW");
			Assert.AreEqual(DateTime.Today, date, "Date is different");
			Assert.AreEqual(new DateTime(2005,1,1), m_Parser.ParseUiDateTime("01.01.2005", "j_J"), "Date is not 01.01.2005");
			Assert.AreEqual(null, m_Parser.ParseUiDateTime("01.01.2006 ", "j_J"), "Date is not null (trailing space)");
			Assert.AreEqual(null, m_Parser.ParseUiDateTime(" 01.01.2007 ", "j_J"), "Date is not null (leading and trailing spaces)");
			Assert.AreEqual(null, m_Parser.ParseUiDateTime("01.01.200T", "j_J"), "Date is not null (200T)");
			Assert.AreEqual(null, m_Parser.ParseUiDateTime("01.01.0000", "j_J"), "Date is not null (zero)");
			Assert.AreEqual(null, m_Parser.ParseUiDateTime("01.01.10000", "j_J"), "Date is not null (10000)");
			Assert.AreEqual(null, m_Parser.ParseUiDateTime("01.01.-0001", "j_J"), "Date is not null (-1)");
			Assert.AreEqual(null, m_Parser.ParseUiDateTime("29.02.2017", "j_J"), "Date is not null (29.02.2017)");
			Assert.AreEqual(null, m_Parser.ParseUiDateTime("01/01/2017", "j_J"), "Date is not null (01/01/2017)");
		}

		[TestMethod]
		public void ParseFileDateTimeIsOk()
		{
			Assert.AreEqual(new DateTime(2005, 1, 1), m_Parser.ParseFileDateTime("01.Jan 2005"), "Date is not 01.01.2005");
			Assert.AreEqual(null, m_Parser.ParseFileDateTime("01.Jan 2006 "), "Date is not null (trailing space)");
			Assert.AreEqual(null, m_Parser.ParseFileDateTime(" 01.Jan 2007 "), "Date is not null (leading and trailing spaces)");
			Assert.AreEqual(null, m_Parser.ParseFileDateTime("01.Jan 200T"), "Date is not null (200T)");
			Assert.AreEqual(null, m_Parser.ParseFileDateTime("01.Jan 0000"), "Date is not null (zero)");
			Assert.AreEqual(null, m_Parser.ParseFileDateTime("01.Jan 10000"), "Date is not null (10000)");
			Assert.AreEqual(null, m_Parser.ParseFileDateTime("01.Jan -0001"), "Date is not null (-1)");
			Assert.AreEqual(null, m_Parser.ParseFileDateTime("29.Feb 2017"), "Date is not null (29.Feb.2017)");
			Assert.AreEqual(null, m_Parser.ParseFileDateTime("02.01.2017"), "Date is not null (02.01.2017)");
			Assert.AreEqual(null, m_Parser.ParseFileDateTime("03.Jan.2017"), "Date is not null (03.Jan.2017)");
		}

		[TestMethod]
		public void ParseFloatIsOk()
		{
			Assert.AreEqual(1.1f, m_Parser.ParseFloat("1.1"), "Float is not 1.1");
			Assert.AreEqual(null, m_Parser.ParseFloat("1,1"), "Float is not null (comma)");
			Assert.AreEqual(1.2f, m_Parser.ParseFloat("1.2 "), "Float is not 1.2");
			Assert.AreEqual(1.3f, m_Parser.ParseFloat(" 1.3 "), "Float is not 1.3");
		}

	}
}