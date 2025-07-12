namespace DataAccess
{
    public class Class1
    {



        /* appsetting.json

		"ConnectionStrings": {
		"DefaultConnectionString": "Server=(local);Uid=sa;Pwd=12345;Database=Sp25PharmaceuticalDB; TrustServerCertificate=True"
		}
		*/

        /* scaffold database: remember database must be exist and window security is turned off

		Scaffold-DbContext "Server=(local);Database=Sp25PharmaceuticalDB;Uid=sa;Pwd=12345;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir ./
		*/

        /* Connect to database

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseSqlServer(GetConnectionString());


		private string GetConnectionString()
		{
			IConfiguration configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", true, true).Build();
			return configuration["ConnectionStrings:DefaultConnectionString"];
		}
		*/





        /* Guild

		Step 0:
		- Turn off Window security
		Step 1: Set up

		- Name solution: 
		- Name Project:
		- Create: BusinessLogicLayer, DataAccessLayer, BusinessObject
		- Create Folder: 
			+ DataAccessLayer:DAOs
			+ BusinessLogicLayer:Repositories, IRepositories
		- Add project preference: 
			+ Presentation: BusinessLogicLayer, BusinessObject
			+ BusinessLogicLayer: DataAccessLayer, BusinessObject
			+ DataAccessLayer: BusinessObject
		- Set up DataAccessLayer as startup project, then download nuget
			+ SqlServer, tools, design, configuration, configuration.json
		- Add ConnectionStrings in appsettings.json, Run Scaffold and add connection to database. (use snippet: step1setup).
		- Move all entities to BusinessObject and change namespace from DataAccessLayer to BusinessObject. (All the enitities in BusinessObject) and DBContext.
		- Setup Presentation layer as start up project

		Step 2: Create DAO
		-(1) Create LoginDAO (use snippet: step2loginDAO)
		-(2) Create MainEntityDAO (use snippet: step2EntityDAO)

		Step 3: Create Repo
		-(1) Create LoginRepo (use snippet: step3loginRepo)
		-(2) Create EntityRepo (use snippet: step3EntityRepo)
		-(2) Create SubEntityRepo (use snippet: step3SubEntityRepo)

		Step 4: Presentation

		4.1 MVC 
		- Add Dependence Injection, Session in Program.cs (use snippet: step4programSetup)
		-(1) Login: Fix the code in Views/Home/Index. In HomeController add (use snippet: step4MVCloginPageLogic)
		-(1) Error: Fix the code in Views/Shares/Error. (use snippet: step4errorSetup)
		-(2) Click Controller Folder. Choose CRUD with Entity Framework. Choose DbContext and Entity. After generating, change Model 
		if @model DataAccessLayer.MedicineInformation. 
		Change from @model DataAccessLayer.MedicineInformation to @model BusinessObject.MedicineInformation
		-(2) Add logic into main entity (use snippet: step4MVCEntityPageLogic)
		-(3) Authorize and paging in Entity index page (use snippet: step4MVCAuthorizePaging)
		-(4) Add Search function (use snippet: step4MVCSearch)
		-(5) Remember to change the server in appsetting.json

		4.2 Razor Page
		- Add Dependence Injection, Session in Program.cs (use snippet: step4programSetup)
		-(1) Login: Fix the code in Pages/Index. (use snippet: step4RazorPageLogin)
		-(1) Error: Fix the code in Views/Shares/Error. (use snippet: step4errorSetup)
		-(2) Click Pages folder and create a page named after Main Entity. Choose CRUD with Entity Framework. Choose DbContext and Entity. After generating, change Model 
		if @model DataAccessLayer.MedicineInformation. 
		Change from @model DataAccessLayer.MedicineInformation to @model BusinessObject.MedicineInformation
		-(2) Add logic into main entity index (use snippet: step4RazorPageEntityIndex)
		-(2) Add logic into main entity edit (use snippet: step4RazorPageEntityEdit)
		-(2) Add logic into main entity detail (use snippet: step4RazorPageEntityDetail)
		-(2) Add logic into main entity delete (use snippet: step4RazorPageEntityDelete)
		-(2) Add logic into main entity create (use snippet: step4RazorPageEntityCreate)
		-(3) Authorize and paging in Entity index page (use snippet: step4RazorPageAuthorizePaging)
		-(4) Add Search function (use snippet: step4MVCSearch)
		-(5) Remember to change the server in appsetting.json

		*/

        /*

		Other snippet
		- generateId

		*/

        
       


	}
}
