using System;
using System.Linq.Expressions;
using TicketProject.Services.Implement;
using Xunit;

namespace TicketProject.Tests.Services.Implement
{
    /// <summary>
    /// 測試 DynamicQueryBuilderService 類別。
    /// </summary>
    public class DynamicQueryBuilderServiceTests
    {
        private readonly DynamicQueryBuilderService<TestEntity> _service;

        /// <summary>
        /// 初始化 DynamicQueryBuilderServiceTests 類的新實例。
        /// </summary>
        public DynamicQueryBuilderServiceTests()
        {
            _service = new DynamicQueryBuilderService<TestEntity>();
        }

        /// <summary>
        /// 測試 BuildFilter 方法在使用 "And" 條件時正確結合過濾表達式。
        /// </summary>
        [Fact]
        public void BuildFilter_ShouldCombineFiltersWithAndCondition()
        {
            // Arrange
            Expression<Func<TestEntity, bool>> filter1 = x => x.Id > 0;
            Expression<Func<TestEntity, bool>> filter2 = x => x.Name == "Test";

            // Act
            var combinedFilter = _service.BuildFilter(filter1, filter2, "And");

            // Assert
            var compiledFilter = combinedFilter.Compile();
            Assert.True(compiledFilter(new TestEntity { Id = 1, Name = "Test" }));
            Assert.False(compiledFilter(new TestEntity { Id = 1, Name = "NotTest" }));
        }

        /// <summary>
        /// 測試 BuildFilter 方法在使用 "Or" 條件時正確結合過濾表達式。
        /// </summary>
        [Fact]
        public void BuildFilter_ShouldCombineFiltersWithOrCondition()
        {
            // Arrange
            Expression<Func<TestEntity, bool>> filter1 = x => x.Id > 0;
            Expression<Func<TestEntity, bool>> filter2 = x => x.Name == "Test";

            // Act
            var combinedFilter = _service.BuildFilter(filter1, filter2, "Or");

            // Assert
            var compiledFilter = combinedFilter.Compile();
            Assert.True(compiledFilter(new TestEntity { Id = 1, Name = "Test" }));
            Assert.True(compiledFilter(new TestEntity { Id = 1, Name = "NotTest" }));
            Assert.False(compiledFilter(new TestEntity { Id = 0, Name = "NotTest" }));
        }

        /// <summary>
        /// 測試 ReplaceParameter 方法正確替換參數。
        /// </summary>
        [Fact]
        public void ReplaceParameter_ShouldReplaceParameterCorrectly()
        {
            // Arrange
            Expression<Func<TestEntity, bool>> filter = x => x.Id > 0;
            var parameter = Expression.Parameter(typeof(TestEntity), "y");

            // Act
            var replacedExpression = ReplaceParameter(filter.Body, filter.Parameters[0], parameter);

            // Assert
            var lambda = Expression.Lambda<Func<TestEntity, bool>>(replacedExpression, parameter);
            var compiledFilter = lambda.Compile();
            Assert.True(compiledFilter(new TestEntity { Id = 1 }));
            Assert.False(compiledFilter(new TestEntity { Id = 0 }));
        }

        /// <summary>
        /// 測試用的實體類別。
        /// </summary>
        private class TestEntity
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        /// <summary>
        /// 替換表達式中的參數。
        /// </summary>
        /// <param name="expression">要替換的表達式。</param>
        /// <param name="oldParameter">舊的參數。</param>
        /// <param name="newParameter">新的參數。</param>
        /// <returns>替換後的表達式。</returns>
        private Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
        }

        /// <summary>
        /// 用於替換表達式參數的訪問者類別。
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }
    }
}
