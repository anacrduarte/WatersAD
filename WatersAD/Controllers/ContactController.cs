using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public ContactController(IMailHelper mailHelper, IFlashMessage flashMessage)
        {
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendContactForm([FromForm] ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subject = $"Nova mensagem de {model.FullName}";
                var messageBody = $"Nome: {model.FullName}\nEmail: {model.Email}\nMensagem: {model.Message}";

                Response response = await _mailHelper.SendMail("ana.cinel.testes@gmail.com", "ana.cinel.testes@gmail.com", subject, messageBody);

                if (response.IsSuccess)
                {
                    _flashMessage.Info("Mensagem enviada com sucesso!");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _flashMessage.Info("Erro ao enviar a mensagem. ");
                    return RedirectToAction("Index", "Home");
                }
               
            }
            _flashMessage.Info("Erro no formulário. ");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("SendNewsletter")]
        public async Task<IActionResult> SendNewsletter([FromForm] NewsletterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subject = "Águas Duarte – Conexão com a Natureza";
                var messageBody = @"
                <!DOCTYPE html>
                <html lang='pt-PT'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Águas Duarte - Conexão com a Natureza</title>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            margin: 0;
                            padding: 20px;
                            background-color: #f9f9f9;
                            color: #333;
                        }
                        .container {
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #fff;
                            padding: 20px;
                            border-radius: 5px;
                            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                        }
                        h1, h2, h3 {
                            color: #0056b3;
                        }
                        a {
                            color: #0056b3;
                            text-decoration: none;
                        }
                        footer {
                            margin-top: 20px;
                            font-size: 12px;
                            color: #888;
                        }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>Águas Duarte</h1>
                        <h2>Distribuição de Água de Qualidade</h2>
                        <p><strong>Edição: Setembro 2024</strong></p>
                        <h3>1. Mensagem do Diretor</h3>
                        <p>Caro cliente,</p>
                        <p>Na Águas Duarte, o nosso compromisso é fornecer água de qualidade, respeitando o meio ambiente e a saúde da nossa comunidade. Agradecemos a sua confiança e estamos aqui para servi-lo!</p>
                        <h3>2. Destaque do Mês: A Importância da Água Potável</h3>
                        <p>A água é essencial para a nossa saúde e bem-estar. Nesta edição, destacamos a importância da água potável e como ela afeta a sua qualidade de vida. Lembre-se de beber pelo menos 2 litros de água por dia!</p>
                        <p><a href='#'>Leia mais sobre os benefícios da água potável</a></p>
                        <h3>3. Novidades: Sistema de Entrega Rápida</h3>
                        <p>Temos o prazer de anunciar o lançamento do nosso novo sistema de entrega rápida! Agora, pode receber água diretamente na sua casa em menos de 24 horas. Para mais informações, visite o nosso site ou ligue para o nosso serviço de apoio ao cliente.</p>
                        <h3>4. Dicas de Sustentabilidade: Como Economizar Água</h3>
                        <ul>
                            <li>Feche a torneira enquanto escova os dentes.</li>
                            <li>Utilize a máquina de lavar roupa apenas quando estiver cheia.</li>
                            <li>Instale redutores de fluxo nas torneiras e chuveiros.</li>
                        </ul>
                        <p>Essas pequenas ações ajudam a conservar um recurso precioso!</p>
                        <h3>5. Promoção Especial: 15% de Desconto em Planos de Assinatura</h3>
                        <p>Aproveite esta promoção exclusiva para nossos assinantes! Assine um plano de distribuição mensal e receba 15% de desconto no primeiro mês. Use o código: <strong>AGUAS15</strong>.</p>
                        <p><a href='#'>Inscreva-se agora</a></p>
                        <footer>
                            <p>Tem perguntas ou sugestões? Responda a este e-mail ou entre em contato connosco pelo nosso site.</p>
                            <p>Siga-nos nas redes sociais: 
                                <a href='#'>Facebook</a> | 
                                <a href='#'>Instagram</a> | 
                                <a href='#'>LinkedIn</a>
                            </p>
                            <p>Você recebeu este e-mail porque se inscreveu na nossa newsletter.</p>
                        </footer>
                    </div>
                </body>
                </html>";

               
                Response response = await _mailHelper.SendMail("ana.cinel.testes@gmail.com", model.Email, subject, messageBody);

                if (response.IsSuccess)
                {
                    _flashMessage.Info("Newsletter enviada com sucesso!");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _flashMessage.Info("Erro ao enviar a newsletter.");
                    return RedirectToAction("Index", "Home");
                }
            }

            _flashMessage.Info("Erro no formulário da newsletter.");
            return RedirectToAction("Index", "Home");
        }
    }
}
