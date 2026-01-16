using System.Net;
using System.Net.Http.Json;

namespace PeopleApp.Client.Services.Http;

public static class ApiErrorParser
{
    public static async Task<string> ToUserMessageAsync(HttpResponseMessage response)
    {
        // Mensajes por status (UX)
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return "Credenciales inválidas o tu sesión expiró.";

        if (response.StatusCode == HttpStatusCode.Forbidden)
            return "No tienes permisos para realizar esta acción.";

        // 1) Intentar leer ProblemDetails (validation errors incluidos)
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
            if (problem is not null)
            {
                // Si hay errores de validación, muestra el primero
                if (problem.Errors is not null && problem.Errors.Count > 0)
                {
                    var first = problem.Errors.First().Value?.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(first))
                        return ErrorTranslatorEs.Translate(first!);
                }

                if (!string.IsNullOrWhiteSpace(problem.Detail))
                    return ErrorTranslatorEs.Translate(problem.Detail!);

                if (!string.IsNullOrWhiteSpace(problem.Title))
                    return ErrorTranslatorEs.Translate(problem.Title!);
            }
        }
        catch
        {
            // Si no es JSON compatible, ignoramos y seguimos
        }

        // 2) Si el backend devolvió texto plano (ej: Conflict("El email ya está registrado"))
        var raw = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(raw))
            return ErrorTranslatorEs.Translate(raw);

        // 3) Fallback final
        return ErrorTranslatorEs.Translate($"Error {(int)response.StatusCode}: no se pudo completar la solicitud.");
    }
}
