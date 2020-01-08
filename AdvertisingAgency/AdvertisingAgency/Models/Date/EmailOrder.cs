using AdvertisingAgency.Models.ViewModels.Account;
using AdvertisingAgency.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace AdvertisingAgency.Models.Date
{

    public class EmailOrderProcessor  //: IOrderProcessor
    {

        public void ProcessOrder(List<CartVM> cart, UserVM UserInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = true;
                smtpClient.Host = "smtp.mai.ru";
                smtpClient.Port = 999;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("pass", "pass");

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtpClient.PickupDirectoryLocation = @"c:\rcshop _emails";
                smtpClient.EnableSsl = false;

                StringBuilder body = new StringBuilder()
                    .AppendLine("Новый заказ")
                    .AppendLine("---")
                    .AppendLine("Реклама:");

                var Total = 0;
                foreach (var item in cart)
                {
                    body.AppendFormat("{0} {1} {2} {3} {4} (итого: {5:c}",
                        item.ProductId, item.ProductName, item.Quantity, item.Price, item.Total);
                    Total += item.Total;
                }

                body.AppendFormat("Общая стоимость: {0:c}", Total)
                    .AppendLine("---")
                    .AppendLine("Данные пользователя:")
                    .AppendLine(UserInfo.FirstName)
                    .AppendLine(UserInfo.LastName)
                    .AppendLine(UserInfo.EmailAdress)
                    .AppendLine(UserInfo.Username);

                MailMessage mailMessage = new MailMessage(
                                       "PROmedia@mail.ru",              // От кого
                                       "adminPROmedia@mail.ru",         // Кому
                                       "Новый заказ отправлен!",        // Тема
                                       body.ToString());                // Тело письма


                mailMessage.BodyEncoding = Encoding.UTF8;

                smtpClient.Send(mailMessage);
            }

        }
    }
}