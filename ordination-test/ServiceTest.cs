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
    public void OpretPN()
    {
       
        Laegemiddel lm = service.GetLaegemidler().First();
      
       
        PN test = new PN {startDen = DateTime.Now, slutDen = DateTime.Now.AddDays(5), laegemiddel = lm, antalEnheder = 5};
        test.dates.Add(new Dato{dato = DateTime.Now.AddHours(5)});
        test.dates.Add(new Dato{dato = DateTime.Now.AddHours(6)});
        //tester antalgangegivet 
        Assert.AreEqual(test.dates.Count(),test.getAntalGangeGivet(),"test af antalgangegivet(Bør retunere 'true')");
        //tester samletDosis
        Assert.AreEqual((test.antalEnheder * test.dates.Count()),test.samletDosis(),"test af samletDosis(bør returnere 'true)");
        //tester abstrakt metode antalDage (PN nedarver metoden fra Ordination) 
        //cba af rode rundt med datetime
        Assert.AreEqual((double)(DateTime.Now.AddDays(5) - DateTime.Now).TotalDays,test.antalDage(),"DateTime?");
        //tester døgndosis
        Assert.AreEqual(((test.dates.Count()*test.antalEnheder)/test.antalDage()),test.doegnDosis(),"test af døgndosis(bør returnere 'true' hvis forgående test returnerede'true',ellers returnerer den 'false'");
        //tester gettype 
        Assert.AreEqual("PN",test.getType(),"test af gettype, bør returnere 'true'");

    }

    [TestMethod]
    public void OpretDagligFast()
    {
        
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis MD = new Dosis {antal = 1};
        Dosis MiD=new Dosis {antal = 2}; 
        Dosis AD =new Dosis {antal = 3};
        Dosis ND=new Dosis {antal = 4};
        Dosis [] dosisTest = {MD,MiD,AD,ND};
        DagligFast test = new DagligFast{startDen = DateTime.Now, slutDen = DateTime.Now.AddDays(1),laegemiddel = lm,MorgenDosis = MD, MiddagDosis = MiD,AftenDosis = AD, NatDosis = ND};
        //tester døgndosis
        Assert.AreEqual(10,test.doegnDosis(),"test af døgndosis (bør returnere 'true')");
        //tester samletdosis
        Assert.AreEqual((test.antalDage() * test.doegnDosis()),test.samletDosis(),"test af samletdosis(bør returnere 'true')");
        //test getdoser NOTE: Ved ikke hvordan arrays identificeres, så der er en chance for at den returnerer 'false' når den bør returnere 'true'
        CollectionAssert.AreEqual(dosisTest, test.getDoser(),"tester getdoser(bør returnere 'true')");
        

    }
    [TestMethod]
    public void OpretDagligSkaev()
    {
         Laegemiddel lm = service.GetLaegemidler().First();
         List <Dosis> dosisTest = new List<Dosis> {new Dosis(DateTime.Now.AddHours(1),2)};
         DagligSkæv test = new DagligSkæv {startDen = DateTime.Now, slutDen = DateTime.Now.AddDays(1), laegemiddel = lm, doser = dosisTest};
         //tester døgndosis
         Assert.AreEqual(2,test.doegnDosis(),"test af DagligSkaev.doegnDosis()");
         //tester samletdosis
         Assert.AreEqual((test.antalDage() * test.doegnDosis()),test.samletDosis(),"test af DagligSkaev.samletDosis()");
         //tester opretdosis 
         dosisTest.Add(new Dosis(DateTime.Now.AddHours(2),2));
         test.opretDosis(DateTime.Now.AddHours(2),2);
         CollectionAssert.AreEqual(dosisTest,test.doser);


    } 
}