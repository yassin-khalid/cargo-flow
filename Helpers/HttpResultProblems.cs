using Microsoft.AspNetCore.Http.HttpResults;

namespace CargoFlow.Helpers;

public static class HttpResultProblems
{
    public static ValidationProblem ValidationProblem(string field, string[] messages)
    {
        return TypedResults.ValidationProblem(
            errors: new Dictionary<string, string[]>
            {
                [field] = messages
            });
    }
}
