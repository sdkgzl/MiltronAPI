using AutoMapper;
using Business.BusinessRules.GenericBusinessRules.Interface;
using Business.Helper.Interfaces;
using Business.Shared.Ef_Function;
using Business.Shared.Enums;
using DataAccess.Context;
using DataAccess.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Shared.Dto;
using Shared.Helpers;
using Shared.Operations.EnumOperations;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessRules.GenericBusinessRules.Abstract
{
    public class GenericBusinessRules<TDto, TAutomapperProfile> : IGenericBusinessRules<TDto, TAutomapperProfile> where TDto : BaseDto, new()
                                                                                                                                     where TAutomapperProfile : Profile, new()
    {
        private MiltronDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public GenericBusinessRules(MiltronDbContext context,IMapper mapper, IUnitOfWork unitOfWork)
        {
            _context = context;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<TDto>> Insert(TDto insertedDto, List<string> uniqueColumns = null, List<object> uniqueValues = null)
        {
            //sonrasında UI dan gönderilebilir
            insertedDto.IsDeleted = false;
            insertedDto.IsActive = true;
            TDto returnDto = new();
            //Dtodan maplemeye bakarak Entity sınıfını bul
            //Olası duplicate isimli sınıflara karşılık assembly ve namespaceli isim alalım
            string dtoClassFullName = typeof(TDto).FullName;
            EntityDtoMap entityDtoMapEnum = dtoClassFullName.GetValueFromDescription<EntityDtoMap>();
            string workingEntityFullName = entityDtoMapEnum.GetDisplayOrValueFromEnum<EntityDtoMap>().ToString();
                        

            //Burada muhakkak DiasDataAccessLayer de hep olacak bir sınıf yazmalıyız
            Assembly dalAssembly = typeof(DataAccessLayerEnums).Assembly;
            //Dto ya karşılık gelen entity'in tipini al
            Type tEntity = dalAssembly.GetType(workingEntityFullName);

            //DbSet<Entity> karşılık gelen tipi al
            Type genericListEntityDbSetType = typeof(DbSet<>).MakeGenericType(tEntity);

            dynamic existentRecordList = new ExpandoObject();
            dynamic existentRecord = new ExpandoObject();

            dynamic convertedEntity = new ExpandoObject();

            using (_context)
            {
                //Contexte böyle bir tablo(entity) var mı?
                foreach (PropertyInfo pi in typeof(MiltronDbContext).GetProperties())
                {
                    if (pi.PropertyType == genericListEntityDbSetType)
                    {
                        //Tüm tabloyu çek
                        existentRecordList = await Task.Run(() => pi.GetValue(_context));
                        dynamic instanceDerived = Activator.CreateInstance(tEntity);

                        //Burada InnerDBSet'i IQueryable hale dönüştürüyoruz
                        //Sebebi ve yapılan işlem GetByIdFromInt de açıklandı
                        var queryableParam = CallToQueryableWithDynamic(instanceDerived, existentRecordList, tEntity);

                        if ((uniqueColumns != null) && (uniqueValues != null))
                        {
                            if ((uniqueColumns.Count == 1) && (uniqueColumns[0] != null) &&
                                (uniqueValues.Count == 1) && (uniqueValues[0] != null))
                            {
                                //Filtrele
                                existentRecord = await EF_DynamicExpressions.FilterPredicateWithOneParameterSingleOutput(uniqueColumns[0], uniqueValues[0].ToString(), tEntity, queryableParam, false);
                            }
                            else
                            {
                                //Filtrele(by unique parameters)
                                existentRecord = await EF_DynamicExpressions.FilterPredicateWithMultiParameterSingleOutput(uniqueColumns, uniqueValues, true, tEntity, queryableParam, false);
                            }
                        }
                    }
                }

                if (existentRecordList != null)
                {
                    if ((uniqueColumns != null) && (uniqueValues != null))
                    {
                        //Aynı kayıt veritabanında var mı?
                        if (existentRecord == null)
                        {
                            convertedEntity = _mapper.Map(insertedDto, typeof(TDto), tEntity);
                        }
                        else
                        {
                            //Aynı kayıt varsa hata gönder
                            return Response<TDto>.Fail("Aynı Kayıt Vardır", 404, true);
                        }
                    }
                    else
                    {
                        convertedEntity = _mapper.Map(insertedDto, typeof(TDto), tEntity);
                    }                    
                    var entityUser = existentRecordList.AddAsync(convertedEntity);
                    await _unitOfWork.CommmitAsync();
                    
                    return Response<TDto>.Success(insertedDto,"Başarılı Şekilde Kayıt Yapıldı",200);
                }
                else
                {
                    //Tablo bulunamadı
                    return Response<TDto>.Fail("Tablo Bulunamadı", 404, true);
                }
            }
        }

        // <summary>
        /// InternalDbSet'i IAsyncQueryable<T> yapmak için ara geçiş metodu
        /// </summary>
        private IAsyncQueryable<T> CallToQueryableWithDynamic<T>(T ignored, dynamic instance, Type tEntity) where T : class
        {
            //InternalDbSet<T> i önce IAsyncEnumerable<T> e sonra IAsyncQueryable<T> e çevireceğiz
            //IAsyncEnumerable<T> değişkenine yeni bir memory alanı assign etmek zorundayız yoksa yine InternalDbSet<T> döner
            IAsyncEnumerable<T> tempFirst = AsyncEnumerable.Empty<T>();
            IAsyncQueryable<T> tempSecond;
            tempFirst = instance.AsAsyncEnumerable();
            tempSecond = tempFirst.AsAsyncQueryable();
            return tempSecond;
        }

    }


}
