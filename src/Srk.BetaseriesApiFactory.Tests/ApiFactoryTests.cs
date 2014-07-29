
namespace Srk.BetaseriesApiFactory.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text;
    using System.IO;

    [TestClass]
    public class ApiFactoryTests
    {
        private ApiFactoryContext GetContext1()
        {
            var context = new ApiFactoryContext();
            context.Methods.Add(new MethodDescription
            {
                Category = "simple",
                Description = "suepr description de la méthode",
                FullPath = "simple/method",
                Method = "POST",
            });
            var responseMethod = new MethodDescription
            {
                Category = "simple",
                Description = "suepr description de la méthode",
                FullPath = "result/method",
                Method = "POST",
            };
            responseMethod.ResponseFormat = new Response();
            responseMethod.ResponseFormat.Entity = new Entity();
            responseMethod.ResponseFormat.Entity.Name = "comments";
            responseMethod.ResponseFormat.Entity.ItemName = "comment";
            responseMethod.ResponseFormat.Entity.HasMultipleChild = true;
            responseMethod.ResponseFormat.Entity.Fields.Add("id", new EntityField("id", EntityFieldType.Integer, null));
            responseMethod.ResponseFormat.Entity.Fields.Add("type", new EntityField("type", EntityFieldType.Enum, new EntityEnumField() { "a", "b", }));
            context.Methods.Add(responseMethod);
            return context;
        }

        [TestMethod]//, Ignore]
        public void TestMethod1()
        {
            var factory = new TestApiFactory();
            var stream = new MemoryStream();
            factory.Run(new StreamWriter(stream));
            stream.Seek(0L, SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();
        }

        [TestMethod]
        public void WriteServiceWorks()
        {
            var context = GetContext1();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var factory = new TestApiFactory();
            factory.DoWriteService(context, writer);
            stream.Seek(0L, SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();

            Assert.IsTrue(result.Contains("public class result_method"));
            Assert.IsTrue(result.Contains("public enum typeEnum"));
        }

        [TestMethod]
        public void WriteEntitiesWorks()
        {
            var context = GetContext1();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var factory = new TestApiFactory();
            factory.DoWriteEntities(context, writer);
            stream.Seek(0L, SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();

            Assert.IsTrue(result.Contains("public class result_method"));
            Assert.IsTrue(result.Contains("public enum typeEnum"));
        }

        [TestMethod]
        public void ParseReponseFormat_comments_comment()
        {
            var factory = new TestApiFactory();
            const string pre = @"comment
comment:
  - id: string
  - user_id: integer
  - login: string
  - avatar: url
  - date: datetime
  - text: string
  - inner_id: integer
  - in_reply_to: integer";
            var result = factory.DoParseReponseFormat(pre);

            Assert.IsNotNull(result.Entity);
            Assert.AreEqual("pictures", result.Entity.Name);
            Assert.IsNull(result.Entity.ItemName);
            Assert.IsFalse(result.Entity.HasOneChild);
            Assert.IsFalse(result.Entity.HasMultipleChild);
            Assert.IsNull(result.Entity.ItemEntity);
            Assert.AreEqual(8, result.Entity.Fields.Count);
            Assert.AreEqual(EntityFieldType.String, result.Entity.Fields["id"].Type);
            Assert.AreEqual("id", result.Entity.Fields["id"].Name);
        }

        [TestMethod]
        public void ParseReponseFormat_pictures()
        {
            var factory = new TestApiFactory();
            const string pre = @"pictures: picture*
picture:
  - id: integer
  - show_id: integer
  - login_id: integer
  - url: url
  - width: width
  - height: height
  - date: datetime
  - picked: none|banner|show";
            var result = factory.DoParseReponseFormat(pre);

            Assert.IsNotNull(result.Entity);
            Assert.AreEqual("pictures", result.Entity.Name);
            Assert.AreEqual("picture", result.Entity.ItemName);
            Assert.IsFalse(result.Entity.HasOneChild);
            Assert.IsTrue(result.Entity.HasMultipleChild);
            Assert.IsNotNull(result.Entity.ItemEntity);
            Assert.AreEqual(8, result.Entity.Fields.Count);
            Assert.AreEqual(EntityFieldType.Integer, result.Entity.Fields["id"].Type);
            Assert.AreEqual("id", result.Entity.Fields["id"].Name);
        }
    }
}
