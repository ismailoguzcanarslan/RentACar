﻿using Application.Features.Brands.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.PipeLines.Caching;
using Core.Application.PipeLines.Logging;
using Core.Application.PipeLines.Transaction;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Brands.Commands.Create;

public class CreateBrandCommand : IRequest<CreatedBrandResponse>, ICacheRemoverRequest, ILoggableRequest
{
    public string Name { get; set; }

    public string? CacheKey => "";

    public bool ByPassCache { get; }

    public string? CacheGroupKey => "GetBrands";

    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, CreatedBrandResponse>, ITransactionalRequest
    {

        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly BrandBusinessRules _brandBusinessRules;    

        public CreateBrandCommandHandler(IBrandRepository brandRepository, IMapper mapper, BrandBusinessRules brandBusinessRules)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _brandBusinessRules = brandBusinessRules;
        }

        public async Task<CreatedBrandResponse>? Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {

            await _brandBusinessRules.BrandNameCanNotBeDublicatedWhenInserted(request.Name);

            Brand brand = _mapper.Map<Brand>(request);
            brand.Id = Guid.NewGuid();

            //Brand brand2 = _mapper.Map<Brand>(request);
            //brand2.Id = Guid.NewGuid();

            await _brandRepository.AddAsync(brand);
            //await _brandRepository.AddAsync(brand2);

            CreatedBrandResponse createdBrandResponse = _mapper.Map<CreatedBrandResponse>(brand);

            return createdBrandResponse;
        }
    }
}
