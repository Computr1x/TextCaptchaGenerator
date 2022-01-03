using System;
using System.Linq;

namespace TextCaptchaGenerator.RND.Range{
    public class RNDEnumRange <T> where T: struct {
        public T[] EnumValues {get;set;}
        public RNDEnumRange(){
            EnumValues = (T[])Enum.GetValues (typeof (T));
        }
    }
}