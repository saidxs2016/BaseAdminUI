// ****************************** Scaffold from packagemanager console ******************************
	// ================= Scaffold =================
	Scaffold-DbContext "Host=localhost;Database=main_db;Username=postgres;Password=123" Npgsql.EntityFrameworkCore.PostgreSQL -Context MDbContext -ContextDir ./DB1/Context -OutputDir ./DB1/Entities -DataAnnotations -Verbose -force

	// Note: can you select specific schemas: with -Schemas public,other,........	
	// Note: can you select specific tables: with -Tables admin,module,........			 

// ****************************** scaffold from Terminal ******************************
	// ================= Scaffold  =================  
	dotnet ef dbcontext scaffold "Host=localhost;Database=main_db;Username=postgres;Password=123" Npgsql.EntityFrameworkCore.PostgreSQL --context MDbContext --context-dir ./DB1/Context --output-dir ./DB1/Entities --data-annotations --verbose --force
	 

	// Note: can you select specific schemas: with --schema public --schema other ........	
	// Note: can you select specific tables: with --table admin --table module ........	




