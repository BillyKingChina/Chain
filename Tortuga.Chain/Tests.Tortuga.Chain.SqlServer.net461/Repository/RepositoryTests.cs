﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Repository
{
    [TestClass]
    public class RepositoryTests : TestBase
    {
        [TestMethod]
        public void BasicCrud()
        {
            var source = Class1DataSource;
            var repo = new Repository<Employee, int>(source, "HR.Employee");

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var echo1 = repo.Insert(emp1);

            Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo1.FirstName, "FirstName");
            Assert.AreEqual(emp1.LastName, echo1.LastName, "LastName");
            Assert.AreEqual(emp1.Title, echo1.Title, "Title");


            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = echo1.EmployeeKey };
            var echo2 = repo.Insert(emp2);
            Assert.AreNotEqual(0, echo2.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo2.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo2.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo2.Title, "Title");
            Assert.AreEqual(emp2.ManagerKey, echo2.ManagerKey, "ManagerKey");

            var list = repo.GetAll();
            Assert.IsTrue(list.Any(e => e.EmployeeKey == echo1.EmployeeKey), "Employee 1 is missing");
            Assert.IsTrue(list.Any(e => e.EmployeeKey == echo2.EmployeeKey), "Employee 2 is missing");

            var get1 = repo.Get(echo1.EmployeeKey);
            Assert.AreEqual(echo1.EmployeeKey, get1.EmployeeKey);

            repo.Delete(echo2.EmployeeKey);
            repo.Delete(echo1.EmployeeKey);

            var list2 = repo.GetAll();
            Assert.AreEqual(list.Count - 2, list2.Count);

        }
    }
}