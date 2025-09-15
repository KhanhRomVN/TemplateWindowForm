// Simple compilation test
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

// Test that basic syntax works
class TestClass
{
    private IContainer? components = null;
    
    public void TestMethod()
    {
        var button = new Button 
        {
            Text = "Test",
            Font = new Font(FontFamily.GenericSansSerif, 9.5F, FontStyle.Bold),
            Size = new Size(256, 42),
            Location = new Point(0, 0),
            FillColor = Color.Transparent
        };
        
        // Test anonymous types
        var navigationItems = new[]
        {
            new { Text = "Home", Icon = "🏠", Route = "Home", Y = 0 },
            new { Text = "Tasks", Icon = "📋", Route = "Tool", Y = 46 }
        };
        
        foreach (var item in navigationItems)
        {
            Console.WriteLine($"{item.Text}: {item.Route}");
        }
    }
}