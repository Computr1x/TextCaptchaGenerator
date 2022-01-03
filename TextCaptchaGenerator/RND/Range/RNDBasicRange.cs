
namespace TextCaptchaGenerator.RND.Range{
    public class RNDBasicRange <T> where T: struct {
        public T Max {get;set;}

        public T Min {get;set;}

        public RNDBasicRange(T min, T max){
            Min = min;
            Max = max;
        }

        public RNDBasicRange(T length){
            Min = length;
            Max = length;
        }
    }
}