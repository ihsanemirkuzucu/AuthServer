using AutoMapper;

namespace AuthServer.Service.Mappers
{
    public static class ObjectMapper
    {
        #region Kısa hali core 8.0

        //private static readonly Lazy<IMapper> _lazy = new(() =>
        //    new MapperConfiguration(cfg => cfg.AddProfile<DtoMapper>())
        //        .CreateMapper());

        #endregion

        private static readonly Lazy<IMapper> _lazy = new(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoMapper>();
            });
            return config.CreateMapper();
        });

        public static IMapper Mapper => _lazy.Value;// propertynin get kısmı: ' => _lazy.Value '
    }
}
