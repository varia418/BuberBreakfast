using BuberBreakfast.Models;
using ErrorOr;

namespace BuberBreakfast.Services.Breakfasts;

public interface IBreakfastService
{
    ErrorOr<Breakfast> GetBreakfast(Guid breakfastId);
    ErrorOr<Created> CreateBreakfast(Breakfast breakfast);
    ErrorOr<UpsertedBreakfast> UpsertBreakfast(Breakfast breakfast);
    ErrorOr<Deleted> DeleteBreakfast(Guid breakfastId);
}