
namespace TextCaptchaGenerator.RND {
    public class RNDBasicRange <T> where T: struct {
        public T High {get;set;}

        public T Low {get;set;}

        public RNDBasicRange(T low, T high){
            Low = low;
            High = high;
        }
    }
}