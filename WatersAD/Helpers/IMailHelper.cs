namespace WatersAD.Helpers
{
    public interface IMailHelper
    {

        Task<Response> SendMail(string toName, string toEmail, string subject, string body);


    }
}
