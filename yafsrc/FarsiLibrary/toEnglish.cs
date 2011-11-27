namespace FarsiLibrary
{
	/// <summary>
	/// Helper class to convert numbers to it's farsi equivalent. Use this class' methods to overcome a problem in displaying farsi numeric values.
	/// </summary>
	public sealed class toEnglish
	{
		/// <summary>
		/// Converts a Farsi number to it's English numeric values.
		/// </summary>
		/// <remarks>This method only converts the numbers in a string, and does not convert any non-numeric characters.</remarks>
		/// <param name="FarsiNumber"></param>
		/// <returns></returns>
		static public string Convert(string FarsiNumber)
		{
            string numFarsi = string.Empty;
			string numTemp;

			for(int i=0;i<FarsiNumber.Length;i++) 
			{
				numTemp = FarsiNumber.Substring(i,1);
				switch(numTemp) 
				{
					case "۰" : 
						numFarsi = numFarsi + "0";
						break;
					case "۱" : 
						numFarsi = numFarsi + "1";
						break;
					case "۲" :
						numFarsi = numFarsi + "2";
						break;
					case "۳" :
						numFarsi = numFarsi + "3"; 
						break;
					case "۴" :
						numFarsi = numFarsi + "4";
						break;
					case "۵" : 
						numFarsi = numFarsi + "5";
						break;
					case "۶" : 
						numFarsi = numFarsi + "6";
						break;
					case "۷" : 
						numFarsi = numFarsi + "7";
						break;
					case "۸" : 
						numFarsi = numFarsi + "8";
						break;
					case "۹" : 
						numFarsi = numFarsi + "9";
						break;
					default :
						numFarsi = numFarsi + numTemp;
						break;
				}
			}
			return(numFarsi);
		}
	}
}
