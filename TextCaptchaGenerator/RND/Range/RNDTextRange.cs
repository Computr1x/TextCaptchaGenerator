using System.Text;
using System.Linq;

namespace TextCaptchaGenerator.RND.Range{
    public class RNDTextRange {
        public static readonly string punctuation = "!\"#$%&'()*+, -./:;<=>?@[\\]^_`{|}~";
        public static readonly string asciiLowerCase = "abcdefghijklmnopqrstuvwxyz";
        public static readonly string asciiUpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly string asciiLetters = asciiLowerCase + asciiUpperCase;
        public static readonly string digits = "0123456789";
        public static readonly string hexDigits = "0123456789abcdefABCDEF";
        public static readonly string octDigits = "01234567";

        public char[] Chars {get;set;}
        public RNDBasicRange<int> TextLengthRange {get;set;}

        public bool GenerateMode {get;}

        public string[] Words {get;set;}
        public RNDTextRange(string chars, RNDBasicRange<int> textLengthRange){
            Chars = chars.ToArray().Distinct().ToArray();
            TextLengthRange = textLengthRange;
            GenerateMode = true;
        }

        public RNDTextRange(char[] chars, RNDBasicRange<int> textLengthRange){
            Chars = chars.Distinct().ToArray();
            TextLengthRange = textLengthRange;
            GenerateMode = true;
        }

        public RNDTextRange(string[] words){    
            Words = words;
            GenerateMode = false;
        }
    }
}