using System;

namespace ReflectionEmitGuider {
    public class Builder {
        #region Event
        public delegate void BuilderDelegate(string message);

        public event BuilderDelegate InvokeMessage;

        public virtual void OnInvokeMessage(string message) {
            if (this.InvokeMessage != null)
                this.InvokeMessage(message);
        }
        #endregion

        #region Fields
        private int firstNum, secondNum;
        public int FirstNum {
            get { return this.firstNum; }
            set { this.firstNum = value; }
        }

        public int SecondNum {
            get { return this.secondNum; }
            set { this.secondNum = value; }
        }

        #endregion

        #region Constructors
        public Builder(int firstnum, int secondnum) {
            this.FirstNum = firstnum;
            this.SecondNum = secondnum;
        }
        #endregion

        #region IBuilder Members

        public float Sum(int firstnum, int secondnum) {
            return firstnum + secondnum;
        }

        public float Substract(int firstnum, int secondnum) {
            return firstnum - secondnum;
        }

        public float Multiply(int firstnum, int secondnum) {
            return firstnum * secondnum;
        }

        public float Divide(int firstnum, int secondnum) {
            try {
                return firstnum / secondnum;
            } catch (DivideByZeroException ex) {
                Console.WriteLine("ZeroDivide exception : {0}", ex.Message);
                return 0;
            }
        }

        #endregion

        #region Methods

        public float GetProduct() {
            return this.Multiply(this.FirstNum, this.secondNum);
        }
        public override string ToString() {
            return string.Format("FirstNum : {0}, SecondNum : {1}", this.FirstNum, this.SecondNum);
        }

        #endregion
    }
}