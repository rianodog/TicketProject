using System;
using System.Linq.Expressions;
using TicketProject.Services.Implement;
using Xunit;

namespace TicketProject.Tests.Services.Implement
{
    /// <summary>
    /// ���� DynamicQueryBuilderService ���O�C
    /// </summary>
    public class DynamicQueryBuilderServiceTests
    {
        private readonly DynamicQueryBuilderService<TestEntity> _service;

        /// <summary>
        /// ��l�� DynamicQueryBuilderServiceTests �����s��ҡC
        /// </summary>
        public DynamicQueryBuilderServiceTests()
        {
            _service = new DynamicQueryBuilderService<TestEntity>();
        }

        /// <summary>
        /// ���� BuildFilter ��k�b�ϥ� "And" ����ɥ��T���X�L�o��F���C
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
        /// ���� BuildFilter ��k�b�ϥ� "Or" ����ɥ��T���X�L�o��F���C
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
        /// ���� ReplaceParameter ��k���T�����ѼơC
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
        /// ���եΪ��������O�C
        /// </summary>
        private class TestEntity
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        /// <summary>
        /// ������F�������ѼơC
        /// </summary>
        /// <param name="expression">�n��������F���C</param>
        /// <param name="oldParameter">�ª��ѼơC</param>
        /// <param name="newParameter">�s���ѼơC</param>
        /// <returns>�����᪺��F���C</returns>
        private Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
        }

        /// <summary>
        /// �Ω������F���Ѽƪ��X�ݪ����O�C
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
