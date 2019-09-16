using MatthiWare.CommandLine.Core.Attributes;

namespace FluentValidationsExtensions.Tests.Models
{
    public class EmailModel
    {
        [Required, Name("e")]
        public string Email { get; set; }

        [Required, Name("i")]
        public int Id { get; set; }
    }
}
