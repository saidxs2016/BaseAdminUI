// ****************************** Scaffold from packagemanager console ******************************
	// ================= Scaffold =================
	Scaffold-DbContext "Host=localhost;Database=main_sys_db;Username=postgres;Password=123" Npgsql.EntityFrameworkCore.PostgreSQL -Context SysDbContext -ContextDir ./DB2/Context -OutputDir ./DB2/Entities -Schemas public -DataAnnotations -Verbose -force

	// Note: can you select specific schemas: with -Schemas public,other,........	
	// Note: can you select specific tables: with -Tables admin,module,........			 

// ****************************** scaffold from Terminal ******************************
	// ================= Scaffold  =================  
	dotnet ef dbcontext scaffold "Host=localhost;Database=main_sys_db;Username=postgres;Password=123" Npgsql.EntityFrameworkCore.PostgreSQL --context SysDbContext --context-dir ./DB2/Context --output-dir ./DB2/Entities --schema public --data-annotations --verbose --force
	 

	// Note: can you select specific schemas: with --schema public --schema other ........	
	// Note: can you select specific tables: with --table admin --table module ........	

	 





