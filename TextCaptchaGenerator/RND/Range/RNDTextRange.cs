using System.Text;
using System.Linq;

namespace TextCaptchaGenerator.RND.Range{
    public class RNDTextRange {
        public static string punctuation = "!\"#$%&'()*+, -./:;<=>?@[\\]^_`{|}~";
        public static string asciiLowerCase = "abcdefghijklmnopqrstuvwxyz";
        public static string asciiUpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string asciiLetters = asciiLowerCase + asciiUpperCase;
        public static string digits = "0123456789";
        public static string hexdigits = "0123456789abcdefABCDEF";
        public static string octdigits = "01234567";

        public char[] Chars {get;set;}
        public RNDBasicRange<int> TextLengthRange {get;set;}
        public RNDTextRange(string chars, RNDBasicRange<int> textLengthRange){
            Chars = chars.ToArray().Distinct().ToArray();
            TextLengthRange = textLengthRange;
        }

        public RNDTextRange(char[] chars, RNDBasicRange<int> textLengthRange){
            Chars = chars.Distinct().ToArray();
            TextLengthRange = textLengthRange;
        }
    }
}