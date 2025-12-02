using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Commands.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Credentials", new[] { "Invalid username or password" } }
            });
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto(
            user.Id,
            user.Username,
            user.Email,
            token);
    }
}
