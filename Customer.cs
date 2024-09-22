namespace ADO
{
    public class Customer
    {
        public string ID { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Customer(string ID, string Surname, string Name, string Patronymic,
            string PhoneNumber, string Email)
        {
            this.ID = ID;
            this.Surname = Surname;
            this.Name = Name;
            this.Patronymic = Patronymic;
            this.PhoneNumber = PhoneNumber;
            this.Email = Email;
        }

        public Customer() { }
    }
}
