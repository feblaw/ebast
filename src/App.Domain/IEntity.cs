using System;

namespace App.Domain
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
