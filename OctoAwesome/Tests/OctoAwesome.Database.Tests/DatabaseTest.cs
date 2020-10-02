﻿using NUnit.Framework;
using System.IO;
using System.Text;
namespace OctoAwesome.Database.Tests
{
    [TestOf(typeof(Database<>))]
    public class DatabaseTest
    {
        [TestCase("test.key", "test.value", TestName = "Default integration test")]
        public void DefaultIntegrationTest(string keyPath, string valuePath)
        {
            var temp = Path.GetTempPath();

            var keyFile = new FileInfo(Path.Combine(temp, keyPath));
            var valueFile = new FileInfo(Path.Combine(temp, valuePath));

            var database = new Database<TestTag>(keyFile, valueFile);


            try
            {
                database.Open();
                database.AddOrUpdate(new TestTag(42), new Value(Encoding.UTF8.GetBytes("Hello World 0")));
                database.AddOrUpdate(new TestTag(45), new Value(Encoding.UTF8.GetBytes("Hello World 1")));
                database.AddOrUpdate(new TestTag(47), new Value(Encoding.UTF8.GetBytes("Hello World 2")));
                var value0 = database.GetValue(new TestTag(42));
                var value2 = database.GetValue(new TestTag(45));
                var value3 = database.GetValue(new TestTag(47));
            }
            finally
            {
                database.Dispose();
                keyFile.Delete();
                valueFile.Delete(); //CONTINUE: https://youtu.be/l0X12-2TE9w?t=5548
            }
        }

        private class TestTag : ITagable
        {
            public int Tag { get; set; }

            public TestTag(int tag)
            {
                Tag = tag;
            }
        }
    }
}