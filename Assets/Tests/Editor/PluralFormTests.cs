using UnityEngine;
using System.Collections;
using NUnit.Framework;

/// <summary>
/// Plural form tests. Information on this site was used to create the tests - https://developer.mozilla.org/en-US/docs/Mozilla/Localization/Localization_and_Plurals
/// </summary>
using System;


namespace SmartLocalization.Editor
{
	[TestFixture]
	public class PluralFormTests 
	{
		const string baseKey = "TestKey";
		const string baseKeyZero = "TestKey_0";
		const string baseKeyOne = "TestKey_1";
		const string baseKeyTwo = "TestKey_2";
		const string baseKeyThree = "TestKey_3";
		const string baseKeyFour = "TestKey_4";
		const string baseKeyFive = "TestKey_5";
		
		[Test]
		public void TestPlural_ar()
		{
			TestArabic("ar");
		}
		
		[Test]
		public void TestPlural_ar_SA()
		{
			TestArabic("ar-SA");
		}
		
		[Test]
		public void TestPlural_ar_IQ()
		{
			TestArabic("ar-IQ");
		}
		
		[Test]
		public void TestPlural_ar_EG()
		{
			TestArabic("ar-EG");
		}
		
		[Test]
		public void TestPlural_ar_LY()
		{
			TestArabic("ar-LY");
		}
		
		[Test]
		public void TestPlural_ar_DZ()
		{
			TestArabic("ar-DZ");
		}
		
		[Test]
		public void TestPlural_ar_MA()
		{
			TestArabic("ar-MA");
		}
		
		[Test]
		public void TestPlural_ar_TN()
		{
			TestArabic("ar-TN");
		}
		
		[Test]
		public void TestPlural_ar_OM()
		{
			TestArabic("ar-OM");
		}
		
		[Test]
		public void TestPlural_ar_YE()
		{
			TestArabic("ar-YE");
		}
		
		[Test]
		public void TestPlural_ar_SY()
		{
			TestArabic("ar-SY");
		}
		
		[Test]
		public void TestPlural_ar_JO()
		{
			TestArabic("ar-JO");
		}
		
		[Test]
		public void TestPlural_ar_LB()
		{
			TestArabic("ar-LB");
		}
		
		[Test]
		public void TestPlural_ar_KW()
		{
			TestArabic("ar-KW");
		}
		
		[Test]
		public void TestPlural_ar_AE()
		{
			TestArabic("ar-AE");
		}
		
		[Test]
		public void TestPlural_ar_BH()
		{
			TestArabic("ar-BH");
		}
		
		[Test]
		public void TestPlural_ar_QA()
		{
			TestArabic("ar-QA");
		}
		
		[Test]
		public void TestPlural_bg()
		{
			TestTwoFormLanguage("bg");
		}
		
		[Test]
		public void TestPlural_bg_BG()
		{
			TestTwoFormLanguage("bg-BG");
		}
		
		[Test]
		public void TestPlural_ca()
		{
			TestTwoFormLanguage("ca");
		}
		
		[Test]
		public void TestPlural_ca_ES()
		{
			TestTwoFormLanguage("ca-ES");
		}
		
		[Test]
		public void TestPlural_zh_CHS()
		{
			TestOneFormLanguage("zh-CHS");
		}
		
		[Test]
		public void TestPlural_zh_CN()
		{
			TestOneFormLanguage("zh-CN");
		}
		
		[Test]
		public void TestPlural_zh_HK()
		{
			TestOneFormLanguage("zh-HK");
		}
		
		[Test]
		public void TestPlural_zh_SG()
		{
			TestOneFormLanguage("zh-SG");
		}
		
		[Test]
		public void TestPlural_zh_MO()
		{
			TestOneFormLanguage("zh-MO");
		}
		
		[Test]
		public void TestPlural_zh_CHT()
		{
			TestOneFormLanguage("zh-CHT");
		}
		
		[Test]
		public void TestPlural_cs()
		{
			TestThreeFormSlavicCzechSlovakia("cs");
		}
		
		[Test]
		public void TestPlural_cs_CZ()
		{
			TestThreeFormSlavicCzechSlovakia("cs-CZ");
		}
		
		[Test]
		public void TestPlural_da()
		{
			TestTwoFormLanguage("da");
		}
		
