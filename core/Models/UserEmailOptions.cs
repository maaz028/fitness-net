namespace core.Models
{
    public class UserEmailOptions
    {
        public UserEmailOptions()
        {
            Email = new List<string>();
        }

        public List<string>? Email { get; set; }

        public string? Subject { get; set; }

        public string? Body { get; set; }

        public List<KeyValuePair<string, string>>? KeyValuePairs { get; set; }
    }
}
