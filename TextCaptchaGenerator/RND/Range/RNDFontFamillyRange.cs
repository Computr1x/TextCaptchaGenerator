using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace TextCaptchaGenerator.RND.Range{
    public class RNDFontFamilyRange {
        private static string[] allFonts;
        private static HashSet<string> allFontsSet;
        public string[] FontFamilies {get;private set;}

        private static void InitListFonts(){
            SKFontManager fm = SKFontManager.CreateDefault();
            var allFontsList = fm.GetFontFamilies().Distinct().ToList();
            for(int i = 0; i < allFontsList.Count; i++){
                SKTypeface typeface = SKTypeface.FromFamilyName(
                    allFontsList[i]);
                
                if(!typeface.ContainsGlyph(RNDTextRange.asciiLetters[0]))
                    allFontsList.RemoveAt(i--);
            }
            allFonts = allFontsList.ToArray();
            allFontsSet = new HashSet<string>(allFonts.Select(x => x.ToLower()));
        }

        static RNDFontFamilyRange(){
            InitListFonts();
        }


        public RNDFontFamilyRange(){
            FontFamilies = allFonts;
        }

        public RNDFontFamilyRange(IEnumerable<string> fontFamilies){
            FontFamilies = fontFamilies.Where(font => allFontsSet.Contains(font.ToLower())).ToArray();
        }

        public RNDFontFamilyRange(string[] fontFamilies) {
            FontFamilies = fontFamilies.Where(font => allFontsSet.Contains(font.ToLower())).ToArray();
        }
    }
}