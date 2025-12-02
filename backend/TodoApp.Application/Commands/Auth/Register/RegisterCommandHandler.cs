using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Commands.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Username", new[] { "Username is already taken" } }
            });
        }

        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Email", new[] { "Email is already registered" } }
            });
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto(
            user.Id,
            user.Username,
            user.Email,
            token);
    }
}
