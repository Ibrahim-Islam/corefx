﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Xunit;

public static class NullReferenceExceptionTests
{
    private const string exceptionMessage = "Created an exception";
    private const string innerExceptionMessage = "Created an InnerException";
    private const uint COR_E_ENDOFSTREAM = 0x80070026;
    private const uint E_POINTER = 0x80004003;

    [Fact]
    public static void NullReferenceException_Ctor_Empty()
    {
        NullReferenceException i = new NullReferenceException();
        Assert.Equal(E_POINTER, (uint)i.HResult);
    }

    [Fact]
    public static void NullReferenceException_Ctor_String()
    {
        NullReferenceException i = new NullReferenceException(exceptionMessage);
        Assert.Equal(exceptionMessage, i.Message);
        Assert.Equal(E_POINTER, (uint)i.HResult);
    }

    [Fact]
    public static void NullReferenceException_Ctor_String_Exception()
    {
        Exception ex = new Exception(innerExceptionMessage);
        NullReferenceException i = new NullReferenceException(exceptionMessage, ex);

        Assert.Equal(exceptionMessage, i.Message);
        Assert.Equal(i.InnerException.Message, innerExceptionMessage);
        Assert.Equal(i.InnerException.HResult, ex.HResult);
        Assert.Equal(E_POINTER, (uint)i.HResult);
    }

    [Fact]
    public static void EndOfStreamException_Ctor_Empty()
    {
        EndOfStreamException i = new EndOfStreamException();
        Assert.Equal(COR_E_ENDOFSTREAM, (uint)i.HResult);
    }

    [Fact]
    public static void EndOfStreamException_Ctor_String()
    {
        EndOfStreamException i = new EndOfStreamException(exceptionMessage);
        Assert.Equal(exceptionMessage, i.Message);
        Assert.Equal(COR_E_ENDOFSTREAM, (uint)i.HResult);
    }

    [Fact]
    public static void EndOfStreamException_Ctor_String_Exception()
    {
        Exception ex = new Exception(innerExceptionMessage);
        EndOfStreamException i = new EndOfStreamException(exceptionMessage, ex);

        Assert.Equal(exceptionMessage, i.Message);
        Assert.Equal(i.InnerException.Message, innerExceptionMessage);
        Assert.Equal(i.InnerException.HResult, ex.HResult);
        Assert.Equal(COR_E_ENDOFSTREAM, (uint)i.HResult);
    }
}
