﻿
namespace Shay.Core.Domain.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }

        bool IsTransient();

    }

    public interface IEntity { }
}
