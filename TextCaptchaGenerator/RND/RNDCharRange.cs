using System.Text;

namespace TextCaptchaGenerator.RND {
    public class RNDCharRange {
        public char[] Chars {get;set;}

        public RNDCharRange(string chars){
            Chars = chars.ToCharArray();
        }

        public RNDCharRange(char[] chars){
            Chars = chars;
        }
    }
}