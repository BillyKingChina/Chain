﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Tests.Models;
using Tortuga.Chain;

namespace Tests.Materializers
{
    [TestClass]
    public class ModelTests : TestBase
    {
        [TestMethod]
        public void DecomposeTest()
        {
            var ds = DataSource;

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var emp1Key = ds.Insert(EmployeeTableName, emp1).ToInt32().Execute();

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = emp1Key };
            var emp2Key = ds.Insert(EmployeeTableName, emp2).ToInt32().Execute();

            var echo = ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).ToObject<EmployeeWithManager>().Execute();

            Assert.AreNotEqual(0, echo.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo.Title, "Title");

            Assert.AreNotEqual(0, echo.Manager.EmployeeKey, "Manager.EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo.Manager.FirstName, "Manager.FirstName");
            Assert.AreEqual(emp1.LastName, echo.Manager.LastName, "Manager.LastName");
            Assert.AreEqual(emp1.Title, echo.Manager.Title, "Manager.Title");

            Assert.IsNotNull(echo.AuditInfo.CreatedDate, "CreatedDate via AuditInfo");

        }

        [TestMethod]
        public void CompiledDecomposeTest()
        {
            var ds = DataSource;

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var emp1Key = ds.Insert(EmployeeTableName, emp1).ToInt32().Execute();

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = emp1Key };
            var emp2Key = ds.Insert(EmployeeTableName, emp2).ToInt32().Execute();

