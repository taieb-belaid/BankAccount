#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
namespace BankAccount.Models;
public class Transaction
{
    [Key]
    public int TransactionId {get;set;}
    [Required]
    public double Amount{get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public int UserId{get;set;}
    public User Transfer{get;set;}
}