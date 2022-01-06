namespace MyTestApp.Model
{
    public class BankAccount
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string Patronymic { get; set; }
        public string? Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal Balance { get; set; }

    }
}
