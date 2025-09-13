using System.ComponentModel;

namespace Presentation.WinFormsApp.Models
{
    public class ToolModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Tool Name")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Description")]
        public string Description { get; set; } = string.Empty;

        [DisplayName("Category")]
        public string Category { get; set; } = string.Empty;

        [DisplayName("Version")]
        public string Version { get; set; } = string.Empty;

        [DisplayName("Status")]
        public string Status { get; set; } = string.Empty;

        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Last Updated")]
        public DateTime LastUpdated { get; set; }

        public static List<ToolModel> GetFakeData()
        {
            return new List<ToolModel>
            {
                new ToolModel
                {
                    Id = 1,
                    Name = "Code Generator",
                    Description = "Automatically generates boilerplate code for various patterns",
                    Category = "Development",
                    Version = "1.2.3",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddMonths(-6),
                    LastUpdated = DateTime.Now.AddDays(-5)
                },
                new ToolModel
                {
                    Id = 2,
                    Name = "Database Migrator",
                    Description = "Tool for managing database schema migrations",
                    Category = "Database",
                    Version = "2.1.0",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddMonths(-8),
                    LastUpdated = DateTime.Now.AddDays(-10)
                },
                new ToolModel
                {
                    Id = 3,
                    Name = "API Tester",
                    Description = "Comprehensive API testing and validation tool",
                    Category = "Testing",
                    Version = "3.0.1",
                    Status = "Inactive",
                    CreatedDate = DateTime.Now.AddMonths(-4),
                    LastUpdated = DateTime.Now.AddDays(-20)
                },
                new ToolModel
                {
                    Id = 4,
                    Name = "Log Analyzer",
                    Description = "Advanced log file analysis and reporting tool",
                    Category = "Monitoring",
                    Version = "1.5.2",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddMonths(-10),
                    LastUpdated = DateTime.Now.AddDays(-2)
                },
                new ToolModel
                {
                    Id = 5,
                    Name = "Performance Monitor",
                    Description = "Real-time application performance monitoring",
                    Category = "Monitoring",
                    Version = "2.3.1",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddMonths(-7),
                    LastUpdated = DateTime.Now.AddDays(-1)
                },
                new ToolModel
                {
                    Id = 6,
                    Name = "Config Manager",
                    Description = "Centralized configuration management system",
                    Category = "Configuration",
                    Version = "1.8.4",
                    Status = "Maintenance",
                    CreatedDate = DateTime.Now.AddMonths(-12),
                    LastUpdated = DateTime.Now.AddDays(-15)
                },
                new ToolModel
                {
                    Id = 7,
                    Name = "Report Builder",
                    Description = "Dynamic report generation and customization tool",
                    Category = "Reporting",
                    Version = "2.0.0",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddMonths(-3),
                    LastUpdated = DateTime.Now.AddDays(-3)
                },
                new ToolModel
                {
                    Id = 8,
                    Name = "Security Scanner",
                    Description = "Automated security vulnerability scanning tool",
                    Category = "Security",
                    Version = "1.4.7",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddMonths(-9),
                    LastUpdated = DateTime.Now.AddDays(-7)
                },
                new ToolModel
                {
                    Id = 9,
                    Name = "Backup Manager",
                    Description = "Automated backup and recovery management system",
                    Category = "Utility",
                    Version = "3.1.2",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddMonths(-11),
                    LastUpdated = DateTime.Now.AddDays(-4)
                },
                new ToolModel
                {
                    Id = 10,
                    Name = "Documentation Generator",
                    Description = "Automatic documentation generation from code comments",
                    Category = "Development",
                    Version = "1.3.5",
                    Status = "Inactive",
                    CreatedDate = DateTime.Now.AddMonths(-5),
                    LastUpdated = DateTime.Now.AddDays(-30)
                }
            };
        }

        public static List<ToolModel> SearchTools(List<ToolModel> tools, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return tools;

            searchText = searchText.ToLower();
            return tools.Where(t => 
                t.Name.ToLower().Contains(searchText) ||
                t.Description.ToLower().Contains(searchText) ||
                t.Category.ToLower().Contains(searchText) ||
                t.Status.ToLower().Contains(searchText)
            ).ToList();
        }
    }
}