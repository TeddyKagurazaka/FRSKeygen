using System;
using System.Security.Cryptography;
using System.Text;
namespace FRSKeygen
{
    public static class Generator
    {
        static string customerName = "DUMMY";
        static string issueDate = "20160101";
        //static string key;
        static string maximumCameras = "1024";
        static string maximumEnrolmentSize = "20000";
        static string maximumMatchingSize = "20000";

        public static string Generate()
        {
            Console.WriteLine("Customer Name:" + customerName +
                              "\nMax Camera:" + maximumCameras +
                              "\nMax Enrollment:" + maximumEnrolmentSize +
                              "\nMax MatchingSize:" + maximumMatchingSize +
                              "\nIssue Date:" + issueDate + "\n");

            MD5CryptoServiceProvider NameHasher = new MD5CryptoServiceProvider();
            var NameHash = NameHasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(customerName));
            string NameHashStr = "";
            foreach (byte DetailByte in NameHash)
                NameHashStr += DetailByte.ToString("X2");

            NameHashStr = NameHashStr.Substring(8, 16);
            Console.WriteLine("Name Hash:" + NameHashStr);

            string MaxCameraHex = int.Parse(maximumCameras).ToString("X5");
            string MaxEnrollHex = int.Parse(maximumEnrolmentSize).ToString("X8");
            string MaxMatchHex = int.Parse(maximumMatchingSize).ToString("X8");
            string DateHex = int.Parse(issueDate).ToString("X");
            Console.WriteLine("Key Hash:" + MaxCameraHex + " " + MaxEnrollHex + " " + MaxMatchHex + " " + DateHex);

            string OriginalCode = MaxCameraHex + MaxEnrollHex + MaxMatchHex + DateHex;
            string SerialCode = "";
            for (int i = 0; i < OriginalCode.Length;i++){
                SerialCode += ToEncryptedCode(OriginalCode.Substring(i, 1));   
            }
            Console.WriteLine("Crypted Key:" +SerialCode);

            //byte[] SerialCodeByte = Encoding.UTF8.GetBytes(SerialCode);
            //byte[] NameByte = Encoding.UTF8.GetBytes(NameHashStr);
            string FinalCode = "";

            int SerialCodeCount = 0;
            int NameByteCount = 0;

            for (int q = 0; q < 44;q++){
                if(q % 2 == 0 && q < 32)
                {
                    FinalCode += NameHashStr.Substring(NameByteCount, 1);
                    NameByteCount++;
                }else{
                    FinalCode += SerialCode.Substring(SerialCodeCount, 1);
                    SerialCodeCount++;
                }
            }

            Console.WriteLine("Serial w/o checksum:" + FinalCode);
            string Checksum = ReturnChecksum(FinalCode);
            Console.WriteLine("Checksum:" + Checksum);
            FinalCode = Checksum + FinalCode;

            return FinalCode.Substring(0, 6) + "-" +
                            FinalCode.Substring(6, 6) + "-" +
                            FinalCode.Substring(12, 6) + "-" +
                            FinalCode.Substring(18, 6) + "-" +
                            FinalCode.Substring(24, 6) + "-" +
                            FinalCode.Substring(30, 6) + "-" +
                            FinalCode.Substring(36, 6) + "-" +
                            FinalCode.Substring(42, 6);
        }

        public static string ReturnChecksum(string code){
            int num2 = 0;
            foreach (char ch in code.Replace("-", ""))
            {
                num2 += ch;
            }
            Console.WriteLine("Calculated Checksum:" + num2);
            string OriginalChecksum = num2.ToString();
            Console.WriteLine("Original Checksum:" + OriginalChecksum);
            string EncryptedChecksum = "";
            for (int i = 0; i < OriginalChecksum.Length; i++)
            {
                EncryptedChecksum += ToEncryptedCode(OriginalChecksum.Substring(i, 1));
            }
            return EncryptedChecksum;
        }


        private static string ToEncryptedCode(string code)
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
                if (strArray4[i] == code)
                {
                    index = i;
                    break;
                }
            }
            return strArray2[index];
        }
    }

    public class Configuration
    {
        // Fields
        public const int CamerasLength = 5;
        public const int EnrolmentSizeLength = 8;
        public const int IssueDateLength = 7;
        public const int KeyHeaderLength = 4;
        public const int KeyLength = 0x37;
        public const int KeySegmentCount = 8;
        public const int MacLength = 0x10;
        public const int MatchingSizeLength = 8;

    }

}

