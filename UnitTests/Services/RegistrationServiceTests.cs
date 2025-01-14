using AutoMapper;
using Common.Dto.Auth;
using Common.Exceptions;
using DAL.Repositories.Abstract;
using Domain;
using Domain.Abstract;
using Service.Abstract.Auth;
using Service.Auth;

namespace UnitTests.Services;

public class RegistrationServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IEncryptionService _encryptionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CancellationToken _ct;
    private readonly RegistrationService _subject;

    public RegistrationServiceTests()
    {
        _ct = CancellationToken.None;
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _encryptionService = Substitute.For<IEncryptionService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _subject = new RegistrationService(
            _userRepository,
            _mapper,
            _encryptionService,
            _unitOfWork
        );
    }

    [Fact]
    public async Task RegisterAsync_ShouldAddUser_IfRequestIsOk()
    {
        // Arrange
        const string nickname = "joepeach";
        const string hash = "hash";
        var request = new RegistrationDto
        {
            Username = nickname,
            Password = "passwd",
            ConfirmPassword = "passwd"
        };

        var user = new User
        {
            Id = 0,
            Password = "passwd",
            Username = nickname
        };

        _userRepository.IsNicknameOccupied(nickname, _ct).Returns(false);
        _mapper.Map<User>(request).Returns(user);
        _encryptionService.EncryptPassword("passwd").Returns(hash);
        _userRepository.AddAsync(user, _ct).Returns(user);

        // Act
        var actual = await _subject.RegisterAsync(request, _ct);
        
        // Assert
        actual.Username.Should().Be(nickname);
        actual.PasswordHash.Should().Be(hash);
        await _unitOfWork.ReceivedWithAnyArgs(1).CommitAsync(_ct);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowIf_NicknameIsOccupied()
    {
        // Arrange
        const string nickname = "joepeach";

        var request = new RegistrationDto
        {
            Username = nickname,
            FirstName = null,
            LastName = null,
            Password = "passwd",
            ConfirmPassword = "passwd"
        };

        _userRepository.IsNicknameOccupied(nickname, _ct).Returns(true);

        // Act
        var actual = () => _subject.RegisterAsync(request, _ct);

        // Assert
        await actual.Should().ThrowExactlyAsync<ValidationException>($"Username {nickname} is occupied");
    }

    [Theory]
    [InlineData("Length", "passwd", "passwdddasdasd")]
    [InlineData("Case 1", "passwd", "PASSWD")]
    [InlineData("Case 2", "c0nteNt", "c0nTeNt")]
    public async Task RegisterAsync_ShouldThrow_When_PasswordsAreNotEqual(string testKind, string passwd, string confirmPasswd)
    {
        // Arrange
        const string nickname = "joepeach";

        var request = new RegistrationDto
        {
            Username = nickname,
            FirstName = null,
            LastName = null,
            Password = passwd,
            ConfirmPassword = confirmPasswd
        };

        _userRepository.IsNicknameOccupied(nickname, _ct).Returns(false);

        // Act
        var actual = () => _subject.RegisterAsync(request, _ct);

        // Assert
        await actual.Should().ThrowExactlyAsync<ValidationException>("Passwords do not match",new object[]
        {
            testKind
        });
    }
}