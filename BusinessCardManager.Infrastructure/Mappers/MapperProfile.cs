// MapperProfile.cs defines the mapping configuration for the AutoMapper library, 
// establishing the relationships between the BusinessCard entity and the 
// AddBusinessCardDto data transfer object. This profile enables seamless 
// object-to-object mapping, facilitating the transfer of data between 
// different layers of the application, such as the service and presentation layers.

using AutoMapper;
using BusinessCardManager.Core.DTOs.BusinessCardDto;
using BusinessCardManager.Core.Entities;
using System;

namespace BusinessCardManager.Infrastructure.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Creates a mapping configuration between BusinessCard and AddBusinessCardDto
            // Another mapper  between BusinessCard and BusinessCardCsvXmlDto
            // The ReverseMap() method allows for two-way mapping, enabling both 
            // conversions: BusinessCard to AddBusinessCardDto and vice versa.
            CreateMap<BusinessCard, AddBusinessCardDto>().ReverseMap();
            CreateMap<BusinessCardCsvXmlDto, BusinessCard>().ReverseMap();
        }
    }
}
