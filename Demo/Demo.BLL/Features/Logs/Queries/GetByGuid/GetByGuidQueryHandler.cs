using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.Log;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.BLL.Features.Logs.Queries.GetByGuid
{
    public class GetByGuidQueryHandler : IRequestHandler<GetByGuidQuery, IEnumerable<LogDto>>
    {
        private readonly IDemoUnitOfWork _demoUnitOfWork;
        private readonly IMapper _mapper;

        public GetByGuidQueryHandler(
            IDemoUnitOfWork demoUnitOfWork,
            IMapper mapper)
        {
            _mapper = mapper;
            _demoUnitOfWork = demoUnitOfWork;
        }

        public async Task<IEnumerable<LogDto>> Handle(GetByGuidQuery request, CancellationToken cancellationToken)
        {
            var logs = await _demoUnitOfWork.LogRepository.GetMultipleAsync(x=>x.TraceId == request.Guid,
                cancellationToken);

            return _mapper.Map<IEnumerable<LogDto>>(logs);
        }
    }
}

