using Domain.Features.IncomingInvoices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Features.IncomingInvoices;

public static class IncomingInvoicesServiceCollectionExtensions
{
    public static IServiceCollection AddIncomingInvoiceParsers(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceParser, InterCarsInvoiceParser>();
        services.AddScoped<IInvoiceParserFactory, InvoiceParserFactory>();

        return services;
    }
}
