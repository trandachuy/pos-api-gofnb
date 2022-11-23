using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Email;
using GoFoodBeverage.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

using MoreLinq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.EmailCampaigns.Commands
{
    public class TriggerSendEmailCampaignRequest : IRequest<TriggerSendEmailCampaignResponse>
    {
        public double Interval { get; set; } = DefaultConstants.SENDING_EMAIL_CAMPAIGN_INTERVAL;
    }

    public class TriggerSendEmailCampaignResponse
    {
        public bool Success { get; set; }

        public TriggerSendEmailCampaignResponse(bool success)
        {
            Success = success;
        }
    }

    public class TriggerSendEmailCampaignRequestHandler : IRequestHandler<TriggerSendEmailCampaignRequest, TriggerSendEmailCampaignResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSenderProvider _emailProvider;

        public TriggerSendEmailCampaignRequestHandler(IUnitOfWork unitOfWork, IEmailSenderProvider emailProvider)
        {
            _unitOfWork = unitOfWork;
            _emailProvider = emailProvider;
        }

        public async Task<TriggerSendEmailCampaignResponse> Handle(TriggerSendEmailCampaignRequest request, CancellationToken cancellationToken)
        {
            var response = new TriggerSendEmailCampaignResponse(true);

            var emailCampaigns = await _unitOfWork.EmailCampaigns.GetAllByLastMinutes(request.Interval).ToListAsync(cancellationToken);
            if (emailCampaigns.Any())
            {
                var listInsertEmailCampaignSendingTransaction = new List<EmailCampaignSendingTransaction>();
                foreach (var emailCampaign in emailCampaigns)
                {
                    var emailCampaignSendingTransactions = emailCampaign.EmailCampaignSendingTransactions;
                    if (!emailCampaignSendingTransactions.Any())
                    {
                        // Send to 1 Customer Email Address
                        if (emailCampaign.EmailCampaignType == EnumEmailCampaignType.SendToEmailAddress)
                        {
                            var sendingTransaction = await SendMailToCustomer(emailCampaign.EmailSubject, emailCampaign.Template, emailCampaign.EmailAddress, emailCampaign.Id);
                            listInsertEmailCampaignSendingTransaction.Add(sendingTransaction);
                        }
                        // Send to Groups Customer Segments
                        else
                        {
                            var customerEmails = new List<string>();
                            var customerCustomerSegments = emailCampaign.EmailCampaignCustomerSegments.Select(x => x.CustomerSegment.CustomerCustomerSegments);
                            customerCustomerSegments.ForEach(ccs =>
                            {
                                var emails = ccs.Select(c => c.Customer.Email)
                                    .Where(mail => !string.IsNullOrEmpty(mail))
                                    .Distinct();
                                customerEmails.AddRange(emails);
                            });

                            if (customerEmails.Any())
                            {
                                var sendingTransactions = await BulkSendMailToCustomers(emailCampaign.EmailSubject, emailCampaign.Template, customerEmails, emailCampaign.Id);
                                listInsertEmailCampaignSendingTransaction.AddRange(sendingTransactions);
                            }
                        }
                    }
                }

                if (listInsertEmailCampaignSendingTransaction.Any())
                {
                    _unitOfWork.EmailCampaignSendingTransactions.AddRange(listInsertEmailCampaignSendingTransaction);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return response;
        }

        private async Task<EmailCampaignSendingTransaction> SendMailToCustomer(string subject, string body, string receiverEmail, Guid emailCampaignId)
        {
            var sendingResult = await _emailProvider.SendEmailAsync(subject, body, receiverEmail);
            var emailCampaignSendingTransaction = new EmailCampaignSendingTransaction()
            {
                EmailCampaignId = emailCampaignId,
                CustomerEmail = receiverEmail,
                Status = sendingResult ? EnumEmailCampaignSendingStatus.Success : EnumEmailCampaignSendingStatus.Failed
            };
            return emailCampaignSendingTransaction;
        }

        private async Task<List<EmailCampaignSendingTransaction>> BulkSendMailToCustomers(string subject, string body, List<string> receiverEmails, Guid emailCampaignId)
        {
            var tasks = new List<Task<EmailCampaignSendingTransaction>>();
            var batchSize = DefaultConstants.SEND_EMAIL_BATCH_SIZE;
            int numberOfBatches = (int)Math.Ceiling((double)receiverEmails.Count / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var currentEmails = receiverEmails.Skip(i * batchSize).Take(batchSize);
                var listTask = currentEmails.Select(email =>
                {
                    return SendMailToCustomer(subject, body, email, emailCampaignId);
                });
                tasks.AddRange(listTask);
            }
            var result = await Task.WhenAll(tasks);
            return result.ToList();
        }
    }
}