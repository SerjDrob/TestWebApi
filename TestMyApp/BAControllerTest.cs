using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTestApp.Controllers;
using MyTestApp.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TestMyApp
{
    public class BAControllerTest
    {
        private readonly DbContextOptions<BankAccountContext> _dbContextOptions;
        private Random rand;
        const int ThreadCount = 10;
        const int AccountsCount = 50;
        public BAControllerTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BankAccountContext>()
                .UseInMemoryDatabase(databaseName: "TestBank")
                .Options;
            rand = new Random();
        }
        DateTime RandomDay(Random random)
        {
            DateTime start = new DateTime(1965, 1, 1);
            int range = (new DateTime(1996, 1, 1) - start).Days;
            return start.AddDays(random.Next(range));
        }
        [SetUp]
        public void Setup()
        {
            var persons = new List<string>(File.ReadAllLines("../../../Names.txt"));
            var randDay = new Random();
            var controller = new BankAccountController(new BankAccountContext(_dbContextOptions));
            for (int i = 0; i < AccountsCount; i++)
            {
                var num = rand.Next(0, 99 - i);
                var person = persons[num].Split(' ');
                persons.RemoveAt(num);
                var ba = new BankAccount
                {
                    Name = person[0],
                    Patronymic = person[1],
                    Surname = person[2],
                    BirthDate = RandomDay(randDay)
                };
                controller.PostBankAccount(ba).ContinueWith(res =>
                {
                    var result = res.Result;
                    Assert.IsInstanceOf<CreatedAtActionResult>(result);
                });
            }
        }

        [Test]
        public void PutNWithdarawReturnNoContent()
        {
            using (var context = new BankAccountContext(_dbContextOptions))
            {
                var controller = new BankAccountController(context);
                controller.GetBankAccounts().ContinueWith(res =>
                {
                    var list = new List<BankAccount>(res.Result.Value!);
                    Assert.That(list.Count == AccountsCount);
                });
            }


            rand = new Random();
            var putMoneyList = new List<Task>();
            var withdrawMoneyList = new List<Task>();

            for (int i = 0; i < ThreadCount; i++)
            {
                putMoneyList.Add(new Task(async () =>
                {
                    for (int id = 1; id < AccountsCount + 1; id++)
                    {
                        using var context = new BankAccountContext(_dbContextOptions);
                        var controller = new BankAccountController(context);
                        var result = await controller.PutMoney(id, 100);
                        Assert.IsInstanceOf<NoContentResult>(result);
                    }
                }));

                withdrawMoneyList.Add(new Task(async () =>
                {
                    for (int id = 1; id < AccountsCount + 1; id++)
                    {
                        using var context = new BankAccountContext(_dbContextOptions);
                        var controller = new BankAccountController(context);
                        var result = await controller.WithdrawMoney(id, 10);                       
                        Assert.IsInstanceOf<NoContentResult>(result);

                    }
                }));
            }
            putMoneyList.ForEach(t => t.Start());
            Task.WaitAll(putMoneyList.ToArray());
            withdrawMoneyList.ForEach(t => t.Start());
            Task.WaitAll(withdrawMoneyList.ToArray());
        }
        [Test]
        public void WithdarawReturnObjectResult()
        {            
            rand = new Random();
            using var context = new BankAccountContext(_dbContextOptions);
            var controller = new BankAccountController(context);
            controller.WithdrawMoney(1, 1000000).ContinueWith(res =>
            {
                var result = res.Result;
                Assert.IsInstanceOf<ObjectResult>(result);
            });
        }
        [Test]
        public void WithdarawReturnBadRequest()
        {
            rand = new Random();
            using var context = new BankAccountContext(_dbContextOptions);
            var controller = new BankAccountController(context);
            controller.WithdrawMoney(-1, 1000000).ContinueWith(res =>
            {
                var result = res.Result;
                Assert.IsInstanceOf<BadRequestResult>(result);
            });
        }
        [Test]
        public void PutReturnBadRequest()
        {
            rand = new Random();
            using var context = new BankAccountContext(_dbContextOptions);
            var controller = new BankAccountController(context);
            controller.PutMoney(-1, 1000000).ContinueWith(res =>
            {
                var result = res.Result;
                Assert.IsInstanceOf<BadRequestResult>(result);
            });
        }
    }
}