namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;
using static shared.Util;

[TestClass]
public class ServiceTest
{
    private readonly DataService service;

    public ServiceTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database"); 
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretPN()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(4, service.GetPNs().Count());

        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 5, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(5, service.GetPNs().Count());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
        Assert.AreNotEqual(3, service.GetDagligFaste().Count());

    }
    [TestMethod]
    public void OpretDagligSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis[] doser = new Dosis[]
        {
            new Dosis (CreateTimeOnly(12, 0, 0), 1),
            new Dosis (CreateTimeOnly(16, 30, 0), 4),
            new Dosis (CreateTimeOnly(22, 45, 0), 2)
        };
        Assert.AreEqual(1, service.GetDagligSkæve().Count());
        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligSkæve().Count());


    } 
}