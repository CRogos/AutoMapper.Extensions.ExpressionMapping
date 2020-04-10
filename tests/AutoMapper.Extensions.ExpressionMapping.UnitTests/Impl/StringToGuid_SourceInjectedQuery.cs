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
    public class StringToGuid_SourceInjectedQuery : AutoMapperSpecBase
    {
        private readonly Source[] _source = new[]
        {
            new Source
            {
                ValueProp =  "90875833-20f0-4735-bf3f-f92fa678bca6"
            },
            new Source
            {
                ValueProp = "3b0db672-f2ff-40ed-a883-6c5661a85c4b"
            },
            new Source
            {
                ValueProp = "fe2ed13d-d4b2-4773-bc47-4f3f83ae5175"
            }
        };

        private class Source
        {
            public string ValueProp { get; set; }
        }

        private class IntermediateLayer
        {
            public Guid ValueProp { get; set; }
        }

        private class Destination
        {
            public Guid ValueProp { get; set; }
        }

        protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<string, Guid>().ConvertUsing(s => new Guid(s));

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