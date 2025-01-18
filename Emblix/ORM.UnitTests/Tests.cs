using System.Data;
using Microsoft.Data.SqlClient;
using Moq;
using ORM;

[TestFixture]
public class ORMContextTests
{
    private Mock<IDbConnection> _mockConnection;
    private Mock<IDbCommand> _mockCommand;
    private Mock<IDataReader> _mockReader;
    private Mock<IDataParameterCollection> _mockParameters;
    private ORMContext<TestEntity> _context;

    [SetUp]
    public void Setup()
    {
        // Настройка моков
        _mockConnection = new Mock<IDbConnection>();
        _mockCommand = new Mock<IDbCommand>();
        _mockReader = new Mock<IDataReader>();
        _mockParameters = new Mock<IDataParameterCollection>();

        // Базовая настройка команды
        _mockCommand.Setup(c => c.Parameters).Returns(_mockParameters.Object);
        _mockCommand.Setup(c => c.CreateParameter()).Returns(() => new SqlParameter());
        
        // Настройка подключения
        _mockConnection.Setup(c => c.CreateCommand()).Returns(_mockCommand.Object);
        _mockConnection.Setup(c => c.State).Returns(ConnectionState.Closed);
        
        _context = new ORMContext<TestEntity>(_mockConnection.Object);
    }

    [Test]
    public void Create_ValidEntity_ShouldInsertAndSetId()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", Value = 123 };
        _mockCommand.Setup(c => c.ExecuteScalar()).Returns(1);

        // Act
        _context.Create(entity);

        // Assert
        Assert.That(entity.Id, Is.EqualTo(1));
    }

    [Test]
    public void ReadById_ExistingId_ShouldReturnEntity()
    {
        // Arrange
        _mockReader.Setup(r => r.Read()).Returns(true);
        _mockReader.Setup(r => r.GetOrdinal("Id")).Returns(0);
        _mockReader.Setup(r => r.GetOrdinal("Name")).Returns(1);
        _mockReader.Setup(r => r.GetOrdinal("Value")).Returns(2);
        _mockReader.Setup(r => r[0]).Returns(1);
        _mockReader.Setup(r => r[1]).Returns("Test");
        _mockReader.Setup(r => r[2]).Returns(123);
        _mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);
        
        _mockCommand.Setup(c => c.ExecuteReader()).Returns(_mockReader.Object);

        // Act
        var result = _context.ReadById(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Test"));
    }

    [Test]
    public void ReadByAll_WithoutFilter_ShouldReturnAllEntities()
    {
        // Arrange
        _mockReader.SetupSequence(r => r.Read())
            .Returns(true)
            .Returns(true)
            .Returns(false);

        _mockReader.Setup(r => r.GetOrdinal("Id")).Returns(0);
        _mockReader.Setup(r => r.GetOrdinal("Name")).Returns(1);
        _mockReader.Setup(r => r.GetOrdinal("Value")).Returns(2);
    
        // Настройка значений для первой записи
        _mockReader.SetupSequence(r => r[0])
            .Returns(1)
            .Returns(2);
        _mockReader.SetupSequence(r => r[1])
            .Returns("Test1")
            .Returns("Test2");
        _mockReader.SetupSequence(r => r[2])
            .Returns(100)
            .Returns(200);

        _mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);

        _mockCommand.Setup(c => c.ExecuteReader()).Returns(_mockReader.Object);

        // Act
        var results = _context.ReadByAll().ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results[0].Id, Is.EqualTo(1));
        Assert.That(results[0].Name, Is.EqualTo("Test1"));
        Assert.That(results[0].Value, Is.EqualTo(100));
        Assert.That(results[1].Id, Is.EqualTo(2));
        Assert.That(results[1].Name, Is.EqualTo("Test2"));
        Assert.That(results[1].Value, Is.EqualTo(200));
    }

    [Test]
    public void Delete_ExistingId_ShouldExecuteDeleteCommand()
    {
        // Arrange
        _mockCommand.Setup(c => c.ExecuteNonQuery()).Returns(1);

        // Act
        _context.Delete(1);

        // Assert
        _mockCommand.Verify(c => c.ExecuteNonQuery(), Times.Once);
    }

    [Test]
    public void Update_ValidEntity_ShouldExecuteUpdateCommand()
    {
        // Arrange
        var entity = new TestEntity { Name = "Updated", Value = 456 };
        _mockCommand.Setup(c => c.ExecuteNonQuery()).Returns(1);

        // Act
        _context.Update(1, entity);

        // Assert
        _mockCommand.Verify(c => c.ExecuteNonQuery(), Times.Once);
    }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
}