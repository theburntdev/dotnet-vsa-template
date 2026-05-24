using BackendTemplate.Domain.Common;
using Xunit;

namespace BackendTemplate.Domain.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_GivenValue_ThenIsSuccessTrue()
    {
        var result = Result<int>.Success(42);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Failure_GivenError_ThenIsFailureTrue()
    {
        var result = Result<int>.Failure("Something went wrong");
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("Something went wrong", result.Error);
    }

    [Fact]
    public void Failure_WhenNoKindProvided_ThenDefaultsToValidation()
    {
        var result = Result<int>.Failure("error");
        Assert.Equal(ErrorKind.Validation, result.Kind);
    }

    [Fact]
    public void Failure_GivenNotFoundKind_ThenKindIsNotFound()
    {
        var result = Result<int>.Failure("not found", ErrorKind.NotFound);
        Assert.Equal(ErrorKind.NotFound, result.Kind);
    }

    [Fact]
    public void Value_WhenFailure_ThenThrowsInvalidOperationException()
    {
        var result = Result<int>.Failure("error");
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void Error_WhenSuccess_ThenThrowsInvalidOperationException()
    {
        var result = Result<int>.Success(42);
        Assert.Throws<InvalidOperationException>(() => _ = result.Error);
    }

    [Fact]
    public void Kind_WhenSuccess_ThenThrowsInvalidOperationException()
    {
        var result = Result<int>.Success(42);
        Assert.Throws<InvalidOperationException>(() => _ = result.Kind);
    }
}
