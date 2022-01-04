
namespace TextCaptchaGenerator.RND.Range{
    public class RNDSizeRange <T> where T: struct {
        public RNDBasicRange<T> widthRange {get;set;}
        public RNDBasicRange<T> heightRange {get;set;}        

        public RNDSizeRange(T width, T height){
            widthRange = new(width);
            heightRange = new(height);
        }

        public RNDSizeRange(RNDBasicRange<T> widthRange, RNDBasicRange<T> heightRange)
        {
            this.widthRange = widthRange;
            this.heightRange = heightRange;
        }
    }
}