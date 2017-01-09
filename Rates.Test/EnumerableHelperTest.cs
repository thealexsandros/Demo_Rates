using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlexSandros.DemoApps.Rates.Test
{
	[TestClass]
	public class EnumerableHelperTest
	{
		[TestMethod]
		public void ReadLinesReturnsEmptySequenceOnEmptyProvider()
		{
			Assert.AreEqual(0, EnumerableHelper.ReadLines(null).Count(), "Sequence is not empty");
		}

		[TestMethod]
		public void ReadLinesReturnsEmptySequenceOnProviderWithEmptyResult()
		{
			Assert.AreEqual(0, EnumerableHelper.ReadLines(() => null).Count(), "Sequence is not empty");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SplitStringThrowsExceptionOnEmptyConfig()
		{
			EnumerableHelper.SplitStringByConfig(string.Empty, null).ToArray();
		}

		[TestMethod]
		public void SplitStringWorksCorrect()
		{
			var config = new ConfigurationStub();
			var sep = new[] {config.Separator};
			var strings =
				new[]
				{
					null,
					string.Empty,
					"bobo",
					$"bobo{config.Separator}jojo",
					$"bobo{config.Separator}jojo{config.Separator}",
					$"bobo{config.Separator}jojo{config.Separator}keke"
				};
			foreach (var s in strings)
			{
				var helperResult = EnumerableHelper.SplitStringByConfig(s, config).ToArray();
				var splitResult = s?.Split(sep, StringSplitOptions.None) ?? new string[0];
				Assert.IsTrue(helperResult.SequenceEqual(splitResult), $"Results for string [{s}] are not equal");
			}
		}
	}
}