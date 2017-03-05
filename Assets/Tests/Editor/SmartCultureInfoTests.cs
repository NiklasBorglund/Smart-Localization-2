
namespace SmartLocalization.Editor
{
	using NUnit.Framework;

	[TestFixture]
	public class SmartCultureInfoTests 
	{
		[Test]
		public void AddCultureToCollection_Success()
		{
			SmartCultureInfoCollection testCollection = new SmartCultureInfoCollection();
			testCollection.AddCultureInfo(new SmartCultureInfo("te-ST", "TEST", "TEEEST", false));

			Assert.AreEqual(1, testCollection.cultureInfos.Count);
		}

		[Test]
		public void AddCultureToCollection_Failure()
		{
			SmartCultureInfoCollection testCollection = new SmartCultureInfoCollection();
			testCollection.AddCultureInfo(null);
			
			Assert.AreEqual(0, testCollection.cultureInfos.Count);
		}

		[Test]
		public void FindCultureInCollection_Success()
		{
			SmartCultureInfoCollection testCollection = new SmartCultureInfoCollection();
			SmartCultureInfo cultureInfoToFind = new SmartCultureInfo("te-ST", "TEST", "TEEEST", false);
			testCollection.AddCultureInfo(cultureInfoToFind);

			SmartCultureInfo foundCultureInfo = testCollection.FindCulture(cultureInfoToFind);
			Assert.AreEqual(cultureInfoToFind, foundCultureInfo);
		}
		
		[Test]
		public void FindCulture_Success()
		{
			var testCollection = new SmartCultureInfoCollection();
			var chineseOne = new SmartCultureInfo("zh-CHT", "Chinese", "Chinese", false);
			var chineseTwo = new SmartCultureInfo("zh-CHS", "Chinese", "Chinese", false);
			
			testCollection.AddCultureInfo(chineseTwo);
			testCollection.AddCultureInfo(chineseOne);
			
			var foundCulture = testCollection.FindCulture(chineseTwo);
			
			Assert.AreEqual(foundCulture, chineseTwo);
		}
	}
}