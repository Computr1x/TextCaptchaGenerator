using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace TextCaptchaGenerator.RND {
    public class RNDFontFamilyRange {
        private static string[] allFonts;
        private static HashSet<string> allFontsSet;
        public string[] FontFamilies {get;private set;}

        private static void InitListFonts(){
            allFonts = SKFontManager.CreateDefault().GetFontFamilies();
            allFontsSet = new HashSet<string>(allFonts.Select(x => x.ToLower()));
        }

        static RNDFontFamilyRange(){
            InitListFonts();
        }


        public RNDFontFamilyRange(){
            FontFamilies = allFonts;
        }

        public RNDFontFamilyRange(IEnumerable<string> fontFamilies){
            List<string> fonts = new();
            foreach(string font in fontFamilies)
                if(allFontsSet.Contains(font.ToLower()))
                    fonts.Add(font);
            FontFamilies = fonts.ToArray();
        }

        public RNDFontFamilyRange(string[] fontFamilies) {
            List<string> fonts = new();
            foreach(string font in fontFamilies)
                if(allFontsSet.Contains(font.ToLower()))
                    fonts.Add(font);
            FontFamilies = fonts.ToArray();
        }
    }
}