using SoepkipAPI.Models;

namespace SoepkipAPI.Data.Interfaces;

public interface ITrashRepository : IModelRepository<TrashItem>
{
    List<TrashItem> ReadRange(DateTime dateLeft, DateTime dateRight);
    bool TryParseIsoUtc(string input, out DateTime result);
}