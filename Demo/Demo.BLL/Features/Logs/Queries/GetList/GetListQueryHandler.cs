using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.Log;
using MediatR;

namespace Demo.BLL.Features.Logs.Queries.GetList
{
    public class GetInstructionByQrCodeAndTypeQueryHandler : IRequestHandler<GetListQuery, IEnumerable<LogDto>>
    {
        private readonly IDemoUnitOfWork _demoUnitOfWork;
        private readonly IMapper _mapper;

        public GetInstructionByQrCodeAndTypeQueryHandler(
            IMapper mapper, IDemoUnitOfWork demoUnitOfWork)
        {
            _mapper = mapper;
            _demoUnitOfWork = demoUnitOfWork;
        }

        public async Task<IEnumerable<LogDto>> Handle(GetListQuery request, CancellationToken cancellationToken)
        {
            var logs = await _demoUnitOfWork.LogRepository.GetMultipleAsync(null,
                cancellationToken);

            return _mapper.Map<IEnumerable<LogDto>>(logs);
        }
    }
}

