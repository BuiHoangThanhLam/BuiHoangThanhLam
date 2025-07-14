namespace MvcMovie.Models.Process
{
    public class AutoGenerateId
    {
        public string GenerateId(string inputID, int prefixLength)
        {
            string strOutput = "";
            //lay phan text cua inputID
            string prefix = inputID.Substring(0, prefixLength);
            //lay phan so cua inputID
            string numberPart = inputID.Substring(prefixLength);
            //chuyen so thanh so nguyen
            int number = int.Parse(numberPart);
            //tang so len 1 don vi
            number++;
            //chuyen so ve chuoi
            strOutput = prefix + number.ToString();
            return strOutput;
        }
        public string GenerateId(string inputID)
        {
            //STD008
            var match = System.Text.RegularExpressions.Regex.Match(inputID, @"^(?<prefix>[A-Za-z]+)(?<number>\d+)$");
            if (!match.Success)
            {
                throw new ArgumentException("Invalid id format");
            }
            string prefix = match.Groups["prefix"].Value;
            //STD
            string numberPart = match.Groups["number"].Value;
            //008
            int number = int.Parse(numberPart) + 1;
            //9
            string newNumberPart = number.ToString().PadLeft(numberPart.Length, '0');
            //STD009
            return prefix + newNumberPart;
        }
    }
}