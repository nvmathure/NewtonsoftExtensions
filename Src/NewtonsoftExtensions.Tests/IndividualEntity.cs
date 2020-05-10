namespace CloudNDevOps.Newtonsoft.Extensions.Tests
{
    public class IndividualEntity<TType> : Entity<TType>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
