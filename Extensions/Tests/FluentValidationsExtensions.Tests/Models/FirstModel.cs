using MatthiWare.CommandLine.Core.Attributes;

namespace FluentValidationsExtensions.Tests.Models
{
    public class FirstModel
    {
        [Required, Name("f")]
        public string FirstName { get; set; }

        [Required, Name("l")]
        public string LastName { get; set; }
    }
}
