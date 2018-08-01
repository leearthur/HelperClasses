namespace HelperClasses.Tests.ObjectMapper
{
    public class BasicSourceClass
    {
        public int Identifier { get; set; }
        public string FullName { get; set; }
    }

    public class BasicDestinationClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
