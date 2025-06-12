using Application.Mapping;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;


namespace Application
{
    public static class ApplicationInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        }
    }
}
