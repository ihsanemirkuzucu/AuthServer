using SharedLibrary.DTOs;
using System.Linq.Expressions;

namespace AuthServer.Core.Services
{
    public interface IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        Task<ResponseDto<TDto>> GetByIdAsync(int id);
        Task<ResponseDto<IEnumerable<TDto>>> GetAllAsync();
        Task<ResponseDto<IEnumerable<TDto>>> WhereAsync(Expression<Func<TEntity, bool>> predicate);
        Task<ResponseDto<TDto>> AddAsync(TDto dto);
        Task<ResponseDto<NoDataDto>> RemoveAsync(int id);
        Task<ResponseDto<NoDataDto>> UpdateAsync(TDto dto,int id);
    }
}
