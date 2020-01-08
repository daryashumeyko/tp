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
    public class EmailOrder
    {
        public string MailToAddress = "adminPROmedia@mail.ru";
        public string MailFromAddress = "PROmedia@mail.ru";
        public bool UseSsl = true;
        public string Username = "pass";
        public string Password = "pass";
        public string ServerName = "smtp.mai.ru";
        public int ServerPort = 999;
        public bool WriteAsFile = true;
        public string FileLocation = @"c:\rcshop _emails";
    }

    public class EmailOrderProcessor  //: IOrderProcessor
    {
        private EmailOrder emailOrder;

        public EmailOrderProcessor(EmailOrder order)
        {
            emailOrder = order;
        }
        
            public void ProcessOrder(CartVM cart, UserProfileVM UserInfo)
            {
                using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.EnableSsl = emailOrder.UseSsl;
                        smtpClient.Host = emailOrder.ServerName;
                        smtpClient.Port = emailOrder.ServerPort;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(emailOrder.Username, emailOrder.Password);

                        if (emailOrder.WriteAsFile)
                        {
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                            smtpClient.PickupDirectoryLocation = emailOrder.FileLocation;
                            smtpClient.EnableSsl = false;
                        }

                        StringBuilder body = new StringBuilder()
                            .AppendLine("Новый заказ")
                            .AppendLine("---")
                            .AppendLine("Товары:");

                        foreach (var item in cart)
                        {
                            var subtotal = item.Game.Price * item.Quantity;
                            body.AppendFormat("{0} x {1} (итого: {2:c}",
                                item.Quantity, item.Game.Name, subtotal);
                        }

                        body.AppendFormat("Общая стоимость: {0:c}", cart.Total)
                            .AppendLine("---")
                            .AppendLine("Данные пользователя:")
                            .AppendLine(UserInfo.FirstName)
                            .AppendLine(UserInfo.LastName)
                            .AppendLine(UserInfo.EmailAdress)
                            .AppendLine(UserInfo.Username);

                        MailMessage mailMessage = new MailMessage(
                                               emailOrder.MailFromAddress,	    // От кого
                                               emailOrder.MailToAddress,		// Кому
                                               "Новый заказ отправлен!",		// Тема
                                               body.ToString()); 				// Тело письма

                        if (emailOrder.WriteAsFile)
                        {
                            mailMessage.BodyEncoding = Encoding.UTF8;
                        }

                        smtpClient.Send(mailMessage);
                    }
                
            }*
    }
}