TemplateWindowForm/
│
├── src/
│ ├── Core/
│ │ ├── Entities/
│ │ │ ├── Product.cs
│ │ │ ├── User.cs
│ │ │ └── ...
│ │ ├── Enums/
│ │ │ ├── UserRole.cs
│ │ │ └── ...
│ │ ├── Events/
│ │ │ ├── ProductCreatedEvent.cs
│ │ │ └── ...
│ │ ├── Exceptions/
│ │ │ ├── DomainException.cs
│ │ │ └── ...
│ │ ├── Interfaces/
│ │ │ ├── Repositories/
│ │ │ │ ├── IProductRepository.cs
│ │ │ │ └── ...
│ │ │ └── Services/
│ │ │ ├── IEmailService.cs
│ │ │ └── ...
│ │ ├── ValueObjects/
│ │ │ ├── Address.cs
│ │ │ └── ...
│ │ └── Core.csproj
│ │
│ ├── Application/
│ │ ├── Common/
│ │ │ ├── Interfaces/
│ │ │ │ ├── ICurrentUserService.cs
│ │ │ │ └── ...
│ │ │ ├── Models/
│ │ │ │ ├── Result.cs
│ │ │ │ └── ...
│ │ │ └── Behaviors/
│ │ │ ├── LoggingBehavior.cs
│ │ │ └── ...
│ │ ├── DTOs/
│ │ │ ├── Product/
│ │ │ │ ├── ProductDto.cs
│ │ │ │ └── CreateProductDto.cs
│ │ │ └── ...
│ │ ├── UseCases/
│ │ │ ├── Product/
│ │ │ │ ├── Commands/
│ │ │ │ │ ├── CreateProduct/
│ │ │ │ │ │ ├── CreateProductCommand.cs
│ │ │ │ │ │ ├── CreateProductCommandHandler.cs
│ │ │ │ │ │ └── CreateProductCommandValidator.cs
│ │ │ │ │ └── ...
│ │ │ │ └── Queries/
│ │ │ │ ├── GetProducts/
│ │ │ │ │ ├── GetProductsQuery.cs
│ │ │ │ │ ├── GetProductsQueryHandler.cs
│ │ │ │ │ └── GetProductsQueryValidator.cs
│ │ │ │ └── ...
│ │ │ └── ...
│ │ ├── Mappings/
│ │ │ ├── MappingProfile.cs
│ │ │ └── ...
│ │ └── Application.csproj
│ │
│ ├── Infrastructure/
│ │ ├── Data/
│ │ │ ├── Configurations/
│ │ │ │ ├── ProductConfiguration.cs
│ │ │ │ └── ...
│ │ │ ├── Migrations/
│ │ │ │ ├── 20230101000000_Initial.cs
│ │ │ │ └── ...
│ │ │ ├── ApplicationDbContext.cs
│ │ │ └── ApplicationDbContextSeed.cs
│ │ ├── Services/
│ │ │ ├── DateTimeService.cs
│ │ │ ├── EmailService.cs
│ │ │ └── ...
│ │ ├── Repositories/
│ │ │ ├── ProductRepository.cs
│ │ │ └── ...
│ │ └── Infrastructure.csproj
│ │
│ └── Presentation/
│ ├── WinFormsApp/
│ │ ├── Forms/
│ │ │ ├── MainForm.cs
│ │ │ ├── MainForm.Designer.cs
│ │ │ ├── ProductForm.cs
│ │ │ ├── ProductForm.Designer.cs
│ │ │ └── ...
│ │ ├── UserControls/
│ │ │ ├── ProductListControl.cs
│ │ │ ├── ProductListControl.Designer.cs
│ │ │ └── ...
│ │ ├── Services/
│ │ │ ├── FormFactory.cs
│ │ │ └── ...
│ │ ├── Program.cs
│ │ └── WinFormsApp.csproj
│ └── Web/ (Optional - for APIs if needed)
│
├── tests/
│ ├── Application.UnitTests/
│ │ ├── UseCases/
│ │ │ ├── Product/
│ │ │ │ ├── Commands/
│ │ │ │ │ ├── CreateProductCommandTests.cs
│ │ │ │ │ └── ...
│ │ │ │ └── Queries/
│ │ │ │ ├── GetProductsQueryTests.cs
│ │ │ │ └── ...
│ │ │ └── ...
│ │ └── Application.UnitTests.csproj
│ │
│ ├── Infrastructure.IntegrationTests/
│ │ ├── Repositories/
│ │ │ ├── ProductRepositoryTests.cs
│ │ │ └── ...
│ │ └── Infrastructure.IntegrationTests.csproj
│ │
│ └── Presentation.UnitTests/
│ ├── Forms/
│ │ ├── MainFormTests.cs
│ │ └── ...
│ └── Presentation.UnitTests.csproj
│
├── .gitignore
├── README.md
├── LICENSE
├── MyWinFormsApp.sln
└── Dockerfile (Optional)
