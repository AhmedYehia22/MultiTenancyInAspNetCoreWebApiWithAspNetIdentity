﻿namespace MultiTenancy.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML);
    }
}
