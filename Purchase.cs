namespace ADO
{
    public class Purchase
    {
        public string ID { get; set; }
        public string Email { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public Purchase(string id, string email, string productCode, string productName)
        {
            ID = id;
            Email = email;
            ProductCode = productCode;
            ProductName = productName;
        }

        public Purchase() { }
    }
}
