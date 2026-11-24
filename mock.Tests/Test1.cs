using Microsoft.AspNetCore.Mvc;
using mock.depart.Controllers;
using mock.depart.Models;
using mock.depart.Services;
using mock.Exceptions;
using Moq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
namespace mock.Tests
{
    [TestClass]
    public sealed class Test1
    {

        private Mock<CatsService> serviceMock;
        private Mock<CatsController> controller;

        [TestInitialize]
        public void Init()
        {
            serviceMock = new Mock<CatsService>();
            controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };
            controller.Setup(c => c.UserId).Returns("user1");
        }


        [TestMethod]
        public void Test_NotFound()
        {

            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(value: null);

            var result = controller.Object.DeleteCat(1);

            NotFoundResult? notFound = result.Result as NotFoundResult;
            Assert.IsNotNull(notFound);


        }

        [TestMethod]
        public void Test_BadRequest()
        {

            controller.Setup(c => c.UserId).Returns("1");

            Cat cat = new Cat
            {
                Id = 5,
                CatOwner = new CatOwner { Id = "2" }
            };

            serviceMock.Setup(s => s.Get(5)).Returns(cat);

            var result = controller.Object.DeleteCat(5);

            BadRequestObjectResult? bad = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(bad);
            Assert.AreEqual("Cat is not yours", bad.Value);

        }

        [TestMethod]
        public void Test_DeleteOk()
        {

            controller.Setup(c => c.UserId).Returns("2");

            Cat cat = new Cat
            {
                Id = 5,
                Name = "Felix",
                CuteLevel = Cuteness.BarelyOk,
                CatOwner = new CatOwner { Id = "2" }
            };

            serviceMock.Setup(s => s.Get(5)).Returns(cat);
            serviceMock.Setup(s => s.Delete(5)).Returns(cat); 

            var result = controller.Object.DeleteCat(5);

            OkObjectResult? ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(cat, ok.Value);

        }

        [TestMethod]
        public void Test_TooCute()
        {

            controller.Setup(c => c.UserId).Returns("2");

            Cat cat = new Cat
            {
                Id = 5,
                Name = "Felix",
                CuteLevel = Cuteness.Amazing,
                CatOwner = new CatOwner { Id = "2" }
            };

            serviceMock.Setup(s => s.Get(5)).Returns(cat);


            var result = controller.Object.DeleteCat(5);
            BadRequestObjectResult? bad = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(bad);
            Assert.AreEqual("Cat is too cute", bad.Value);

        }

        [TestMethod]
        public void Test_Unauthorized()
        {
            
            controller.Setup(c => c.UserId).Returns("1");
            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Throws(new Code18Exception());

      
            var resultat = controller.Object.DeleteCat(5);

    
            UnauthorizedResult? unauthorized = resultat.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorized);


        }


        [TestMethod]
        public void Test_ChiffreChanceux()
        {
            serviceMock.SetupSequence(c => c.ChiffreChanceux()).Returns(33).Returns(42).Returns(67);

            var r1 = controller.Object.ChiffreChanceux();
            var r2 = controller.Object.ChiffreChanceux();
            var r3 = controller.Object.ChiffreChanceux();

            OkObjectResult? ok1 = r1.Result as OkObjectResult;
            Assert.IsNotNull(ok1);
            Assert.AreEqual(33, ok1.Value);

            OkObjectResult? ok2 = r2.Result as OkObjectResult;
            Assert.IsNotNull(ok2);
            Assert.AreEqual(42, ok2.Value);

            OkObjectResult? ok3 = r3.Result as OkObjectResult;
            Assert.IsNotNull(ok3);
            Assert.AreEqual(67, ok3.Value);
        }
    }
}
