using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ExceptionsRefactoring.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    /// <remarks>
    /// 最もシンプルだがコードが冗長になりがちで try catch の制御もしんどい。Exception を吐くのも使い方としてよくない。
    /// </remarks>>

    #region pattern 0

    [HttpPost("pattern0/{name}")]
    public ActionResult Create0(string name)
    {
        try
        {
            ValidateName0(name);
            // some creation proc
            return Ok(name);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private void ValidateName0(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Name cannot be empty.");
        if (name.Length > 50) throw new ValidationException("Name is too long");
        if (name.Length > 50) throw new ValidationException("Name is too long");
    }

    #endregion pattern 0

    /// <remarks>
    /// </remarks>>

    #region pattern 1

    public ActionResult Create1(string name)
    {
        var errorMessage = ValidateName1(name);
        if (errorMessage != string.Empty) return BadRequest(errorMessage);
        // some creation proc
        return Ok(name);
    }

    private string ValidateName1(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Employee name should not be empty.";
        if (name.Length > 50) return "Employee name is too long";
        return string.Empty;
    }

    #endregion pattern 1

    #region pattern 2: Result pattern

    public ActionResult Create2(string name)
    {
        var result = ValidateName2(name);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok(result);
    }

    private Result ValidateName2(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Fail("Employee name should not be empty.");
        if (name.Length > 50) return Result.Fail("Employee name is too long.");
        return Result.Ok();
    }

    #endregion pattern 2: Result pattern

    #region ValueObject

    public ActionResult CreateUser(string email)
    {
        var result = Email.Create(email);
        if (result.IsFailure) return BadRequest(result.Error);
        var user = ExceptionsRefactoring.User.Create(result.Value);
        // some proc
        return new OkObjectResult(user);
    }

    #endregion ValueObject
}