		[Test]
		public void TestPlural_da_DK()
		{
			TestTwoFormLanguage("da-DK");
		}
		
		[Test]
		public void TestPlural_de()
		{
			TestTwoFormLanguage("de");
		}
		
		[Test]
		public void TestPlural_de_DE()
		{
			TestTwoFormLanguage("de-DE");
		}
		
		[Test]
		public void TestPlural_de_CH()
		{
			TestTwoFormLanguage("de-CH");
		}
		
		[Test]
		public void TestPlural_de_AT()
		{
			TestTwoFormLanguage("de-AT");
		}
		
		[Test]
		public void TestPlural_de_LU()
		{
			TestTwoFormLanguage("de-LU");
		}
		
		[Test]
		public void TestPlural_el()
		{
			TestTwoFormLanguage("el");
		}
		
		[Test]
		public void TestPlural_el_GR()
		{
			TestTwoFormLanguage("el-GR");
		}
		
		[Test]
		public void TestPlural_en()
		{
			TestTwoFormLanguage("en");
		}
		
		[Test]
		public void TestPlural_en_US()
		{
			TestTwoFormLanguage("en-US");
		}
		
		[Test]
		public void TestPlural_en_GB()
		{
			TestTwoFormLanguage("en-GB");
		}
		
		[Test]
		public void TestPlural_en_AU()
		{
			TestTwoFormLanguage("en-AU");
		}
		
		[Test]
		public void TestPlural_en_CA()
		{
			TestTwoFormLanguage("en-CA");
		}
		
		[Test]
		public void TestPlural_en_NZ()
		{
			TestTwoFormLanguage("en-NZ");
		}
		
		[Test]
		public void TestPlural_en_IE()
		{
			TestTwoFormLanguage("en-IE");
		}
		
		[Test]
		public void TestPlural_en_ZA()
		{
			TestTwoFormLanguage("en-ZA");
		}
		
		[Test]
		public void TestPlural_en_TT()
		{
			TestTwoFormLanguage("en-TT");
		}
		
		[Test]
		public void TestPlural_en_ZW()
		{
			TestTwoFormLanguage("en-ZW");
		}
		
		[Test]
		public void TestPlural_en_PH()
		{
			TestTwoFormLanguage("en-PH");
		}
		
		[Test]
		public void TestPlural_es()
		{
			TestTwoFormLanguage("es");
		}
		
		[Test]
		public void TestPlural_es_MX()
		{
			TestTwoFormLanguage("es-MX");
		}
		
		[Test]
		public void TestPlural_es_ES()
		{
			TestTwoFormLanguage("es-ES");
		}
		
		[Test]
		public void TestPlural_es_GT()
		{
			TestTwoFormLanguage("es-GT");
		}
		
		[Test]
		public void TestPlural_es_CR()
		{
			TestTwoFormLanguage("es-CR");
		}
		
		[Test]
		public void TestPlural_es_PA()
		{
			TestTwoFormLanguage("es-PA");
		}
		
		[Test]
		public void TestPlural_es_DO()
		{
			TestTwoFormLanguage("es-DO");
		}
		
		[Test]
		public void TestPlural_es_VE()
		{
			TestTwoFormLanguage("es-VE");
		}
		
		[Test]
		public void TestPlural_es_CO()
		{
			TestTwoFormLanguage("es-CO");
		}
		
		[Test]
		public void TestPlural_es_PE()
		{
			TestTwoFormLanguage("es-PE");
		}
		
		[Test]
		public void TestPlural_es_AR()
		{
			TestTwoFormLanguage("es-AR");
		}
		
		[Test]
		public void TestPlural_es_EC()
		{
			TestTwoFormLanguage("es-EC");
		}
		
		[Test]
		public void TestPlural_es_CL()
		{
			TestTwoFormLanguage("es-CL");
		}
		
		[Test]
		public void TestPlural_es_UY()
		{
			TestTwoFormLanguage("es-UY");
		}
		
		[Test]
		public void TestPlural_es_PY()
		{
			TestTwoFormLanguage("es-PY");
		}
		
		[Test]
		public void TestPlural_es_BO()
		{
			TestTwoFormLanguage("es-BO");
		}
		
		[Test]
		public void TestPlural_es_HN()
		{
			TestTwoFormLanguage("es-HN");
		}
		
