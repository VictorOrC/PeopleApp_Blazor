namespace PeopleApp.Client.Services.Http;

public static class ErrorTranslatorEs
{
    public static string Translate(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return "Ocurrió un error.";

        // comunes de Identity/DataAnnotations
        message = message.Replace("The Email field is not a valid e-mail address.", "El correo no tiene un formato válido.");
        message = message.Replace("The field Password must be a string or array type with a minimum length of '8'.", "La contraseña debe tener mínimo 8 caracteres.");
        message = message.Replace("The field PhoneNumber must be a string or array type with a minimum length of '10'.", "El teléfono debe tener mínimo 10 dígitos.");

        // si te llega esto:
        message = message.Replace("Passwords do not match.", "Las contraseñas no coinciden.");

        return message;
    }
}
