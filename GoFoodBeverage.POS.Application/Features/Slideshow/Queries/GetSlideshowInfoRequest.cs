using GoFoodBeverage.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Slideshow.Queries
{
    public class GetSlideshowInfoRequest : IRequest<GetSlideshowInfoResponse>
    {
        public int Type { get; set; } = -1;
    }

    public class GetSlideshowInfoResponse
    {
        public int TotalCount { get; set; }

        public IEnumerable<SlideshowInfo> Results { get; set; }
    }

    public class SlideshowInfo
    {
        public string Name { get; set; }

        public int Type { get; set; }

        public string FileUrl { get; set; }
    }

    public class GetSlideshowInfoHandler : IRequestHandler<GetSlideshowInfoRequest, GetSlideshowInfoResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSlideshowInfoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetSlideshowInfoResponse> Handle(GetSlideshowInfoRequest request, CancellationToken cancellationToken)
        {
            var slideshow = _unitOfWork.FileUpload.GetSliderImagesAsync().Where(x => x.IsActivated == true);

            if (request.Type != -1)
            {
                slideshow = slideshow.Where(x => x.Type == request.Type);
            }

            var results = slideshow.Select(x => new SlideshowInfo()
            {
                FileUrl = x.FileUrl,
                Name = x.Name,
                Type = x.Type,
            })
            .ToList();

            var response = new GetSlideshowInfoResponse()
            {
                Results = results,
                TotalCount = results.Count
            };

            return response;
        }
    }
}