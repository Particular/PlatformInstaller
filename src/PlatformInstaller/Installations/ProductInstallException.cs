using System;

// Custom Exception to send back to Raygun when a product install fails.
public class ProductInstallException : Exception
{
    public ProductInstallException(string message) : base(message)
    {
    }
}
