#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace BankAccount.Models;

public class Login
{
    [Required]
    [EmailAddress]
    public string LoginEmail{get;set;}
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)] 
    public string LoginPassword{get;set;}
}   