using ASPNET.Data;
using ASPNET.Models;
using AutoMapper;

namespace ASPNET.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper() 
        {
            CreateMap<Book, BookModel>().ReverseMap();
        }
    }
}
