using System.ComponentModel.DataAnnotations;

namespace TestModels
{
    public enum TestType { MultipleChoice, Sequence, SelectImages }
    public class Test
    {
        public TestType TestType { get; set; }
        public string Text { get; set; }
        public IList<int> Numbers { get; set; }
        public IList<byte[]> Images { get; set; }
    }

    public class UsedTest
    {
        public int Id { get; set; }
        public TestType TestType { get; set; }
        [MaxLength(1000)]
        public string RightAnswers { get; set; }
        [MaxLength(1000)]
        public string UserAnswers { get; set; }
        public bool IsTestPassed { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
