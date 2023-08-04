using System.ComponentModel.DataAnnotations;

namespace TestModels
{
    public class User
    {

        public virtual int Id { get; set; }
        [MaxLength(50), Required]
        public virtual string Name { get; set; }
        public virtual bool IsTestPassed { get; set; }
        public ICollection<UsedTest> UsedTests { get; set; }
    }
}