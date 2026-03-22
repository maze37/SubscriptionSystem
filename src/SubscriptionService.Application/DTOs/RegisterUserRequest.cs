namespace SubscriptionService.Application.DTOs;

/// <summary>HTTP request модель для регистрации пользователя.</summary>
/// <param name="Email">Email пользователя.</param>
public record RegisterUserRequest(string Email);