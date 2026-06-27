namespace Mansari.Store.Catalog.Domain.Common;

//فلسفه این بخش: جدا کنیم اکسپشن های سطح دامین قابل تشخیص بشن توی لایه های بیرونی 
public sealed class DomainException : Exception
{
    public DomainException(string message)
        : base(message)
    {
    }
}

