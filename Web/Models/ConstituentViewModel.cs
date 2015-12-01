using System;
using AutoMapper;
using Domain;
using Web.Controllers.Api;
using Web.Infrastructure.Mapping;

namespace Web.Models
{
    public class ConstituentViewModel : IHaveCustomMappings, IEquatable<ConstituentViewModel>
    {
        public int Id { get; set; }
        public string LookupId { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public void CreateMappings(IConfiguration config)
        {
            config.CreateMap<CsvTaxRecordViewModel, ConstituentViewModel>()
                .ForMember(m => m.LookupId, opt => opt.MapFrom(u => u.LookupId))
                .ForMember(m => m.Street, opt => opt.MapFrom(u => u.Addressline1))
                .ForMember(m => m.Street2, opt => opt.MapFrom(u => u.Addressline2))
                .ReverseMap();


            config.CreateMap<Constituent, ConstituentViewModel>()
                .ForMember(m => m.LookupId, opt => opt.MapFrom(u => u.LookupId))
                .ReverseMap();

        }

        public bool Equals(ConstituentViewModel other)
        {
            if (other == null) return false;

            if (other.LookupId != LookupId) return false;
            if (other.Name != Name) return false;
            if (other.Street != Street) return false;
            if (other.Street2 != Street2) return false;
            if (other.City != City) return false;
            if (other.State != State) return false;
            if (other.Zipcode != Zipcode) return false;
            if (other.Email != Email) return false;
            if (other.Phone != Phone) return false;

            return true;
        }
    }
}