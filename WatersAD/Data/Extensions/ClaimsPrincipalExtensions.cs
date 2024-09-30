using System.Security.Claims;

namespace WatersAD.Data.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetInitials(this ClaimsPrincipal user)
        {
            if (user == null || !user.Identity.IsAuthenticated || string.IsNullOrEmpty(user.Identity.Name))
                return ""; // Valor padrão se não estiver autenticado ou sem nome

            var fullName = user.Identity.Name;
            var names = fullName.Split(' ');
            if (names.Length > 1)
            {
                // Pega a primeira letra do primeiro e do último nome
                return $"{names[0][0]}{names[names.Length - 1][0]}".ToUpper();
            }
            else
            {
                // Se houver apenas um nome, pega as duas primeiras letras
                return fullName.Substring(0, 2).ToUpper();
            }
        }
    }
}
