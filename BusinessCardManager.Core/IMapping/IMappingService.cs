// MappingService.cs provides an abstraction layer for object mapping 
// using AutoMapper. It defines an interface, IMappingService, that 
// outlines the mapping operations for converting between source and 
// destination object types. The service facilitates both single and 
// collection mapping, promoting clean code and separation of concerns.

using AutoMapper;
using System;
using System.Collections.Generic;

namespace BusinessCardManager.Core.IMapping
{
    public interface IMappingService
    {
        // Maps a single source object to a destination type
        TDestination Map<TSource, TDestination>(TSource source);

        // Maps a source object to an existing destination object
        void Map<TSource, TDestination>(TSource source, TDestination destination);
    }

    public class MappingService : IMappingService
    {
        private readonly IMapper _mapper;

        public MappingService(IMapper mapper)
        {
            _mapper = mapper;
        }

        // Maps a single source object to a destination type
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TDestination>(source);
        }

        // Maps a source object to an existing destination object
        public void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            _mapper.Map(source, destination);
        }

        // Maps a source object to a destination type with inferred source type
        public TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        // Maps a collection of source objects to a collection of destination types
        public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source)
        {
            return _mapper.Map<IEnumerable<TDestination>>(source);
        }
    }
}
