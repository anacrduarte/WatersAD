namespace WatersAD.Helpers
{
    public interface IMailHelper
    {
        /// <summary>
        /// Asynchronously sends an email with the specified details, optionally including a PDF attachment.
        /// </summary>
        /// <param name="toName">The name of the recipient.</param>
        /// <param name="toEmail">The email address of the recipient.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <param name="pdfStream">An optional <see cref="MemoryStream"/> containing the PDF data to be attached to the email; defaults to null.</param>
        /// <param name="fileName">The name of the PDF file attachment; defaults to "invoice.pdf".</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response"/> object indicating the result of the email sending operation.</returns>
        Task<Response> SendMail(string toName, string toEmail, string subject, string body, MemoryStream pdfStream = null, string fileName = "invoice.pdf");

    }
}
