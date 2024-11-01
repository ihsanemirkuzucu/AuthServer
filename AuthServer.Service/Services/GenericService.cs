using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWorks;
using AuthServer.Service.Mappers;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTOs;
using System.Linq.Expressions;
using System.Net;

namespace AuthServer.Service.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class
        where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;

        public GenericService(IGenericRepository<TEntity> genericRepository, IUnitOfWork unitOfWork)
        {
            _genericRepository = genericRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<TDto>> GetByIdAsync(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);
            if (product is null)
            {
                return ResponseDto<TDto>.Fail("ID not found", HttpStatusCode.NotFound.GetHashCode(), true);
            }

            var productDto = ObjectMapper.Mapper.Map<TDto>(product);
            return ResponseDto<TDto>.Success(productDto, HttpStatusCode.OK.GetHashCode());
        }

        public async Task<ResponseDto<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());
            return ResponseDto<IEnumerable<TDto>>.Success(products, HttpStatusCode.OK.GetHashCode());
        }

        public async Task<ResponseDto<IEnumerable<TDto>>> WhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var list = await _genericRepository.Where(predicate).ToListAsync();
            var listedDto = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(list);
            return ResponseDto<IEnumerable<TDto>>.Success(listedDto, HttpStatusCode.OK.GetHashCode());
        }

        public async Task<ResponseDto<TDto>> AddAsync(TDto dto)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
            return ResponseDto<TDto>.Success(newDto, HttpStatusCode.Created.GetHashCode());
        }

        public async Task<ResponseDto<NoDataDto>> RemoveAsync(int id)
        {
            var isExsitEntity = await _genericRepository.GetByIdAsync(id);
            if (isExsitEntity is null)
            {
                return ResponseDto<NoDataDto>.Fail("ID not found", HttpStatusCode.NotFound.GetHashCode(), true);
            }
            _genericRepository.Remove(isExsitEntity);
            await _unitOfWork.CommitAsync();
            return ResponseDto<NoDataDto>.Success(HttpStatusCode.OK.GetHashCode());
        }

        public async Task<ResponseDto<NoDataDto>> UpdateAsync(TDto dto, int id)
        {
            var isExsitEntity = await _genericRepository.GetByIdAsync(id);
            if (isExsitEntity is null)
            {
                return ResponseDto<NoDataDto>.Fail("ID not found", HttpStatusCode.NotFound.GetHashCode(), true);
            }
            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
            _genericRepository.Update(updateEntity);
            await _unitOfWork.CommitAsync();
            return ResponseDto<NoDataDto>.Success(HttpStatusCode.NoContent.GetHashCode());
        }
    }
}
