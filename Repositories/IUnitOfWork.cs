namespace StudentMangement.Repositories
{
    public interface IUnitOfWork
    {
        IStudentRepository Students { get; }
        IClassRepository Classes { get; }
        Task CompleteAsync();
    }
}
