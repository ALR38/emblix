using NUnit.Framework;
using Emblix.Core.Templates;

[TestFixture]
public class TemplatorTests
{
    private ITemplator _templator;

    [SetUp]
    public void Setup()
    {
        _templator = new Templator();
    }

    [Test]
    public void GetHtmlByTemplate_SimpleVariable_ShouldReplace()
    {
        // Arrange
        var template = "Hello {{name}}!";
        var name = "World";

        // Act
        var result = _templator.GetHtmlByTemplate(template, name);

        // Assert
        Assert.That(result, Is.EqualTo("Hello World!"));
    }

    [Test]
    public void GetHtmlByTemplate_ComplexObject_ShouldReplaceAllProperties()
    {
        // Arrange
        var template = "{{Name}} is {{Age}} years old";
        var person = new { Name = "John", Age = 30 };

        // Act
        var result = _templator.GetHtmlByTemplate(template, person);

        // Assert
        Assert.That(result, Is.EqualTo("John is 30 years old"));
    }

    [Test]
    public void GetHtmlByTemplate_WithCondition_ShouldProcessIfTrue()
    {
        // Arrange
        var template = "{{#if IsAdmin}}Admin{{else}}User{{/if}}";
        var user = new { IsAdmin = true };

        // Act
        var result = _templator.GetHtmlByTemplate(template, user);

        // Assert
        Assert.That(result, Is.EqualTo("Admin"));
    }

    [Test]
    public void GetHtmlByTemplate_WithCondition_ShouldProcessIfFalse()
    {
        // Arrange
        var template = "{{#if IsAdmin}}Admin{{else}}User{{/if}}";
        var user = new { IsAdmin = false };

        // Act
        var result = _templator.GetHtmlByTemplate(template, user);

        // Assert
        Assert.That(result, Is.EqualTo("User"));
    }

    [Test]
    public void GetHtmlByTemplate_WithEqCondition_ShouldProcessWhenEqual()
    {
        // Arrange
        var template = "{{#if (eq Role 'Admin')}}Administrator{{else}}Regular User{{/if}}";
        var user = new { Role = "Admin" };

        // Act
        var result = _templator.GetHtmlByTemplate(template, user);

        // Assert
        Assert.That(result, Is.EqualTo("Administrator"));
    }

    [Test]
    public void GetHtmlByTemplate_WithLoop_ShouldProcessCollection()
    {
        // Arrange
        var template = "{{#each Items}}{{Name}},{{/each}}";
        var data = new { Items = new[] { new { Name = "Item1" }, new { Name = "Item2" } } };

        // Act
        var result = _templator.GetHtmlByTemplate(template, data);

        // Assert
        Assert.That(result, Is.EqualTo("Item1,Item2,"));
    }

    [Test]
    public void GetHtmlByTemplate_WithNestedObjects_ShouldProcessNestedProperties()
    {
        // Arrange
        var template = "{{User.Profile.Name}}";
        var data = new { User = new { Profile = new { Name = "John" } } };

        // Act
        var result = _templator.GetHtmlByTemplate(template, data);

        // Assert
        Assert.That(result, Is.EqualTo("John"));
    }

    [Test]
    public void GetHtmlByTemplate_WithDateTime_ShouldFormatCorrectly()
    {
        // Arrange
        var template = "Date: {{Date}}";
        var data = new { Date = new DateTime(2024, 1, 1) };

        // Act
        var result = _templator.GetHtmlByTemplate(template, data);

        // Assert
        Assert.That(result, Does.Contain("2024"));
    }

    [Test]
    public void GetHtmlByTemplate_WithNullValues_ShouldHandleGracefully()
    {
        // Arrange
        var template = "Name: {{Name}}";
        var data = new { Name = (string)null };

        // Act
        var result = _templator.GetHtmlByTemplate(template, data);

        // Assert
        Assert.That(result, Is.EqualTo("Name: "));
    }

    [Test]
    public void GetHtmlByTemplate_EmptyTemplate_ShouldReturnEmpty()
    {
        // Arrange
        var template = "";
        var data = new { Name = "Test" };

        // Act
        var result = _templator.GetHtmlByTemplate(template, data);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetHtmlByTemplate_NullObject_ShouldReturnTemplate()
    {
        // Arrange
        var template = "{{Name}}";

        // Act
        var result = _templator.GetHtmlByTemplate(template, null);

        // Assert
        Assert.That(result, Is.EqualTo(template));
    }
}