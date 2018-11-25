namespace MatthiWare.CommandLine.Abstractions.Models
{
    public struct ArgumentModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public bool HasValue => Value != null;

        public ArgumentModel(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public ArgumentModel(string key)
        {
            this.Key = key;
            this.Value = null;
        }

    }
}
