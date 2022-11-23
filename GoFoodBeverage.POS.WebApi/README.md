
Domain Layer - Domain Driven Design
This is the center of the system. The Domain handles most of the business logic of the system. This layer is also responsible for defining the concepts, behaviors, and rules.
See more detail: https://enlabsoftware.com/development/domain-driven-design-in-asp-net-core-applications.html#:~:text=return%20userDTOs%3B%20%7D%20%7D-,Domain%20Layer,concepts%2C%20behaviors%2C%20and%20rules.

* Structure descriptions
1. WebApi Layer: 
	+ Api controllers
	+ Mediator pattern
2. Core
	2.1. Application
		+ Features
		+ Interfaces
		+ Mappings
	2.2. Domain
		+ Entities model
		+ Enums
		+ Settings class: jwt settings, mail settings
	2.3. Infrastructure
		+ Db context
		+ Migrations
		+ Repositories
	2.4. Models
	2.5. Shared
		+ Common services
		+ Common providers

GUIDELINE
* Add new table with migration
	1. Declare entity class in Domain layer > Entities
	2. Declare DbSet in Infrastructure > Contexts > DbContext.cs
	3. In DbContext.cs > OnModelCreating method we need to declare the name of the table to follow the rules.
	* Rule: The name of table not include 's' in the end of name.
	4. Update databse with migration

* Declare Repository:
	1. Declare interface repository in Application layer > Interfaces > Repositories
	2. Declare repository in Infrastructure layer > Repositories
	3. Register the service with a scoped lifetime in Startup.cs
	4. Declare this repository in UnitOfWork and IUnitOfWork

* Implement a feature: 
	Application layer > Features
	1. Create folder with the name is name of feature
	2. Create Commands folder: include the handle request to handle something may change data
	3. Create Queries folder: include the handle request to query data
	4. Call handle request in the API controller with the mediator
		
Sendgrid
tu.van@mediastep.com/Loginsendgrid123@
