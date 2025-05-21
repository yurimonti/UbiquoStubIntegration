using System;

namespace UbiquoStub.Exceptions;

public class NoStubInSUTException(string message) : Exception(message)
{

}
