
namespace Cars.Domain.Common.Interfaces
{
    public interface ISoftDeleteTable
    {
        bool IsDeleted { get; set; }
    }
}
