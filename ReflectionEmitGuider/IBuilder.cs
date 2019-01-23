namespace ReflectionEmitGuider {
    public interface IBuilder {
        int FirstNum { get; }
        int SecondNum { get; }
        float Sum(int firstnum, int secondnum);
        float Substract(int firstnum, int secondnum);
        float Multiply(int firstnum, int secondnum);
        float Divide(int firstnum, int secondnum);
    }
}