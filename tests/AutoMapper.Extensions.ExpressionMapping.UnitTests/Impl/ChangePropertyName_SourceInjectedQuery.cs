using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper.Mappers;
using Shouldly;
using Xunit;

namespace AutoMapper.Extensions.ExpressionMapping.UnitTests.Impl
{
    public class ChangePropertyName_SourceInjectedQuery : AutoMapperSpecBase
    {
        private readonly Source[] _source = new[]
        {
            new Source
            {
                SourceProp = 5,
            },
            new Source
            {
                SourceProp = 6,
            },
            new Source
            {
                SourceProp = 7,
            }
        };

        private class Source
        {
            public int SourceProp { get; set; }
        }

        private class IntermediateLayer
        {
            public int IntermediateProp { get; set; }
        }

        private class Destination
        {
            public int DestinationProp { get; set; }
        }

        protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Destination, IntermediateLayer>()
                .ForMember(dest => dest.IntermediateProp, opt => opt.MapFrom(src => src.DestinationProp))
                .ReverseMap()
                ;

            cfg.CreateMap<IntermediateLayer, Source>()
                .ForMember(dest => dest.SourceProp, opt => opt.MapFrom(src => src.IntermediateProp))
                .ReverseMap()
                ;
        });

        [Fact]
        public void Shoud_support_source_to_intermediate_const_toList()
        {
            IQueryable<IntermediateLayer> result = _source.AsQueryable()
              .UseAsDataSource(Mapper).For<IntermediateLayer>();

            result.ToList();
        }

        [Fact]
        public void Shoud_support_intermediate_to_destination_result_toList()
        {
            IQueryable<IntermediateLayer> intermediateSource = _source.AsQueryable()
              .UseAsDataSource(Mapper).For<IntermediateLayer>()
              // when persist source before next mapping it is always working
              .ToArray().AsQueryable();

            IQueryable<Destination> result = intermediateSource
                .UseAsDataSource(Mapper).For<Destination>();

            result.ToList();
        }

        [Fact]
        public void Shoud_support_source_to_destination_result_toList()
        {
            IQueryable<IntermediateLayer> intermediateSource = _source.AsQueryable()
              .UseAsDataSource(Mapper).For<IntermediateLayer>();

            IQueryable<Destination> result = intermediateSource
                .UseAsDataSource(Mapper).For<Destination>();

            result.ToList();
        }
    }
}