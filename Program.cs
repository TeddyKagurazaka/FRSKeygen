using System;

namespace FRSKeygen
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string Serial = Generator.Generate();
            Console.WriteLine(Serial);
            //return;
            Console.WriteLine("=============");

            Console.WriteLine("Input:" + Serial);
            Console.WriteLine("Checksum:" + (Checksum(Serial) ? "Passed" : "Failed"));
            var Output = GetAuthorizationObj(Serial);
            Console.WriteLine(Output.Output());
        }

        public static NeofaceLicense GetAuthorizationObj(string code)
        {
            NeofaceLicense license = null;
            try
            {
                if (code.Length != 0x37)
                {
                    return null;
                }
                if (code.Split(new char[] { '-' }).Length == 0)
                {
                    return null;
                }
                string encoded = "";
                foreach (string str3 in code.Substring(4).Split(new char[] { '-' }))
                {
                    //Console.WriteLine(encoded);
                    encoded = encoded + str3;
                }
                Console.WriteLine(encoded);
                string[] strArray2 = Decrypt(encoded).Split(new char[] { '-' });
                if (strArray2.Length != 5)
                {
                    return null;
                }
                license = new NeofaceLicense (
                    strArray2[0],
                    strArray2[1],
                    strArray2[2],
                    strArray2[3],
                    strArray2[4],
                    code
                );
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return null;
            }
            return license;
        }

        public static string Decrypt(string encoded)
        {
            char[] chArray = encoded.Replace("-", "").ToCharArray();
            string str = "";
            string code = "";
            string str3 = "";
            for (int i = 0; i < chArray.Length; i++)
            {
                if ((i > 0) && (i < 0x20))
                {
                    if ((i % 2) == 1)
                    {
                        code = code + chArray[i].ToString();
                    }
                    if ((i % 2) == 0)
                    {
                        str = str + chArray[i].ToString();
                    }
                }
                if (i == 0)
                {
                    str = str + chArray[i].ToString();
                }
                if (i > 0x1f)
                {
                    code = code + chArray[i].ToString();
                }
            }
            Console.WriteLine("str:" + str + ",code:" + code);
            str3 = CharacterToInteger(code);
            Console.WriteLine("decrypted code:" + str3);
            new Configuration();
            //
            string str4 = int.Parse(str3.Substring(0, 5), System.Globalization.NumberStyles.HexNumber).ToString();
            string str5 = int.Parse(str3.Substring(5, 8), System.Globalization.NumberStyles.HexNumber).ToString();
            string str6 = int.Parse(str3.Substring(13, 8), System.Globalization.NumberStyles.HexNumber).ToString();
            string str7 = int.Parse(str3.Substring(21, 7), System.Globalization.NumberStyles.HexNumber).ToString();
            //Name:str
            //Max Camera:str4
            //Max Enrollment:str5
            //Max MatchingSize:str6
            //Issue Date:str7
            return (str + "-" + str4 + "-" + str5 + "-" + str6 + "-" + str7);
        }

        private static string CharacterToInteger(string code)
        {
            string str = "";
            foreach (char ch in code.ToCharArray())
            {
                str = str + ToOriginalCode(ch.ToString());
            }
            return str;
        }

        private static string ToOriginalCode(string code)
        {
            int index = 0;
            string[] strArray2 = new string[] { 
                "S", "T", "U", "V", "W", "X", "Y", "Z", "Q", "R", "A", "B", "C", "D", "E", "F", 
                "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "2", "4", "9", "3", "8", "0", 
                "1", "5", "7", "6"
             };
            string[] strArray4 = new string[] { 
                "2", "4", "9", "3", "8", "0", "1", "5", "7", "6", "S", "T", "U", "V", "W", "X", 
                "Y", "Z", "Q", "R", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", 
                "M", "N", "O", "P"
             };
            for (int i = 0; i < strArray2.Length; i++)
            {
                if (strArray2[i] == code)
                {
                    index = i;
                    break;
                }
            }
            return strArray4[index];
        }
 
        public static bool Checksum(string code){
            string[] strArray = code.Split(new char[] { '-' });
            if (strArray.Length != 8)
            {
                return false;
            }

            string s = CharacterToInteger(strArray[0].Substring(0, 4));
            int result = 0;
            if (!int.TryParse(s, out result))
            {
                return false;
            }
            int num2 = 0;
            foreach (char ch in code.Replace("-", "").Substring(4).ToCharArray())
            {
                num2 += ch;
            }
            Console.WriteLine("Desired result:" + result + ",Calculated:" + num2);
            if (result != num2)
            {
                
                return false;
            }
            return true;

        }
    }

    public class NeofaceLicense
    {
        // Fields
        private string customerName;
        private string issueDate;
        private string key;
        private string maximumCameras;
        private string maximumEnrolmentSize;
        private string maximumMatchingSize;
        private string trailDays;

        public NeofaceLicense(string CustName,string MaxCamera,string MaxEnrolSize,string MaxMatchingSize,string IssDate,string InputKey){
            customerName = CustName;
            maximumCameras = MaxCamera;
            maximumEnrolmentSize = MaxEnrolSize;
            maximumMatchingSize = MaxMatchingSize;
            issueDate = IssDate;
            key = InputKey;
        }

        public string Output()
        {
            return "Customer Name(In MD5):" + customerName + "\nMax Camera:" + maximumCameras + "\nMax Enrollment:" + maximumEnrolmentSize + "\nMax MatchingSize:" + maximumMatchingSize + "\nIssue Date:" + issueDate + "\nSerial:" + key;
        }
    }



 

}
