using System.Security.Claims;


namespace WatersAD.Data.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
     
        public static string GetInitials(this ClaimsPrincipal user)
        {
            if (user == null || !user.Identity.IsAuthenticated || string.IsNullOrEmpty(user.Identity.Name))
                return ""; 

            var fullNameClaim = user.FindFirst("FullName");

            var fullName = fullNameClaim?.Value;

            if (string.IsNullOrEmpty(fullName))
                return "";

            var names = fullName.Split(' ');
            if (names.Length > 1)
            {
                
                return $"{names[0][0]}{names[names.Length - 1][0]}".ToUpper();
            }
            else
            {
                
                return fullName.Substring(0, 2).ToUpper();
            }
        }

        public static string GetImageUrl(this ClaimsPrincipal user)
        {
            if (user == null || !user.Identity.IsAuthenticated)
                return ""; 

            
            var imageUrlClaim = user.FindFirst("ImageUrl");

            var imageUrl = imageUrlClaim?.Value ?? "";


           
            if (imageUrl.Equals("~/image/noimage.png", StringComparison.OrdinalIgnoreCase))
            {
                
                return ""; 
            }

            var filePath = Path.Combine(
             Directory.GetCurrentDirectory(),
             "wwwroot",
             imageUrl.TrimStart('~', '/').Replace('/', '\\'));

            if (!File.Exists(filePath))
            {
                return "";
            }
            return imageUrl;
        }
    }
}
