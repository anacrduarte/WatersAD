namespace WatersAD.Helpers
{
    public interface IMailHelper
    {

        Task<Response> SendMail(string toName, string toEmail, string subject, string body, MemoryStream pdfStream = null, string fileName = "fatura.pdf");

        //Task<Response> SendMail2(string toName, string toEmail, string subject, string body, MemoryStream pdfStream = null, string fileName = "fatura.pdf");
    }
}
