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
    public class SamePropertyName_SourceInjectedQuery : AutoMapperSpecBase
    {
        private readonly Source[] _source = new[]
        {
            new Source
            {
                ValueProp = 5,
            },
            new Source
            {
                ValueProp = 6,
            },
            new Source
            {
                ValueProp = 7,
            }
        };

        private class Source
        {
            public int ValueProp { get; set; }
        }

        private class IntermediateLayer
        {
            public int ValueProp { get; set; }
        }

        private class Destination
        {
            public int ValueProp { get; set; }
        }

        protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Destination, IntermediateLayer>()
                .ReverseMap()
                ;

            cfg.CreateMap<IntermediateLayer, Source>()
                .ReverseMap()
                ;
        });

        [Fact]
        public void Shoud_support_source_to_intermediate_result_toList()
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