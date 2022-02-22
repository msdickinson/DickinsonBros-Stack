using Moq;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DickinsonBros.Infrastructure.SQL.Tests.Mocks.Models
{
    public class ParameterCollection : DbParameterCollection, System.Collections.IEnumerator
    {
        readonly object _lock = new object();
        internal int _dbParameterMocksCurrentIndex = -1;
        internal readonly List<Mock<DbParameter>> _dbParameterMocks;

        public ParameterCollection(List<Mock<DbParameter>> parameters)
        {
            _dbParameterMocks = parameters;
        }

        public override int Count => _dbParameterMocks.Count;

        public override object SyncRoot => _lock;

        public override int Add(object value)
        {
            var dbParameterMock = Moq.Mock.Get((DbParameter)value);
            _dbParameterMocks.Add(dbParameterMock);
            return _dbParameterMocks.Count - 1;
        }

        public override void AddRange(Array values)
        {
            values.OfType<DbParameter>().ForEach(dbParameter => Add(dbParameter));
        }

        public override void Clear()
        {
            _dbParameterMocks.Clear();
        }

        public override bool Contains(object value)
        {
            var dbParameterMock = Moq.Mock.Get((DbParameter)value);
            return _dbParameterMocks.Contains(dbParameterMock);
        }

        public override bool Contains(string value)
        {
            return _dbParameterMocks.Any(a => a.Object.ParameterName == value);
        }

        public override void CopyTo(Array array, int index)
        {
            int indexInsert = index;
            array.
                OfType<DbParameter>().
                ForEach
                (
                    dbParameter => Insert(indexInsert++, dbParameter)
                );
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return this;
        }

        public override int IndexOf(object value)
        {
            var dbParameterMock = Moq.Mock.Get((DbParameter)value);
            return _dbParameterMocks.IndexOf(dbParameterMock);
        }

        public override int IndexOf(string parameterName)
        {
            var dbParameterMock = _dbParameterMocks.FirstOrDefault(a => a.Object.ParameterName == parameterName);
            return _dbParameterMocks.IndexOf(dbParameterMock);
        }

        public override void Insert(int index, object value)
        {
            var dbParameterMock = Moq.Mock.Get((DbParameter)value);
            _dbParameterMocks.Insert(index, dbParameterMock);
        }

        public override void Remove(object value)
        {
            var dbParameterMock = Moq.Mock.Get((DbParameter)value);
            _dbParameterMocks.Remove(dbParameterMock);
        }

        public override void RemoveAt(int index)
        {
            _dbParameterMocks.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            var dbParameterMock = _dbParameterMocks.FirstOrDefault(a => a.Object.ParameterName == parameterName);
            _dbParameterMocks.Remove(dbParameterMock);
        }

        protected override DbParameter GetParameter(int index)
        {
            return _dbParameterMocks[index].Object;
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            var dbParameterMock = _dbParameterMocks.FirstOrDefault(a => a.Object.ParameterName == parameterName);
            return dbParameterMock.Object;
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            _dbParameterMocks.RemoveAt(index);
            var dbParameterMock = Moq.Mock.Get(value);
            _dbParameterMocks.Insert(index, dbParameterMock);
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var dbParameterMockOld = _dbParameterMocks.FirstOrDefault(a => a.Object.ParameterName == parameterName);
            _dbParameterMocks.Remove(dbParameterMockOld);
            var dbParameterMock = Moq.Mock.Get(value);
            _dbParameterMocks.Add(dbParameterMock);
        }

        object System.Collections.IEnumerator.Current => (_dbParameterMocksCurrentIndex >= _dbParameterMocks.Count) ? null : _dbParameterMocks[_dbParameterMocksCurrentIndex].Object;

        bool System.Collections.IEnumerator.MoveNext()
        {
            _dbParameterMocksCurrentIndex += (_dbParameterMocksCurrentIndex < _dbParameterMocks.Count) ? 1 : 0;
            return _dbParameterMocksCurrentIndex < _dbParameterMocks.Count;
        }

        void System.Collections.IEnumerator.Reset()
        {
            _dbParameterMocksCurrentIndex = -1;
        }
    }
}
