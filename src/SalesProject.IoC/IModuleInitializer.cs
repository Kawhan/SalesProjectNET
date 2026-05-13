using Microsoft.AspNetCore.Builder;

namespace SalesProject.IoC;

public interface IModuleInitializer
{
    void Initialize(WebApplicationBuilder builder);
}
