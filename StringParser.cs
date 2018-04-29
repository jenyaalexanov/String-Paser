public static class StringParser
{
    public static bool TryParse(string str)
    {
        try
        {
            Parse(str);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
 
    public static double Parse(string str)
    {
        string[] func = {"sin", "cos", "ctan", "tan"};
        for (int i = 0; i < func.Length; i++)
        {
            Match matchFunc = Regex.Match(str, string.Format(@"{0}\(({1})\)", func[i], @"[1234567890\.\+\-\*\/^%]*"));
            if (matchFunc.Groups.Count > 1)
            {
                string inner = matchFunc.Groups[0].Value.Substring(1 + func[i].Length, matchFunc.Groups[0].Value.Trim().Length - 2 - func[i].Length);
                string left = str.Substring(0, matchFunc.Index);
                string right = str.Substring(matchFunc.Index + matchFunc.Length);
 
                switch (i)
                {
                    case 0:
                        return Parse(left + Math.Sin(Parse(inner)) + right);
 
                    case 1:
                        return Parse(left + Math.Cos(Parse(inner)) + right);
 
                    case 2:
                        return Parse(left + Math.Tan(Parse(inner)) + right);
 
                    case 3:
                        return Parse(left + 1.0 / Math.Tan(Parse(inner)) + right);
                }
            }
        }
 
        Match matchSk = Regex.Match(str, string.Format(@"\(({0})\)", @"[1234567890\.\+\-\*\/^%]*"));
        if (matchSk.Groups.Count > 1)
        {
            string inner = matchSk.Groups[0].Value.Substring(1, matchSk.Groups[0].Value.Trim().Length - 2);
            string left = str.Substring(0, matchSk.Index);
            string right = str.Substring(matchSk.Index + matchSk.Length);
            return Parse(left + Parse(inner) + right);
        }
 
        Match matchMulOp = Regex.Match(str, string.Format(@"({0})\s?({1})\s?({0})\s?", RegexNum, RegexMulOp));
        Match matchAddOp = Regex.Match(str, string.Format(@"({0})\s?({1})\s?({2})\s?", RegexNum, RegexAddOp, RegexNum));
        var match = (matchMulOp.Groups.Count > 1) ? matchMulOp : (matchAddOp.Groups.Count > 1) ? matchAddOp : null;
        if (match != null)
        {
            string left = str.Substring(0, match.Index);
            string right = str.Substring(match.Index + match.Length);
            string val = ParseAct(match).ToString(CultureInfo.InvariantCulture);
            return Parse(string.Format("{0}{1}{2}", left, val, right));
        }
 
        try
        {
            return double.Parse(str, CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            throw new FormatException(string.Format("Bad string '{0}'", str));
        }
    }
 
    private const string RegexNum = @"[-]?\d+\.?\d*";
    private const string RegexMulOp = @"[\*\/^%]";
    private const string RegexAddOp = @"[\+\-]";
 
    private static double ParseAct(Match match)
    {
        double a = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        double b = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
 
        switch (match.Groups[2].Value)
        {
            case "+":
                return a + b;
 
            case "-":
                return a - b;
 
            case "*":
                return a * b;
 
            case "/":
                return a / b;
 
            case "^":
                return Math.Pow(a, b);
 
            case "%":
                return a % b;
 
            default:
                throw new FormatException(string.Format("Bad string '{0}'", match.Value));
        }
    }
}