            var echo = ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).Compile().ToObject<EmployeeWithManager>().Execute();

            Assert.AreNotEqual(0, echo.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo.Title, "Title");

            Assert.AreNotEqual(0, echo.Manager.EmployeeKey, "Manager.EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo.Manager.FirstName, "Manager.FirstName");
            Assert.AreEqual(emp1.LastName, echo.Manager.LastName, "Manager.LastName");
            Assert.AreEqual(emp1.Title, echo.Manager.Title, "Manager.Title");

            Assert.IsNotNull(echo.AuditInfo.CreatedDate, "CreatedDate via AuditInfo");
            Assert.IsNotNull(echo.Manager.AuditInfo.CreatedDate, "CreatedDate via Manager.AuditInfo");

        }

        [TestMethod]
        public void CompiledTest()
        {
            var ds = DataSource;

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var emp1Key = ds.Insert(EmployeeTableName, emp1).ToInt32().Execute();

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = emp1Key };
            var emp2Key = ds.Insert(EmployeeTableName, emp2).ToInt32().Execute();

            var echo = ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).Compile().ToObject<Employee>().Execute();
            var list = ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).Compile().ToCollection<Employee>().Execute();


            Assert.AreNotEqual(0, echo.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo.Title, "Title");
        }

        [TestMethod]
        public void TestWithBinary()
        {
            var ds = DataSource;

            var list = ds.From("dbo.AllTypes").ToCollection<BinaryModel>().Execute();
            Assert.IsTrue(list.All(x => x.BinaryNotNull != null));
        }

        [TestMethod]
        public void CompiledTestWithBinary()
        {
            var ds = DataSource;

            var list = ds.From("dbo.AllTypes").Compile().ToCollection<BinaryModel>().Execute();
            Assert.IsTrue(list.All(x => x.BinaryNotNull != null));
        }

        [TestMethod]
        public void TestWithBinary_Single()
        {
            var ds = DataSource;

            var result = ds.From("dbo.AllTypes").ToObject<BinaryModel>(RowOptions.DiscardExtraRows).Execute();
            Assert.IsTrue(result.BinaryNotNull != null);
        }

        [TestMethod]
        public void CompiledTestWithBinary_Single()
        {
            var ds = DataSource;

            var result = ds.From("dbo.AllTypes").Compile().ToObject<BinaryModel>(RowOptions.DiscardExtraRows).Execute();
            Assert.IsTrue(result.BinaryNotNull != null);
        }

        [TestMethod]
        public void Enum_Byte()
        {
            var ds = DataSource;

            var result = ds.From("dbo.AllTypes").ToCollection<EnumByteModel>().Execute();
        }


        [TestMethod]
        public void Enum_Byte_Compiled()
        {
            var ds = DataSource;

            var result = ds.From("dbo.AllTypes").Compile().ToCollection<EnumByteModel>().Execute();

        }

        [TestMethod]
        public void Enum_Byte_Null()
        {
            var ds = DataSource;

            var result = ds.From("dbo.AllTypes").ToCollection<EnumByteModel>().Execute();
        }


        [TestMethod]
        public void Enum_Byte_Null_Compiled()
        {
            var ds = DataSource;

            var result = ds.From("dbo.AllTypes").Compile().ToCollection<EnumByteNullModel>().Execute();

        }

        [TestMethod]
        public async Task DecomposeTest_Async()
        {
            var ds = DataSource;

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var emp1Key = await ds.Insert(EmployeeTableName, emp1).ToInt32().ExecuteAsync();

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = emp1Key };
            var emp2Key = await ds.Insert(EmployeeTableName, emp2).ToInt32().ExecuteAsync();

            var echo = await ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).ToObject<EmployeeWithManager>().ExecuteAsync();

            Assert.AreNotEqual(0, echo.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo.Title, "Title");

            Assert.AreNotEqual(0, echo.Manager.EmployeeKey, "Manager.EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo.Manager.FirstName, "Manager.FirstName");
            Assert.AreEqual(emp1.LastName, echo.Manager.LastName, "Manager.LastName");
            Assert.AreEqual(emp1.Title, echo.Manager.Title, "Manager.Title");

            Assert.IsNotNull(echo.AuditInfo.CreatedDate, "CreatedDate via AuditInfo");

        }

        [TestMethod]
        public async Task CompiledDecomposeTest_Async()
        {
            var ds = DataSource;

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var emp1Key = await ds.Insert(EmployeeTableName, emp1).ToInt32().ExecuteAsync();

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = emp1Key };
            var emp2Key = await ds.Insert(EmployeeTableName, emp2).ToInt32().ExecuteAsync();

            var echo = await ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).Compile().ToObject<EmployeeWithManager>().ExecuteAsync();

            Assert.AreNotEqual(0, echo.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo.Title, "Title");

            Assert.AreNotEqual(0, echo.Manager.EmployeeKey, "Manager.EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo.Manager.FirstName, "Manager.FirstName");
            Assert.AreEqual(emp1.LastName, echo.Manager.LastName, "Manager.LastName");
            Assert.AreEqual(emp1.Title, echo.Manager.Title, "Manager.Title");

            Assert.IsNotNull(echo.AuditInfo.CreatedDate, "CreatedDate via AuditInfo");
            Assert.IsNotNull(echo.Manager.AuditInfo.CreatedDate, "CreatedDate via Manager.AuditInfo");

        }

        [TestMethod]
        public async Task CompiledTest_Async()
        {
            var ds = DataSource;

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var emp1Key = await ds.Insert(EmployeeTableName, emp1).ToInt32().ExecuteAsync();

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = emp1Key };
            var emp2Key = await ds.Insert(EmployeeTableName, emp2).ToInt32().ExecuteAsync();

            var echo = await ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).Compile().ToObject<Employee>().ExecuteAsync();
            var list = await ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).Compile().ToCollection<Employee>().ExecuteAsync();


            Assert.AreNotEqual(0, echo.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo.Title, "Title");
        }
    }

    public class BinaryModel
    {
        public int Id { get; set; }
        public string RowName { get; set; }
        public byte[] BinaryNotNull { get; set; }

    }

    public class EnumByteModel
    {
        public Gender TinyIntNotNull { get; set; }
    }

    public class EnumByteNullModel
    {
        public Gender? TinyIntNull { get; set; }
    }

    public enum Gender
    {
        Male = 1,
        Female = 2
    }


}