		[Test]
		public void TestPlural_es_NI()
		{
			TestTwoFormLanguage("es-NI");
		}
		
		[Test]
		public void TestPlural_es_PR()
		{
			TestTwoFormLanguage("es-PR");
		}
		
		[Test]
		public void TestPlural_fi()
		{
			TestTwoFormLanguage("fi");
		}
		
		[Test]
		public void TestPlural_fi_FI()
		{
			TestTwoFormLanguage("fi-FI");
		}
		
		[Test]
		public void TestPlural_fr()
		{
			TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish("fr");
		}
		
		[Test]
		public void TestPlural_fr_FR()
		{
			TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish("fr-FR");
		}
		
		[Test]
		public void TestPlural_fr_BE()
		{
			TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish("fr-BE");
		}
		
		[Test]
		public void TestPlural_fr_CA()
		{
			TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish("fr-CA");
		}
		
		[Test]
		public void TestPlural_fr_CH()
		{
			TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish("fr-CH");
		}
		
		[Test]
		public void TestPlural_fr_LU()
		{
			TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish("fr-LU");
		}
		
		[Test]
		public void TestPlural_he()
		{
			TestTwoFormLanguage("he");
		}
		
		[Test]
		public void TestPlural_he_IL()
		{
			TestTwoFormLanguage("he-IL");
		}
		
		[Test]
		public void TestPlural_hu()
		{
			TestTwoFormLanguage("hu");
		}
		
		[Test]
		public void TestPlural_hu_HU()
		{
			TestTwoFormLanguage("hu-HU");
		}
		
		[Test]
		public void TestPlural_is()
		{
			TestTwoFormIcelandic("is");
		}
		
		[Test]
		public void TestPlural_is_IS()
		{
			TestTwoFormIcelandic("is-IS");
		}
		
		[Test]
		public void TestPlural_it()
		{
			TestTwoFormLanguage("it");
		}
		
		[Test]
		public void TestPlural_it_IT()
		{
			TestTwoFormLanguage("it-IT");
		}
		
		[Test]
		public void TestPlural_it_CH()
		{
			TestTwoFormLanguage("it-CH");
		}
		
		[Test]
		public void TestPlural_ja()
		{
			TestOneFormLanguage("ja");
		}
		
		[Test]
		public void TestPlural_ja_JP()
		{
			TestOneFormLanguage("ja-JP");
		}
		
		[Test]
		public void TestPlural_ko()
		{
			TestOneFormLanguage("ko");
		}
		
		[Test]
		public void TestPlural_ko_KR()
		{
			TestOneFormLanguage("ko-KR");
		}
		
		[Test]
		public void TestPlural_nl()
		{
			TestTwoFormLanguage("nl");
		}
		
		[Test]
		public void TestPlural_nl_NL()
		{
			TestTwoFormLanguage("nl-NL");
		}
		
		[Test]
		public void TestPlural_nl_BE()
		{
			TestTwoFormLanguage("nl-BE");
		}
		
		[Test]
		public void TestPlural_no()
		{
			TestTwoFormLanguage("no");
		}
		
		[Test]
		public void TestPlural_nn_NO()
		{
			TestTwoFormLanguage("nn-NO");
		}
		
		[Test]
		public void TestPlural_pl()
		{
			TestThreeFormPolish("pl");
		}
		
		[Test]
		public void TestPlural_pl_PL()
		{
			TestThreeFormPolish("pl-PL");
		}
		
		[Test]
		public void TestPlural_pt()
		{
			TestTwoFormLanguage("pt");
		}
		
		[Test]
		public void TestPlural_pt_PT()
		{
			TestTwoFormLanguage("pt-PT");
		}
		
		[Test]
		public void TestPlural_pt_BR()
		{
			TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish("pt-BR");
		}
		
