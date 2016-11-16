using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortuga.Chain;
using Tortuga.Chain.SqlServer;

namespace Tests
{
    [TestClass]
    public class ExecutionPlanTests
    {


        [TestMethod]
        public void TestMethod1_ExecutionPlans()
        {
            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");

            var plan1 = dataSource.Sql("DELETE FROM Vehicle").AsNonQuery().GetExecutionPlan();
            var plan2 = dataSource.Sql("DELETE FROM Vehicle").AsNonQuery().GetExecutionPlan();

            dataSource.Sql("DELETE FROM Vehicle").Execute();
            dataSource.Sql("DELETE FROM Owner").Execute();

            var plan3 = dataSource.Insert("Owner", new { DriversLicense = "B8572496", FirstName = "Elvis", LastName = "Presley" }).ToInt32().GetExecutionPlan();
            var plan4 = dataSource.Insert("Vehicle", new { VehicleID = "65476XC54E", Make = "Cadillac", Model = "Fleetwood Series 60", Year = 1955 }).ToInt32().GetExecutionPlan();

            var ownerKey = dataSource.Insert("Owner", new { DriversLicense = "B8572496", FirstName = "Elvis", LastName = "Presley" }).ToInt32().Execute();
            var vehicleKey = dataSource.Insert("Vehicle", new { VehicleID = "65476XC54E", Make = "Cadillac", Model = "Fleetwood Series 60", Year = 1955 }).ToInt32().Execute();

            var plan5 = dataSource.Update("Vehicle", new { VehicleKey = vehicleKey, OwnerKey = ownerKey }).AsNonQuery().GetExecutionPlan();
            dataSource.Update("Vehicle", new { VehicleKey = vehicleKey, OwnerKey = ownerKey }).Execute();

            var plan6 = dataSource.From("Vehicle", new { OwnerKey = ownerKey }).ToStringList("VehicleID").Execute();
            var vin = dataSource.From("Vehicle", new { OwnerKey = ownerKey }).ToStringList("VehicleID").Execute();

            var plan7 = dataSource.GetByKey("Owner", ownerKey).ToObject<Owner>().GetExecutionPlan();
            var owner = dataSource.GetByKey("Owner", ownerKey).ToObject<Owner>().Execute();

            var plan8 = dataSource.From("Vehicle", owner).ToCollection<Vehicle>().GetExecutionPlan();
            var cars = dataSource.From("Vehicle", owner).ToCollection<Vehicle>().Execute();

            //make it clear what we want to filter on
            cars = dataSource.From("Vehicle", new { OwnerKey = owner.OwnerKey }).ToCollection<Vehicle>().Execute();

            //or make the WHERE clause explicit
            cars = dataSource.From("Vehicle", "OwnerKey = @OwnerKey", owner).ToCollection<Vehicle>().Execute();


            dataSource.Delete("Vehicle", new { VehicleKey = vehicleKey }).Execute();
            dataSource.Delete("Owner", new { OwnerKey = ownerKey }).Execute();

        }


        public class Owner
        {
            public int OwnerKey { get; set; }
            public string DriversLicense { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public short Zip { get; set; }

        }

        public class Vehicle
        {
            public int VehicleKey { get; set; }
            public string VehicleID { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public short Year { get; set; }
            public int OwnerKey { get; set; }
        }

    }
}
