using Aggregation.Api.Domain.Enums;

namespace Aggregation.Api.Application.Interfaces;

public interface ICategorizationService
{
    TransactionCategory Categorize(string description);
}