		[Test]
		public void TestPlural_ro()
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey("ro", baseKey, 1));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey("ro", baseKey, 0));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey("ro", baseKey, 2));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey("ro", baseKey, 19));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey("ro", baseKey, 109));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey("ro", baseKey, 37));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey("ro", baseKey, 59));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey("ro", baseKey, 66));
		}
		
		[Test]
		public void TestPlural_ru()
		{
			TestThreeFormSlavic("ru");
		}
		
		[Test]
		public void TestPlural_ru_RU()
		{
			TestThreeFormSlavic("ru-RU");
		}
		
		[Test]
		public void TestPlural_hr()
		{
			TestThreeFormSlavic("hr");
		}
		
		[Test]
		public void TestPlural_hr_HR()
		{
			TestThreeFormSlavic("hr-HR");
		}
		
		[Test]
		public void TestPlural_sk()
		{
			TestThreeFormSlavicCzechSlovakia("sk");
		}
		
		[Test]
		public void TestPlural_sk_SK()
		{
			TestThreeFormSlavicCzechSlovakia("sk-SK");
		}
		
		[Test]
		public void TestPlural_sq()
		{
			TestTwoFormLanguage("sq");
		}
		
		[Test]
		public void TestPlural_sq_AL()
		{
			TestTwoFormLanguage("sq-AL");
		}
		
		[Test]
		public void TestPlural_sv()
		{
			TestTwoFormLanguage("sv");
		}
		
		[Test]
		public void TestPlural_sv_SE()
		{
			TestTwoFormLanguage("sv-SE");
		}
		
		[Test]
		public void TestPlural_sv_FI()
		{
			TestTwoFormLanguage("sv-FI");
		}
		
		[Test]
		public void TestPlural_th()
		{
			TestOneFormLanguage("th");
		}
		
		[Test]
		public void TestPlural_th_TH()
		{
			TestOneFormLanguage("th-TH");
		}
		
		[Test]
		public void TestPlural_tr()
		{
			TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish("tr");
		}
		
		[Test]
		public void TestPlural_tr_TR()
		{
			TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish("tr-TR");
		}
		
		[Test]
		public void TestPlural_id()
		{
			TestOneFormLanguage("id");
		}
		
		[Test]
		public void TestPlural_id_ID()
		{
			TestOneFormLanguage("id-ID");
		}
		
		[Test]
		public void TestPlural_uk()
		{
			TestThreeFormSlavic("uk");
		}
		
		[Test]
		public void TestPlural_uk_UA()
		{
			TestThreeFormSlavic("uk-UA");
		}
		
		[Test]
		public void TestPlural_be()
		{
			TestThreeFormSlavic("be");
		}
		
		[Test]
		public void TestPlural_be_BY()
		{
			TestThreeFormSlavic("be-BY");
		}
		
		[Test]
		public void TestPlural_sl()
		{
			TestFourFormSlavic("sl");
		}
		
		[Test]
		public void TestPlural_sl_SI()
		{
			TestFourFormSlavic("sl-SI");
		}
		
		[Test]
		public void TestPlural_et()
		{
			TestTwoFormLanguage("et");
		}
		
		[Test]
		public void TestPlural_et_EE()
		{
			TestTwoFormLanguage("et-EE");
		}
		
		[Test]
		public void TestPlural_lv()
		{
			TestThreeFormBalticLatvia("lv");
		}
		
		[Test]
		public void TestPlural_lv_LV()
		{
			TestThreeFormBalticLatvia("lv-LV");
		}
		
		[Test]
		public void TestPlural_lt()
		{
			TestThreeFormBalticLithuania("lt");
		}
		
		[Test]
		public void TestPlural_lt_LT()
		{
			TestThreeFormBalticLithuania("lt-LT");
		}
		
		[Test]
		public void TestPlural_fa()
		{
			TestOneFormLanguage("fa");
		}
		
		[Test]
		public void TestPlural_fa_IR()
		{
			TestOneFormLanguage("fa-IR");
		}
		
		[Test]
		public void TestPlural_vi()
		{
			TestOneFormLanguage("vi");
		}
		
		[Test]
		public void TestPlural_vi_VN()
		{
			TestOneFormLanguage("vi-VN");
		}
		
		[Test]
		public void TestPlural_hy()
		{
			TestTwoFormLanguage("hy");
		}
		
		[Test]
		public void TestPlural_hy_AM()
		{
			TestTwoFormLanguage("hy-AM");
		}
		
		[Test]
		public void TestPlural_eu()
		{
			TestTwoFormLanguage("eu");
		}
		
		[Test]
		public void TestPlural_eu_ES()
		{
			TestTwoFormLanguage("eu-ES");
		}
		
		[Test]
		public void TestPlural_mk()
		{
			TestThreeFormSlavicMacedonia("mk");
		}
		
		[Test]
		public void TestPlural_mk_MK()
		{
			TestThreeFormSlavicMacedonia("mk-MK");
		}
		
		[Test]
		public void TestPlural_af()
		{
			TestTwoFormLanguage("af");
		}
		
		[Test]
		public void TestPlural_af_ZA()
		{
			TestTwoFormLanguage("af-ZA");
		}
		
		[Test]
		public void TestPlural_ka()
		{
			TestOneFormLanguage("ka");
		}
		
		[Test]
		public void TestPlural_ka_GE()
		{
			TestOneFormLanguage("ka-GE");
		}
		
		[Test]
		public void TestPlural_fo()
		{
			TestTwoFormLanguage("fo");
		}
		
		[Test]
		public void TestPlural_fo_FO()
		{
			TestTwoFormLanguage("fo-FO");
		}
		
		[Test]
		public void TestPlural_hi()
		{
			TestTwoFormLanguage("hi");
		}
		
		[Test]
		public void TestPlural_hi_IN()
		{
			TestTwoFormLanguage("hi-IN");
		}
		
		[Test]
		public void TestPlural_sw()
		{
			TestTwoFormLanguage("sw");
		}
		
		[Test]
		public void TestPlural_sw_KE()
		{
			TestTwoFormLanguage("sw-KE");
		}
		
		[Test]
		public void TestPlural_gu()
		{
			TestTwoFormLanguage("gu");
		}
		
		[Test]
		public void TestPlural_gu_IN()
		{
			TestTwoFormLanguage("gu-IN");
		}
		
		[Test]
		public void TestPlural_ta()
		{
			TestTwoFormLanguage("ta");
		}
		
		[Test]
		public void TestPlural_ta_IN()
		{
			TestTwoFormLanguage("ta-IN");
		}
		
		[Test]
		public void TestPlural_te()
		{
			TestTwoFormLanguage("te");
		}
		
		[Test]
		public void TestPlural_te_IN()
		{
			TestTwoFormLanguage("te-IN");
		}
		
		[Test]
		public void TestPlural_kn()
		{
			TestTwoFormLanguage("kn");
		}
		
		[Test]
		public void TestPlural_kn_IN()
		{
			TestTwoFormLanguage("kn-IN");
		}
		
		[Test]
		public void TestPlural_mr()
		{
			TestTwoFormLanguage("mr");
		}
		
		[Test]
		public void TestPlural_mr_IN()
		{
			TestTwoFormLanguage("mr-IN");
		}
		
		[Test]
		public void TestPlural_gl()
		{
			TestTwoFormLanguage("gl");
		}
		
		[Test]
		public void TestPlural_gl_ES()
		{
			TestTwoFormLanguage("gl-ES");
		}
		
		[Test]
		public void TestPlural_kok()
		{
			TestTwoFormLanguage("kok");
		}
		
		[Test]
		public void TestPlural_kok_IN()
		{
			TestTwoFormLanguage("kok-IN");
		}
		
		[Test]
		public void TestPlural_ms()
		{
			TestOneFormLanguage("ms");
		}
		
		[Test]
		public void TestPlural_UnknownLanguage()
		{
			var languageCode = "unknown";
			Assert.AreEqual(baseKey, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreNotEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreNotEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 21));
			Assert.AreNotEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 21));
		}
		
		[Test]
		public void TestPlural_CustomAlgorithm()
		{
			Func<int, int> customAlgorithm = (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5;
			
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(baseKey, 0, customAlgorithm));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(baseKey, 1, customAlgorithm));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(baseKey, 2, customAlgorithm));
			Assert.AreEqual(baseKeyThree, PluralForms.GetPluralKey(baseKey, 3, customAlgorithm));
			Assert.AreEqual(baseKeyThree, PluralForms.GetPluralKey(baseKey, 203, customAlgorithm));
			Assert.AreEqual(baseKeyThree, PluralForms.GetPluralKey(baseKey, 107, customAlgorithm));
			Assert.AreEqual(baseKeyFour, PluralForms.GetPluralKey(baseKey, 11, customAlgorithm));
			Assert.AreEqual(baseKeyFour, PluralForms.GetPluralKey(baseKey, 33, customAlgorithm));
			Assert.AreEqual(baseKeyFive, PluralForms.GetPluralKey(baseKey, 100, customAlgorithm));
			Assert.AreEqual(baseKeyFive, PluralForms.GetPluralKey(baseKey, 202, customAlgorithm));
		}
		
		void TestTwoFormIcelandic(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 21));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 31));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 2));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 11));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 678));
		}
		
		void TestThreeFormPolish(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 2));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 84));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 12));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 14));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 506));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 56));
		}
		
		void TestThreeFormSlavicCzechSlovakia(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 2));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 4));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 500));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 9001));
		}
		
		void TestThreeFormSlavicMacedonia(string languageCode)
		{
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 11));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 2));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 22));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 500));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 9055));
		}
		
		void TestThreeFormBalticLatvia(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 21));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 31));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 500));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 11));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 0));
		}
		
		void TestThreeFormBalticLithuania(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 21));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 31));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 5));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 66));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 0));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 18));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 80));
		}
		
		/// <summary>
		/// Tests the three form slavic.
		/// Slavic (Belarusian, Bosnian, Croatian, Serbian, Russian, Ukrainian)
		/// </summary>
		void TestThreeFormSlavic(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 21));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 121));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 23));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 92));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 144));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 11));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 36));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 75));
		}
		
		/// <summary>
		/// Tests the one form language.
		/// Families: Asian (Chinese, Japanese, Korean), Persian, Turkic/Altaic (Turkish), Thai, Lao, Indonesian, 
		///
		/// Also used with: Georgian
		/// </summary>
		void TestOneFormLanguage(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 0));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 2));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 9001));
		}
		
		/// <summary>
		/// Tests the two form language.
		/// Families: Germanic (Danish, Dutch, English, Faroese, Frisian, German, Norwegian, Swedish), 
		/// Finno-Ugric (Estonian, Finnish, Hungarian), Language isolate (Basque), 
		/// Latin/Greek (Greek), Semitic (Hebrew), Romanic (Italian, Portuguese, Spanish, Catalan), Vietnamese
		///
		/// Note: also using this for Bulgarian, Armenian, Afrikaans
		/// </summary>
		void TestTwoFormLanguage(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 0));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 5555));
		}
		
		/// <summary>
		/// Tests the four form slavic languages
		/// Families: Slavic (Slovenian, Sorbian)
		/// </summary>
		void TestFourFormSlavic(string languageCode)
		{
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 101));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 2));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 102));
			Assert.AreEqual(baseKeyThree, PluralForms.GetPluralKey(languageCode, baseKey, 3));
			Assert.AreEqual(baseKeyThree, PluralForms.GetPluralKey(languageCode, baseKey, 103));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 5));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 5555));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 9006));
		}
		
		/// <summary>
		/// Tests the two form language for Romanic (French, Brazilian Portuguese) languages
		/// </summary>
		void TestTwoFormLanguageFrenchAndBrazilianPortugueseAndTurkish(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 0));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 5555));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 2));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 356));
		}
		
		
		void TestArabic(string languageCode)
		{
			Assert.AreEqual(baseKeyZero, PluralForms.GetPluralKey(languageCode, baseKey, 0));
			Assert.AreEqual(baseKeyOne, PluralForms.GetPluralKey(languageCode, baseKey, 1));
			Assert.AreEqual(baseKeyTwo, PluralForms.GetPluralKey(languageCode, baseKey, 2));
			Assert.AreEqual(baseKeyThree, PluralForms.GetPluralKey(languageCode, baseKey, 3));
			Assert.AreEqual(baseKeyThree, PluralForms.GetPluralKey(languageCode, baseKey, 203));
			Assert.AreEqual(baseKeyThree, PluralForms.GetPluralKey(languageCode, baseKey, 107));
			Assert.AreEqual(baseKeyFour, PluralForms.GetPluralKey(languageCode, baseKey, 11));
			Assert.AreEqual(baseKeyFour, PluralForms.GetPluralKey(languageCode, baseKey, 33));
			Assert.AreEqual(baseKeyFive, PluralForms.GetPluralKey(languageCode, baseKey, 100));
			Assert.AreEqual(baseKeyFive, PluralForms.GetPluralKey(languageCode, baseKey, 202));
		}
		
	}
}