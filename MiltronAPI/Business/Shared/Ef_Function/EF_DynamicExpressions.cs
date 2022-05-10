using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Shared.Ef_Function
{
    public static class EF_DynamicExpressions
    {
        /// <summary>
        /// Standart iş kuralları dışında kullanılması önerilmez
        /// T -> Üzerinde çalışılacak Entity modeli
        /// </summary>
        /// <param name="propertyToFilter">Filtrelenecek property(sütun) adı</param>
        /// <param name="value"> Filtre koşulu değeri (id == 5) teki 5 </param>
        /// <param name="typeOfEntity">Model sınıfının tipi</param>
        /// <param name="recordList">Filtrelenecek DbSet parametresi</param>
        /// <param name="expectedRecord">İlle de muhakkak bir kayıt dönmesini bekliyorsak(true ise ve kayıt dönmezse hata gönderir, false ise null gönderir)</param>
        /// <returns>Tek bir record, döndüremezse hata veya null döner</returns>
        public static async Task<T> FilterPredicateWithOneParameterSingleOutput<T>(string propertyToFilter, string value, Type typeOfEntity, IAsyncQueryable<T> recordList, bool expectedRecord = true) where T : class
        {
            if ((String.IsNullOrEmpty(propertyToFilter)) || (String.IsNullOrEmpty(value))
                || (typeOfEntity == null) || (recordList == null))
            {
                throw new ArgumentException();
            }

            try
            {
                //Tablo 

                ParameterExpression exp = Expression.Parameter(typeOfEntity);

                //expressionun solu
                MemberExpression memberAccess = Expression.PropertyOrField(exp, propertyToFilter);

                //expressionun sağı
                //önce stringi propertyToFilterin tipine çevirelim
                object valueConverted = Convert.ChangeType(value, memberAccess.Type);
                ConstantExpression exprRight = Expression.Constant(valueConverted);

                //koşul (bu durumda ==)
                BinaryExpression equalExpr = Expression.Equal(memberAccess, exprRight);

                //üretilen expression
                Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(equalExpr, exp);

                if (expectedRecord)
                {
                    return await recordList.SingleAsync(lambda);
                }
                else
                {
                    //Record yoksa null döner
                    return await recordList.SingleOrDefaultAsync(lambda);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Standart iş kuralları dışında kullanılması önerilmez
        /// </summary>
        /// <param name="propertiesToFilter">Filtrelenecek property(sütun) listesi</param>
        /// <param name="values">Propertylerin(sütunların) eşit olacağı değerler</param>
        /// <param name="andOr">true -> expressionlar andlenecek(where(... && ...) gibi
        ///                      false -> expressionlar orlanacak(where(... || ...) gibi</param>
        /// <param name="typeOfEntity">Entity(tablo)tipi</param>
        /// <param name="recordList">Entity(tablonun kendisi)</param>
        /// <param name="expectedRecord">İlle de muhakkak bir kayıt dönmesini bekliyorsak(true ise ve kayıt dönmezse hata gönderir, false ise null gönderir)</param>
        /// <returns>Tek bir record, döndüremezse hata veya null döner</returns>
        public static async Task<T> FilterPredicateWithMultiParameterSingleOutput<T>(List<string> propertiesToFilter, List<object> values, bool andOr, Type typeOfEntity, IAsyncQueryable<T> recordList, bool expectedRecord = true) where T : class
        {
            if ((propertiesToFilter == null) || (propertiesToFilter.Count < 2) ||
                    (values == null) || (values.Count < 2) || (propertiesToFilter.Count != values.Count) ||
                      (typeOfEntity == null) || (recordList == null))
            {
                throw new ArgumentException();
            }

            try
            {
                Expression<Func<T, bool>> lambda = null;

                for (int i = 0; i < propertiesToFilter.Count; i++)
                {
                    if ((String.IsNullOrEmpty(propertiesToFilter[i])) || (values[i] == null))
                    {
                        return null;
                    }

                    //Tablo
                    ParameterExpression expInner = Expression.Parameter(typeOfEntity);

                    //expressionun solu
                    MemberExpression memberAccessInner = Expression.PropertyOrField(expInner, propertiesToFilter[i]);

                    //expressionun sağı
                    ConstantExpression exprRightInner = Expression.Constant(values[i]);

                    //koşul (bu durumda ==)
                    BinaryExpression equalExprInner = Expression.Equal(memberAccessInner, exprRightInner);

                    //üretilen expression
                    Expression<Func<T, bool>> lambdaRightInner = Expression.Lambda<Func<T, bool>>(equalExprInner, expInner);

                    if (lambda != null)
                    {
                        if (andOr)
                        {
                            //İki expressionu andleyelim
                            lambda = lambda.And<T>(lambdaRightInner);
                        }
                        else
                        {
                            //İki expressionu orlayalım
                            lambda = lambda.Or<T>(lambdaRightInner);
                        }
                    }
                    else
                    {
                        //ilk ifade(indeks 0)
                        lambda = lambdaRightInner;
                    }
                }

                if (expectedRecord)
                {
                    return await recordList.SingleAsync(lambda);
                }
                else
                {
                    //Record yoksa null döner
                    return await recordList.SingleOrDefaultAsync(lambda);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Standart iş kuralları dışında kullanılması önerilmez
        /// </summary>
        /// <param name="propertyToFilter">Filtrelenecek property(sütun) adı</param>
        /// <param name="value"> Filtre koşulu değeri (id == 5) teki 5 </param>
        /// <param name="typeOfEntity">Model sınıfının tipi</param>
        /// <param name="recordList">Filtrelenecek DbSet parametresi</param>
        /// <returns>Sonuç Listesi</returns>
        public static async Task<IEnumerable<T>> FilterPredicateWithOneParameterMultiOutput<T>(string propertyToFilter, string value, Type typeOfEntity, IAsyncQueryable<T> recordList, bool expectedRecord = false) where T : class
        {
            if ((String.IsNullOrEmpty(propertyToFilter)) || (String.IsNullOrEmpty(value)) ||
                 (typeOfEntity == null) || (recordList == null))
            {
                throw new ArgumentException();
            }

            try
            {
                //Tablo
                ParameterExpression exp = Expression.Parameter(typeOfEntity);

                //expressionun solu
                MemberExpression memberAccess = Expression.PropertyOrField(exp, propertyToFilter);

                //expressionun sağı
                ConstantExpression exprRight = Expression.Constant(value);

                //koşul (bu durumda ==)
                BinaryExpression equalExpr = Expression.Equal(memberAccess, exprRight);

                //üretilen expression
                Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(equalExpr, exp);

                if (expectedRecord)
                {
                    IEnumerable<T> tempReturn = await recordList.Where(lambda).ToListAsync();

                    if (tempReturn.Count() > 0)
                    {
                        return tempReturn;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                return await recordList.Where(lambda).ToListAsync();
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
        }


        /// <summary>
        /// Standart iş kuralları dışında kullanılması önerilmez
        /// </summary>
        /// <param name="propertiesToFilter">Filtrelenecek property(sütun) listesi</param>
        /// <param name="values">Propertylerin(sütunların) eşit olacağı değerler</param>
        /// <param name="andOr">true -> expressionlar andlenecek(where(... && ...) gibi
        ///                      false -> expressionlar orlanacak(where(... || ...) gibi</param>
        /// <param name="typeOfEntity">Entity(tablo)tipi</param>
        /// <param name="recordList">Entity(tablonun kendisi)</param>
        /// <param name="expectedRecord">İlle de muhakkak bir kayıt dönmesini bekliyorsak(true ise ve kayıt dönmezse hata gönderir, false ise boş bir liste(IEnumerable<T>) gönderir. Single outputların aksine falsedur</param>
        /// <returns>Sonuç listesi</returns>
        public static async Task<IEnumerable<T>> FilterPredicateWithMultiParameterMultiOutput<T>(List<string> propertiesToFilter, List<string> values, bool andOr, Type typeOfEntity, IAsyncQueryable<T> recordList, bool expectedRecord = false) where T : class
        {
            if ((propertiesToFilter == null) || (propertiesToFilter.Count < 2) ||
                    (values == null) || (values.Count < 2) || (propertiesToFilter.Count != values.Count) ||
                      (typeOfEntity == null) || (recordList == null))
            {
                throw new ArgumentException();
            }

            try
            {
                Expression<Func<T, bool>> lambda = null;

                for (int i = 0; i < propertiesToFilter.Count; i++)
                {
                    if ((String.IsNullOrEmpty(propertiesToFilter[i])) || (String.IsNullOrEmpty(values[i])))
                    {
                        return null;
                    }

                    //Tablo
                    ParameterExpression expInner = Expression.Parameter(typeOfEntity);

                    //expressionun solu
                    MemberExpression memberAccessInner = Expression.PropertyOrField(expInner, propertiesToFilter[i]);

                    //expressionun sağı
                    ConstantExpression exprRightInner = Expression.Constant(values[i]);

                    //koşul (bu durumda ==)
                    BinaryExpression equalExprInner = Expression.Equal(memberAccessInner, exprRightInner);

                    //üretilen expression
                    Expression<Func<T, bool>> lambdaRightInner = Expression.Lambda<Func<T, bool>>(equalExprInner, expInner);

                    if (lambda != null)
                    {
                        if (andOr)
                        {
                            //İki expressionu andleyelim
                            lambda = lambda.And<T>(lambdaRightInner);
                        }
                        else
                        {
                            //İki expressionu orlayalım
                            lambda = lambda.Or<T>(lambdaRightInner);
                        }
                    }
                    else
                    {
                        //ilk ifade(indeks 0)
                        lambda = lambdaRightInner;
                    }
                }

                if (expectedRecord)
                {
                    IEnumerable<T> tempReturn = await recordList.Where(lambda).ToListAsync();

                    if (tempReturn.Count() > 0)
                    {
                        return tempReturn;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                return await recordList.Where(lambda).ToListAsync();
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
        }

        //lambda expression andlemeye yarayan generic extension metod
        //Where içinde farklı entitylerle çalışmaya da imkan veriyor
        //Şimdilik sadece bu statik sınıfa açık olsun
        //Nasıl kullanıldığını FilterPredicateWithMultiParameterMultiOutput veya FilterPredicateWithMultiParameterSingleOutput da görebilirsiniz
        private static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        //lambda expression orlamaya yarayan generic extension metod
        //Where içinde farklı entitylerle çalışmaya da imkan veriyor
        //Şimdilik sadece bu statik sınıfa açık olsun
        //Nasıl kullanıldığını FilterPredicateWithMultiParameterMultiOutput veya FilterPredicateWithMultiParameterSingleOutput da görebilirsiniz
        private static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

    }
}
