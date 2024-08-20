using MongoDB.Bson.Serialization.Attributes;

namespace RentApplication.Models;
public class User{
  public string Id {get; set;}="";
  public string Name {get; set;}="";
  public string Email {get; set;}="";
  public int PhoneNumber {get; set;}
}