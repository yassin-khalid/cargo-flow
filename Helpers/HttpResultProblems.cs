using Microsoft.AspNetCore.Http.HttpResults;

namespace CargoFlow.Helpers;

public static class HttpResultProblems
{
    public static ProblemHttpResult ValidationProblem(string title, string detail, string field, string[] messages)
    {
        return TypedResults.Problem(
        statusCode: StatusCodes.Status400BadRequest,
        detail: detail,
        title: title,
        extensions: new Dictionary<string, object?>
        {
            ["errors"] = new Dictionary<string, string[]>
            {
                [field] = messages
            }
        });
    }
}
