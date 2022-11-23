using AutoMapper;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Common;
using GoFoodBeverage.Models.EmailCampaign;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.EmailCampaigns.Commands
{
    public class CreateEmailCampaignRequest : IRequest<CreateEmailCampaignResponse>
    {
        public Guid? StoreId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string EmailSubject { get; set; }

        public DateTime SendingTime { get; set; }

        public EnumEmailCampaignType EmailCampaignType { get; set; }

        public string EmailAddress { get; set; } // manual mapping

        public List<Guid> CustomerSegmentIds { get; set; } // manual mapping

        public string PrimaryColor { get; set; }

        public string SecondaryColor { get; set; }

        public string LogoUrl { get; set; }

        public string Title { get; set; }

        public string FooterContent { get; set; }

        public string Template { get; set; }

        public List<EmailCampaignDetailModel> EmailCampaignDetails { get; set; }

        public List<EmailCampaignSocialModel> EmailCampaignSocials { get; set; }

        public List<EmailCampaignCustomerSegmentModel> EmailCampaignCustomerSegments { get; set; }
    }

    public class CreateEmailCampaignResponse : ResponseCommonModel { }

    public class CreateEmailCampaignRequestHandler : IRequestHandler<CreateEmailCampaignRequest, CreateEmailCampaignResponse>
    {
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserProvider _userProvider;

        public CreateEmailCampaignRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<CreateEmailCampaignResponse> Handle(CreateEmailCampaignRequest request, CancellationToken cancellationToken)
        {
            CreateEmailCampaignResponse response = new CreateEmailCampaignResponse();
            // Get user information from the token.
            var loggedUser = await _userProvider.ProvideAsync();
            request.StoreId = loggedUser.StoreId;
            // Mapping to email campaign.
            // When adding a new data field, that field needs to match the name of the field from the entity side to avoid errors when performing auto mapping.
            EmailCampaign emailCampaignEntity = _mapper.Map<EmailCampaign>(request);
            var convertUtcTime = emailCampaignEntity.SendingTime.ToUniversalTime();
            emailCampaignEntity.SendingTime = convertUtcTime.AddSeconds(-1 * convertUtcTime.Second);

            using var createEmailCampaignTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.EmailCampaigns.AddAsync(emailCampaignEntity);

                await _unitOfWork.SaveChangesAsync();
                await createEmailCampaignTransaction.CommitAsync(cancellationToken);

                response.IsSuccess = true;
                response.Guid = emailCampaignEntity.Id;
                return response;
            }
            catch
            {
                // Data will be restored.
                await createEmailCampaignTransaction.RollbackAsync(cancellationToken);

                response.IsSuccess = false;
                return response;
            }
        }
    }
}
