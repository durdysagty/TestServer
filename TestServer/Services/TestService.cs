using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestModels;
using TestServer.Interfaces;

namespace TestServer.Services
{
    internal class TestService : ITest
    {
        private readonly IRepository _repository;
        private readonly CaptchaImage _captchaImage;
        private const int byteSizeToTest = 300;
        public TestService(IRepository repositoryService)
        {
            _repository = repositoryService;
            _captchaImage = new CaptchaImage();
        }

        public async Task<Test> GetTest(string userName)
        {
            Array values = Enum.GetValues(typeof(TestType));
            Random random = new();
            TestType testType = (TestType)values.GetValue(random.Next(values.Length));
            Test test = null;
            UsedTest usedTest = null;
            User user = await _repository.FindUserByNameAsync(userName);
            switch (testType)
            {
                case TestType.MultipleChoice:
                    IList<int> answers = new List<int>();
                    for (int i = 0; i < 5; i++)
                    {
                        answers.Add(random.Next(1000));
                    }
                    int choiceIndex = random.Next(answers.Count);
                    int choice = answers[choiceIndex] + random.Next(5);
                    IList<int> rightAnswers = answers.Where(a => a < choice).ToList();
                    usedTest = new()
                    {
                        UserId = user.Id,
                        TestType = testType,
                        RightAnswers = string.Join(", ", rightAnswers.OrderBy(ra => ra)),
                    };
                    await _repository.AddUsedTestAsync(usedTest);
                    test = new()
                    {
                        TestType = testType,
                        Numbers = answers,
                        Text = $"Выберите все цифры меньше {choice}"
                    };
                    break;
                case TestType.Sequence:
                    IList<int> sequence = new List<int>();
                    int first = random.Next(20);
                    int add = random.Next(9);
                    sequence.Add(first);
                    int next = first;
                    for (int i = 1; i < 5; i++)
                    {
                        next += add;
                        sequence.Add(next);
                    }
                    IList<int> nextTwo = new List<int>();
                    for (int i = 0; i < 2; i++)
                    {
                        next += add;
                        nextTwo.Add(next);
                    }
                    usedTest = new()
                    {
                        UserId = user.Id,
                        TestType = testType,
                        RightAnswers = string.Join(", ", nextTwo),
                    };
                    await _repository.AddUsedTestAsync(usedTest);
                    test = new()
                    {
                        TestType = testType,
                        Numbers = sequence,
                        Text = "Допишите цифры в последовательности"
                    };
                    break;
                default:
                    IList<int> numbers = new List<int>();
                    for (int i = 0; i < 5; i++)
                        numbers.Add(random.Next(999, 9999));
                    int randomIndex = random.Next(3, 5);
                    int randomIndex2 = random.Next(0, 3);
                    IList<byte[]> images = new List<byte[]>();
                    IList<string> selectedImages = new List<string>();
                    for (int i = 0; i < 5; i++)
                    {
                        byte[] image = _captchaImage.GenerateCaptcha(numbers[i].ToString());
                        images.Add(image);
                        if (i == randomIndex)
                            selectedImages.Add(Convert.ToBase64String(image.Take(byteSizeToTest).ToArray()));
                        if (i == randomIndex2)
                            selectedImages.Add(Convert.ToBase64String(image.TakeLast(byteSizeToTest).ToArray()));
                    }
                    usedTest = new()
                    {
                        UserId = user.Id,
                        TestType = testType,
                        RightAnswers = string.Join(", ", selectedImages.OrderBy(s => s)),
                    };
                    await _repository.AddUsedTestAsync(usedTest);
                    test = new()
                    {
                        TestType = testType,
                        Images = images,
                        Text = $"Выберите картинки с этими цифрами - {numbers[randomIndex]}, {numbers[randomIndex2]}"
                    };
                    break;
            }
            return test;
        }

        public async Task<bool> IsTestPassed(UserAnswer userAnswer)
        {
            User user = await _repository.FindUserByNameAsync(userAnswer.Name);
            if (user == null)
                return false;
            UsedTest usedTest = await _repository.GetUserLastTest(user.Id);
            if (usedTest.TestType == TestType.SelectImages)
            {
                IList<string> selectedImages = new List<string>
                {
                    Convert.ToBase64String(userAnswer.Images[0].Take(byteSizeToTest).ToArray()),
                    Convert.ToBase64String(userAnswer.Images[1].TakeLast(byteSizeToTest).ToArray())
                };
                usedTest.UserAnswers = string.Join(", ", selectedImages.OrderBy(s => s));
                if (usedTest.UserAnswers != usedTest.RightAnswers)
                    return false;
            }
            else
            {
                usedTest.UserAnswers = string.Join(", ", userAnswer.Answer.OrderBy(ra => ra));
                await _repository.SaveUserLastTestAsync(usedTest);
                if (usedTest.UserAnswers != usedTest.RightAnswers)
                    return false;
            }
            user.IsTestPassed = true;
            await _repository.SaveUserChangesAsync(user);
            return true;
        }
    }
}
