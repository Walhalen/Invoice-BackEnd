using Domain.Features.Suppliers.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Features.Suppliers.Handlers.GetHandlers.GetSupplierByIdHandler;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdRequest, SupplierDto?>
{
    private readonly AppDbContext _db;

    public GetSupplierByIdHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByIdRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _db.Suppliers
            .FirstOrDefaultAsync(supplier => supplier.Id == request.Id, cancellationToken);

        if (supplier is null)
        {
            return null;
        }

        return new SupplierDto(
            supplier.Id,
            supplier.Name,
            supplier.Code,
            supplier.VatNumber,
            supplier.DefaultCurrencyCode,
            supplier.Email,
            supplier.PhoneNumber,
            supplier.IBAN,
            supplier.IsActive);
    }
}
