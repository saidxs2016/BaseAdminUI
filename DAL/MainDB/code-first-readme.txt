// =================== Migration from Package Manager Console ===================
	-	add-migration 'your-migration' -c 'your-db-context' -o 'your-output-directory' -args="--provider Npgsql.EntityFrameworkCore.PostgreSQL"
	-	update-database -context 'your-db-context' -m 'your-migration'	

	-   Not: (-C | -c) == -Context, (-O | -o) = -OutputDir, (-M | -m) == -Migration
// =================== Migration from Terminal ===================
	-	dotnet ef migrations add 'your-migration' --context 'your-db-context' --output-dir 'your-output-directory' -- --provider Npgsql.EntityFrameworkCore.PostgreSQL
	-	dotnet ef database update --context 'your-db-context' 'your-migration'





// =================== set applied as false for all migrations from Package Manager Console ===================
	-	update-database -context 'your-db-context' 0
// =================== set applied as false for all migrations from Terminal ===================
	-	dotnet ef database update --context 'your-db-context' 0




// =================== remove last migration from Package Manager Console ===================
	-	Remove-Migration --context 'your-db-context'
// =================== remove last migration from Terminal ===================
	-	dotnet ef migrations remove --context 'your-db-context'




// =================== drop database from Package Manager Console ===================	
	-	drop-database -context 'your-db-context'
// =================== drop database from Terminal ===================
	-	dotnet ef database drop --context 'your-db-context'






// =================== Örenk: Migration from Package Manager Console ===================
	-	add-migration Mig1 -c MDbContext -o ./DB1/Migrations -args "--provider Npgsql.EntityFrameworkCore.PostgreSQL"
	-	update-database -context MDbContext -m Mig1

	-Not: (-C | -c) == -Context, (-O | -o) = -OutputDir, (-M | -m) == -Migration

// =================== Örnek: Migration from Terminal ===================s
	-	dotnet ef migrations add Mig1 --context MDbContext --output-dir ./DB1/Migrations -- --provider Npgsql.EntityFrameworkCore.PostgreSQL
	-	dotnet ef database update --context MDbContext Mig1


