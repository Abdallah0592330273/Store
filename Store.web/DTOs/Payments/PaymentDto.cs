using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.NetworkInformation;

namespace Store.Api.DTOs.Payments
{
    public record PaymentDto(
    int PaymentId,
    int OrderId,
    string Method,
    decimal Amount,
    string Status,
    DateTime CreatedAt
);

}
