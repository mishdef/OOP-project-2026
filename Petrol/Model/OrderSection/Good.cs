namespace gsst.Model
{
    public class Good : Product
    {
        private double _price;
        private string _barCode;

        public byte[] Image { get; set; }
        public override double Price 
        { 
            get => _price;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Price cannot be negative.");
                }
                if (value == 0)
                {
                    throw new ArgumentException("Price cannot be zero.");
                }
                _price = value;
            }
        }
        public string BarCode { get; set; }
    }
}
