namespace App.Application.Contracts.Persistance;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
