namespace DuqSchut.Models
{
    public class EmailTemplate
    {
        public int ID { get; set; }
        public string Name  { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public TemplateType Type { get; set; }
    }
}